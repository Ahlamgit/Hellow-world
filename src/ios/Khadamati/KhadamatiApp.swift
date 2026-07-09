import SwiftUI

@main
struct KhadamatiApp: App {
    @UIApplicationDelegateAdaptor(AppDelegate.self) private var appDelegate
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
