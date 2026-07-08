import SwiftUI

@main
struct KhadamatiApp: App {
    @StateObject private var appSession = AppSession()

    var body: some Scene {
        WindowGroup {
            ContentView()
                .environmentObject(appSession)
                .environment(\.layoutDirection, appSession.layoutDirection)
                .preferredColorScheme(appSession.colorScheme)
        }
    }
}
