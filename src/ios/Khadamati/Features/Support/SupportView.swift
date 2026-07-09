import SwiftUI

struct SupportView: View {
    @StateObject private var viewModel = SupportViewModel()
    @State private var tab = 0
    @State private var submitTab = 0
    @State private var subject = ""
    @State private var description = ""
    @State private var category = "General"

    var body: some View {
        VStack {
            Picker("", selection: $tab) {
                Text(L10n.Support.submit).tag(0)
                Text(L10n.Support.history).tag(1)
            }
            .pickerStyle(.segmented)
            .padding()

            if tab == 0 {
                Form {
                    Picker("", selection: $submitTab) {
                        Text(L10n.Support.complaint).tag(0)
                        Text(L10n.Support.ticket).tag(1)
                    }
                    .pickerStyle(.segmented)
                    TextField(L10n.Support.subject, text: $subject)
                    if submitTab == 1 {
                        TextField(L10n.Support.category, text: $category)
                    }
                    TextField(L10n.Support.description, text: $description, axis: .vertical)
                        .lineLimit(3...6)
                    Button(submitTab == 0 ? L10n.Support.submitComplaint : L10n.Support.submitTicket) {
                        Task {
                            if submitTab == 0 {
                                await viewModel.submitComplaint(subject: subject, description: description)
                            } else {
                                await viewModel.submitTicket(subject: subject, description: description, category: category)
                            }
                            subject = ""
                            description = ""
                        }
                    }
                    .disabled(subject.isEmpty || description.isEmpty)
                }
            } else if viewModel.isLoading {
                ProgressView()
            } else {
                List {
                    Section(L10n.Support.myComplaints) {
                        if viewModel.complaints.isEmpty {
                            Text(L10n.Support.noComplaints)
                        } else {
                            ForEach(viewModel.complaints) { item in
                                VStack(alignment: .leading) {
                                    Text(item.subject).font(.headline)
                                    Text(item.status).font(.caption).foregroundStyle(AppTheme.Colors.primary)
                                    Text(item.description).font(.subheadline)
                                }
                            }
                        }
                    }
                    Section(L10n.Support.myTickets) {
                        if viewModel.tickets.isEmpty {
                            Text(L10n.Support.noTickets)
                        } else {
                            ForEach(viewModel.tickets) { item in
                                VStack(alignment: .leading) {
                                    Text("\(item.ticketNumber) — \(item.subject)").font(.headline)
                                    Text(item.status).font(.caption).foregroundStyle(AppTheme.Colors.primary)
                                    Text(item.description).font(.subheadline)
                                }
                            }
                        }
                    }
                }
            }

            if let message = viewModel.message {
                Text(message).foregroundStyle(AppTheme.Colors.primary).padding()
            }
            if let error = viewModel.errorMessage {
                Text(error).foregroundStyle(AppTheme.Colors.error).padding()
            }
        }
        .navigationTitle(L10n.Support.title)
        .task { await viewModel.load() }
    }
}
