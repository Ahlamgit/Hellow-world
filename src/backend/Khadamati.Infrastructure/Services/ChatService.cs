using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Messaging;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using Khadamati.Domain.Entities;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Services;

public class ChatService : IChatService
{
    private readonly ApplicationDbContext _context;
    private readonly IPushNotificationService _pushNotificationService;
    private readonly IPermissionService _permissionService;

    public ChatService(
        ApplicationDbContext context,
        IPushNotificationService pushNotificationService,
        IPermissionService permissionService)
    {
        _context = context;
        _pushNotificationService = pushNotificationService;
        _permissionService = permissionService;
    }

    public async Task<IReadOnlyList<ChatConversationDto>> GetMyConversationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var conversations = await _context.ChatConversations
            .Include(c => c.Booking).ThenInclude(b => b.Service)
            .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
            .Where(c => c.CustomerId == userId || c.CraftsmanId == userId)
            .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var conversationIds = conversations.Select(c => c.Id).ToList();
        var unreadCounts = await _context.ChatMessages
            .Where(m => conversationIds.Contains(m.ConversationId) && m.SenderId != userId && !m.IsRead)
            .GroupBy(m => m.ConversationId)
            .Select(g => new { ConversationId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);
        var unreadMap = unreadCounts.ToDictionary(x => x.ConversationId, x => x.Count);

        var customerIds = conversations.Select(c => c.CustomerId).Distinct().ToList();
        var craftsmanIds = conversations.Select(c => c.CraftsmanId).Distinct().ToList();
        var userIds = customerIds.Concat(craftsmanIds).Distinct().ToList();

        var profiles = await _context.UserProfiles
            .Where(p => userIds.Contains(p.UserId))
            .AsNoTracking()
            .ToDictionaryAsync(p => p.UserId, cancellationToken);

        return conversations.Select(c =>
        {
            var last = c.Messages.FirstOrDefault();
            profiles.TryGetValue(c.CustomerId, out var customerProfile);
            profiles.TryGetValue(c.CraftsmanId, out var craftsmanProfile);
            return new ChatConversationDto
            {
                Id = c.Id,
                BookingId = c.BookingId,
                BookingReference = c.Booking.BookingReference,
                ServiceName = c.Booking.Service?.NameEn ?? string.Empty,
                CustomerId = c.CustomerId,
                CustomerName = FormatName(customerProfile),
                CraftsmanId = c.CraftsmanId,
                CraftsmanName = FormatName(craftsmanProfile),
                LastMessageAt = c.LastMessageAt,
                LastMessagePreview = last?.Body,
                UnreadCount = unreadMap.GetValueOrDefault(c.Id),
            };
        }).ToList();
    }

    public async Task<ChatConversationDto> GetOrCreateBookingConversationAsync(
        Guid bookingId, Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var booking = await _context.ServiceRequests
            .Include(b => b.Service)
            .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken)
            ?? throw new NotFoundException("Booking not found.");

        var isCustomer = role == "Customer" && booking.CustomerId == userId;
        var isCraftsman = role == "Craftsman" && booking.CraftsmanId == userId;
        var hasAdminAccess = await _permissionService.UserHasPermissionAsync(userId, PermissionCodes.BookingsView, cancellationToken);
        if (!isCustomer && !isCraftsman && !hasAdminAccess)
            throw new ForbiddenException("Access denied.");

        var conversation = await _context.ChatConversations
            .FirstOrDefaultAsync(c => c.BookingId == bookingId, cancellationToken);

