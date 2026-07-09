import SwiftUI

struct ChatListView: View {
    @StateObject private var viewModel = ChatViewModel()

    var body: some View {
        Group {
            if viewModel.isLoading && viewModel.conversations.isEmpty {
                ProgressView()
            } else if viewModel.conversations.isEmpty {
                ContentUnavailableView(L10n.Chat.title, systemImage: "message", description: Text(L10n.Chat.empty))
            } else {
                List(viewModel.conversations) { conversation in
                    NavigationLink {
                        BookingChatView(bookingId: conversation.bookingId)
                    } label: {
                        VStack(alignment: .leading, spacing: 4) {
                            Text(conversation.serviceName).font(.headline)
                            Text(conversation.bookingReference).font(.caption).foregroundStyle(.secondary)
                            if let preview = conversation.lastMessagePreview {
                                Text(preview).font(.subheadline).lineLimit(1)
                            }
                        }
                    }
                }
            }
        }
        .navigationTitle(L10n.Chat.title)
        .task { await viewModel.loadConversations() }
        .refreshable { await viewModel.loadConversations() }
    }
}

struct BookingChatView: View {
    let bookingId: UUID
    @StateObject private var viewModel = ChatViewModel()

    var body: some View {
        VStack(spacing: 0) {
            ScrollViewReader { proxy in
                ScrollView {
                    LazyVStack(alignment: .leading, spacing: 8) {
                        ForEach(viewModel.messages) { message in
                            HStack {
                                if message.isMine { Spacer() }
                                VStack(alignment: message.isMine ? .trailing : .leading) {
                                    if !message.isMine {
                                        Text(message.senderName).font(.caption).foregroundStyle(.secondary)
                                    }
                                    Text(message.body)
                                        .padding(10)
                                        .background(message.isMine ? AppTheme.Colors.primary.opacity(0.15) : Color.gray.opacity(0.15))
                                        .clipShape(RoundedRectangle(cornerRadius: 12))
                                }
                                if !message.isMine { Spacer() }
                            }
                            .id(message.id)
                        }
                    }
                    .padding()
                }
                .onChange(of: viewModel.messages.count) { _, _ in
                    if let last = viewModel.messages.last {
                        proxy.scrollTo(last.id, anchor: .bottom)
                    }
                }
            }

            HStack {
                TextField(L10n.Chat.messageHint, text: $viewModel.draft, axis: .vertical)
                    .textFieldStyle(.roundedBorder)
                Button {
                    Task { await viewModel.sendMessage() }
                } label: {
                    Image(systemName: "paperplane.fill")
                }
                .disabled(viewModel.isSending || viewModel.draft.trimmingCharacters(in: .whitespacesAndNewlines).isEmpty)
            }
            .padding()
        }
        .navigationTitle(viewModel.conversation?.serviceName ?? L10n.Chat.title)
        .task {
            await viewModel.openBookingChat(bookingId: bookingId)
        }
        .task {
            while !Task.isCancelled {
                try? await Task.sleep(nanoseconds: 5_000_000_000)
                await viewModel.refreshMessages()
            }
        }
    }
}
