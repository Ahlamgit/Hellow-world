import SwiftUI

struct ContentView: View {
    @EnvironmentObject private var appSession: AppSession

    var body: some View {
        Group {
            if appSession.isAuthenticated {
                MainTabView()
            } else {
                AuthFlowView()
            }
        }
        .animation(.easeInOut(duration: 0.25), value: appSession.isAuthenticated)
    }
}

// MARK: - Main Tab Navigation

private struct MainTabView: View {
    @EnvironmentObject private var appSession: AppSession

    var body: some View {
        TabView {
            HomeView()
                .tabItem {
                    Label(L10n.Nav.home, systemImage: "house.fill")
                }

            ServicesView()
                .tabItem {
                    Label(L10n.Nav.services, systemImage: "wrench.and.screwdriver.fill")
                }

            MyBookingsView()
                .tabItem {
                    Label(L10n.Nav.bookings, systemImage: "calendar")
                }

            ProfileView()
                .tabItem {
                    Label(L10n.Nav.profile, systemImage: "person.fill")
                }
        }
        .tint(AppTheme.Colors.primary)
    }
}

// MARK: - Auth Flow

private struct AuthFlowView: View {
    @State private var showRegister = false

    var body: some View {
        NavigationStack {
            if showRegister {
                RegisterView(showRegister: $showRegister)
            } else {
                LoginView(showRegister: $showRegister)
            }
        }
    }
}

#Preview {
    ContentView()
        .environmentObject(AppSession())
}