        if (conversation == null)
        {
            conversation = new ChatConversation
            {
                BookingId = bookingId,
                CustomerId = booking.CustomerId,
                CraftsmanId = booking.CraftsmanId,
            };
            await _context.ChatConversations.AddAsync(conversation, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var profiles = await _context.UserProfiles
            .Where(p => p.UserId == booking.CustomerId || p.UserId == booking.CraftsmanId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var customerProfile = profiles.FirstOrDefault(p => p.UserId == booking.CustomerId);
        var craftsmanProfile = profiles.FirstOrDefault(p => p.UserId == booking.CraftsmanId);

        return new ChatConversationDto
        {
            Id = conversation.Id,
            BookingId = conversation.BookingId,
            BookingReference = booking.BookingReference,
            ServiceName = booking.Service?.NameEn ?? string.Empty,
            CustomerId = conversation.CustomerId,
            CustomerName = FormatName(customerProfile),
            CraftsmanId = conversation.CraftsmanId,
            CraftsmanName = FormatName(craftsmanProfile),
            LastMessageAt = conversation.LastMessageAt,
        };
    }

    public async Task<PagedResult<ChatMessageDto>> GetMessagesAsync(
        Guid conversationId, Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var conversation = await GetAuthorizedConversationAsync(conversationId, userId, cancellationToken);

        var query = _context.ChatMessages.Where(m => m.ConversationId == conversationId);
        var total = await query.CountAsync(cancellationToken);
        var messages = await query
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var senderIds = messages.Select(m => m.SenderId).Distinct().ToList();
        var profiles = await _context.UserProfiles
            .Where(p => senderIds.Contains(p.UserId))
            .AsNoTracking()
            .ToDictionaryAsync(p => p.UserId, cancellationToken);

        var unread = await _context.ChatMessages
            .Where(m => m.ConversationId == conversationId && m.SenderId != userId && !m.IsRead)
            .ToListAsync(cancellationToken);
        foreach (var message in unread)
        {
            message.IsRead = true;
            message.UpdatedAt = DateTime.UtcNow;
        }
        if (unread.Count > 0)
            await _context.SaveChangesAsync(cancellationToken);

        return new PagedResult<ChatMessageDto>
        {
            Items = messages
                .OrderBy(m => m.SentAt)
                .Select(m => MapMessage(m, userId, profiles.GetValueOrDefault(m.SenderId)))
                .ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<ChatMessageDto> SendMessageAsync(
        Guid conversationId, Guid userId, SendChatMessageDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Body))
            throw new ValidationException(new[] { "Message body is required." });

        var conversation = await GetAuthorizedConversationAsync(conversationId, userId, cancellationToken);

        var message = new ChatMessage
        {
            ConversationId = conversationId,
            SenderId = userId,
            Body = request.Body.Trim(),
            SentAt = DateTime.UtcNow,
        };

        await _context.ChatMessages.AddAsync(message, cancellationToken);
        conversation.LastMessageAt = message.SentAt;
        conversation.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        var senderProfile = await _context.UserProfiles.AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        var recipientId = userId == conversation.CustomerId ? conversation.CraftsmanId : conversation.CustomerId;
        await _pushNotificationService.SendAsync(new PushNotificationPayload
        {
            UserId = recipientId,
            TitleEn = "New chat message",
            TitleAr = "رسالة جديدة",
            MessageEn = request.Body.Trim().Length > 120 ? request.Body.Trim()[..120] + "…" : request.Body.Trim(),
            MessageAr = request.Body.Trim().Length > 120 ? request.Body.Trim()[..120] + "…" : request.Body.Trim(),
            NotificationType = "ChatMessage",
            ReferenceId = conversation.BookingId,
        }, cancellationToken);

        return MapMessage(message, userId, senderProfile);
    }

    private async Task<ChatConversation> GetAuthorizedConversationAsync(
        Guid conversationId, Guid userId, CancellationToken cancellationToken)
    {
        var conversation = await _context.ChatConversations
            .FirstOrDefaultAsync(c => c.Id == conversationId, cancellationToken)
            ?? throw new NotFoundException("Conversation not found.");

        if (conversation.CustomerId != userId && conversation.CraftsmanId != userId)
            throw new ForbiddenException("Access denied.");

        return conversation;
    }

    private static ChatMessageDto MapMessage(ChatMessage message, Guid userId, UserProfile? senderProfile) => new()
    {
        Id = message.Id,
        ConversationId = message.ConversationId,
        SenderId = message.SenderId,
        SenderName = FormatName(senderProfile),
        Body = message.Body,
        SentAt = message.SentAt,
        IsRead = message.IsRead,
        IsMine = message.SenderId == userId,
    };

    private static string FormatName(UserProfile? profile) =>
        profile == null ? string.Empty : $"{profile.FirstName} {profile.LastName}".Trim();
}
