using FluentAssertions;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Bookings;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Data;
using Khadamati.Infrastructure.Repositories;
using Khadamati.Infrastructure.Services;
using Khadamati.Infrastructure.Services.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Khadamati.Tests.Services;

public class BookingServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly BookingService _service;
    private readonly Guid _customerId;
    private readonly Guid _craftsmanId;
    private readonly Guid _serviceId;
    private readonly Guid _craftsmanProfileId;
    private readonly DateTime _slotStart;

    public BookingServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        _customerId = Guid.NewGuid();
        _craftsmanId = Guid.NewGuid();
        _serviceId = Guid.NewGuid();
        _craftsmanProfileId = Guid.NewGuid();
        _slotStart = DateTime.UtcNow.AddDays(2).Date.AddHours(10);

        var categoryId = Guid.NewGuid();
        _context.ServiceCategories.Add(new ServiceCategory
        {
            Id = categoryId,
            NameEn = "Plumbing",
            NameAr = "سباكة",
            IsActive = true,
        });

        _context.Services.Add(new Service
        {
            Id = _serviceId,
            CategoryId = categoryId,
            NameEn = "Leak Repair",
            NameAr = "إصلاح تسرب",
            BasePrice = 150m,
            EstimatedDurationMinutes = 60,
            IsActive = true,
        });

        _context.Users.AddRange(
            new User
            {
                Id = _customerId,
                Email = "customer@test.com",
                Phone = "+961700000001",
                PasswordHash = "hash",
                Role = UserRole.Customer,
                Status = UserStatus.Active,
                Profile = new UserProfile { FirstName = "Customer", LastName = "Test" },
            },
            new User
            {
                Id = _craftsmanId,
                Email = "craftsman@test.com",
                Phone = "+961700000002",
                PasswordHash = "hash",
                Role = UserRole.Craftsman,
                Status = UserStatus.Active,
                Profile = new UserProfile { FirstName = "Craft", LastName = "Man" },
            });

        _context.CraftsmanProfiles.Add(new CraftsmanProfile
        {
            Id = _craftsmanProfileId,
            UserId = _craftsmanId,
            Specialization = "Plumbing",
            IsAvailable = true,
            ServiceRadiusKm = 25m,
            Services =
            [
                new CraftsmanService
                {
                    ServiceId = _serviceId,
                    CustomPrice = 120m,
                    IsAvailable = true,
                },
            ],
        });

        _context.Addresses.Add(new Address
        {
            UserId = _craftsmanId,
            Label = "Workshop",
            City = "Beirut",
            Country = PlatformDefaults.CountryCode,
            Latitude = 33.8938m,
            Longitude = 35.5018m,
            IsDefault = true,
        });

        _context.SaveChanges();

        var bookingRepository = new BookingRepository(_context);
        var unitOfWork = new UnitOfWork(_context);
        var config = new ConfigurationBuilder().Build();
        var paymentGateway = new DevelopmentPaymentGateway(config);
        var pushMock = new Mock<IPushNotificationService>();
        pushMock.Setup(p => p.SendAsync(It.IsAny<Application.DTOs.Messaging.PushNotificationPayload>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var permissionMock = new Mock<IPermissionService>();
        permissionMock.Setup(p => p.UserHasPermissionAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _service = new BookingService(bookingRepository, unitOfWork, paymentGateway, pushMock.Object, permissionMock.Object);
    }

    [Fact]
    public async Task CreateBooking_ShouldPersistBooking()
    {
        var result = await _service.CreateBookingAsync(_customerId, new CreateBookingDto
        {
            ServiceId = _serviceId,
            CraftsmanId = _craftsmanId,
            ScheduledAt = _slotStart,
            Description = "Kitchen leak",
        });

        result.BookingReference.Should().StartWith("KHD-");
        result.Status.Should().Be(nameof(ServiceRequestStatus.Pending));
        result.EstimatedPrice.Should().Be(120m);

        var stored = await _context.ServiceRequests.SingleAsync();
        stored.CustomerId.Should().Be(_customerId);
        stored.CraftsmanId.Should().Be(_craftsmanId);
    }

    [Fact]
    public async Task CreateBooking_WhenSlotAlreadyReserved_ThrowsConflict()
    {
        await _context.BookingSlotReservations.AddAsync(new BookingSlotReservation
        {
            CraftsmanId = _craftsmanId,
            ServiceRequestId = Guid.NewGuid(),
            SlotStart = _slotStart,
            SlotEnd = _slotStart.AddHours(1),
            IsActive = true,
        });
        await _context.SaveChangesAsync();

        var act = () => _service.CreateBookingAsync(_customerId, new CreateBookingDto
        {
            ServiceId = _serviceId,
            CraftsmanId = _craftsmanId,
            ScheduledAt = _slotStart,
        });

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*no longer available*");
    }

    [Fact]
    public async Task ConfirmPayment_ShouldReserveSlotAndAdvanceStatus()
    {
        var booking = await CreateAwaitingPaymentBookingAsync();

        var result = await _service.ConfirmPaymentAsync(booking.Id, _customerId, new ConfirmPaymentDto
        {
            TransactionReference = $"KHD-{Guid.NewGuid():N}",
        });

        result.Status.Should().Be(nameof(ServiceRequestStatus.PendingCraftsmanConfirmation));

        var reservation = await _context.BookingSlotReservations.SingleAsync();
        reservation.CraftsmanId.Should().Be(_craftsmanId);
        reservation.SlotStart.Should().Be(_slotStart);
        reservation.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task ConfirmPayment_WhenSlotTaken_ThrowsConflict()
    {
        var booking = await CreateAwaitingPaymentBookingAsync();

        await _context.BookingSlotReservations.AddAsync(new BookingSlotReservation
        {
            CraftsmanId = _craftsmanId,
            ServiceRequestId = Guid.NewGuid(),
            SlotStart = _slotStart,
            SlotEnd = _slotStart.AddHours(1),
            IsActive = true,
        });
        await _context.SaveChangesAsync();

        var act = () => _service.ConfirmPaymentAsync(booking.Id, _customerId, new ConfirmPaymentDto
        {
            TransactionReference = $"KHD-{Guid.NewGuid():N}",
        });

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*booked by another customer*");
    }

    [Fact]
    public async Task AcceptBooking_ShouldMoveToConfirmed()
    {
        var booking = await CreatePendingConfirmationBookingAsync();

        var result = await _service.AcceptBookingAsync(booking.Id, _craftsmanId);

        result.Status.Should().Be(nameof(ServiceRequestStatus.Confirmed));
    }

    [Fact]
    public async Task GetNearbyCraftsmenForService_ShouldFilterByDistance()
    {
        var results = await _service.GetNearbyCraftsmenForServiceAsync(
            _serviceId, 33.8938, 35.5018, radiusKm: 5);

        results.Should().HaveCount(1);
        results[0].Id.Should().Be(_craftsmanId);
        results[0].DistanceKm.Should().BeLessThan(1);
    }

    [Fact]
    public async Task GetNearbyCraftsmenForService_OutsideRadius_ReturnsEmpty()
    {
        var results = await _service.GetNearbyCraftsmenForServiceAsync(
            _serviceId, 34.5, 36.0, radiusKm: 5);

        results.Should().BeEmpty();
    }

    private async Task<ServiceRequest> CreateAwaitingPaymentBookingAsync()
    {
        var booking = new ServiceRequest
        {
            BookingReference = "KHD-TEST-001",
            CustomerId = _customerId,
            ServiceId = _serviceId,
            CraftsmanId = _craftsmanId,
            ScheduledAt = _slotStart,
            SlotEnd = _slotStart.AddHours(1),
            EstimatedPrice = 120m,
            Status = ServiceRequestStatus.AwaitingPayment,
        };
        await _context.ServiceRequests.AddAsync(booking);
        await _context.BookingPayments.AddAsync(new BookingPayment
        {
            ServiceRequestId = booking.Id,
            PayerUserId = _customerId,
            PayeeUserId = _craftsmanId,
            Amount = 120m,
            Currency = PlatformDefaults.Currency,
            Status = PaymentStatus.Processing,
            PaymentMethod = "Card",
            TransactionReference = $"KHD-{Guid.NewGuid():N}",
        });
        await _context.SaveChangesAsync();
        return booking;
    }

    private async Task<ServiceRequest> CreatePendingConfirmationBookingAsync()
    {
        var booking = new ServiceRequest
        {
            BookingReference = "KHD-TEST-002",
            CustomerId = _customerId,
            ServiceId = _serviceId,
            CraftsmanId = _craftsmanId,
            ScheduledAt = _slotStart,
            SlotEnd = _slotStart.AddHours(1),
            EstimatedPrice = 120m,
            Status = ServiceRequestStatus.PendingCraftsmanConfirmation,
        };
        await _context.ServiceRequests.AddAsync(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public void Dispose() => _context.Dispose();
}
