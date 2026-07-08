import Foundation

enum L10n {
    enum App {
        static let name = NSLocalizedString("app.name", comment: "App name")
        static let tagline = NSLocalizedString("app.tagline", comment: "App tagline")
    }

    enum Nav {
        static let home = NSLocalizedString("nav.home", comment: "Home tab")
        static let services = NSLocalizedString("nav.services", comment: "Services tab")
        static let bookings = NSLocalizedString("nav.bookings", comment: "Bookings tab")
        static let login = NSLocalizedString("nav.login", comment: "Login")
        static let register = NSLocalizedString("nav.register", comment: "Register")
        static let profile = NSLocalizedString("nav.profile", comment: "Profile tab")
        static let logout = NSLocalizedString("nav.logout", comment: "Logout")
    }

    enum Home {
        static let hero = NSLocalizedString("home.hero", comment: "Hero title")
        static let subtitle = NSLocalizedString("home.subtitle", comment: "Hero subtitle")
        static let browseServices = NSLocalizedString("home.browseServices", comment: "Browse services CTA")
        static let getStarted = NSLocalizedString("home.getStarted", comment: "Get started CTA")
        static let categories = NSLocalizedString("home.categories", comment: "Categories section")
        static let howItWorks = NSLocalizedString("home.howItWorks", comment: "How it works section")
        static let step1 = NSLocalizedString("home.step1", comment: "Step 1")
        static let step2 = NSLocalizedString("home.step2", comment: "Step 2")
        static let step3 = NSLocalizedString("home.step3", comment: "Step 3")
    }

    enum Auth {
        static let email = NSLocalizedString("auth.email", comment: "Email field")
        static let password = NSLocalizedString("auth.password", comment: "Password field")
        static let phone = NSLocalizedString("auth.phone", comment: "Phone field")
        static let firstName = NSLocalizedString("auth.firstName", comment: "First name field")
        static let lastName = NSLocalizedString("auth.lastName", comment: "Last name field")
        static let role = NSLocalizedString("auth.role", comment: "Account type")
        static let customer = NSLocalizedString("auth.customer", comment: "Customer role")
        static let craftsman = NSLocalizedString("auth.craftsman", comment: "Craftsman role")
        static let store = NSLocalizedString("auth.store", comment: "Store role")
        static let loginTitle = NSLocalizedString("auth.loginTitle", comment: "Login title")
        static let registerTitle = NSLocalizedString("auth.registerTitle", comment: "Register title")
        static let noAccount = NSLocalizedString("auth.noAccount", comment: "No account prompt")
        static let hasAccount = NSLocalizedString("auth.hasAccount", comment: "Has account prompt")
    }

    enum Services {
        static let title = NSLocalizedString("services.title", comment: "Services title")
        static let request = NSLocalizedString("services.request", comment: "Request service")
        static let price = NSLocalizedString("services.price", comment: "Price label")
        static let duration = NSLocalizedString("services.duration", comment: "Duration label")
        static let allCategories = NSLocalizedString("services.allCategories", comment: "All categories filter")
    }

    enum Profile {
        static let title = NSLocalizedString("profile.title", comment: "Profile title")
        static let accountInfo = NSLocalizedString("profile.accountInfo", comment: "Account info section")
        static let language = NSLocalizedString("profile.language", comment: "Language setting")
        static let english = NSLocalizedString("profile.english", comment: "English language")
        static let arabic = NSLocalizedString("profile.arabic", comment: "Arabic language")
    }

    enum Booking {
        static let title = NSLocalizedString("booking.title", comment: "Booking wizard title")
        static let myBookings = NSLocalizedString("booking.myBookings", comment: "My bookings title")
        static let newBooking = NSLocalizedString("booking.new", comment: "New booking")
        static let stepService = NSLocalizedString("booking.stepService", comment: "Service step")
        static let stepCraftsman = NSLocalizedString("booking.stepCraftsman", comment: "Craftsman step")
        static let stepDateTime = NSLocalizedString("booking.stepDateTime", comment: "Date time step")
        static let stepConfirm = NSLocalizedString("booking.stepConfirm", comment: "Confirm step")
        static let selectDate = NSLocalizedString("booking.selectDate", comment: "Select date")
        static let loadSlots = NSLocalizedString("booking.loadSlots", comment: "Load slots")
        static let summary = NSLocalizedString("booking.summary", comment: "Booking summary")
        static let confirmAndPay = NSLocalizedString("booking.confirmAndPay", comment: "Confirm and pay")
        static let noCraftsmen = NSLocalizedString("booking.noCraftsmen", comment: "No craftsmen")
        static let noSlots = NSLocalizedString("booking.noSlots", comment: "No slots")
    }

    enum Common {
        static let loading = NSLocalizedString("common.loading", comment: "Loading indicator")
        static let error = NSLocalizedString("common.error", comment: "Generic error")
        static let save = NSLocalizedString("common.save", comment: "Save button")
        static let cancel = NSLocalizedString("common.cancel", comment: "Cancel button")
        static let back = NSLocalizedString("common.back", comment: "Back button")
        static let next = NSLocalizedString("common.next", comment: "Next button")
        static let minutes = NSLocalizedString("common.minutes", comment: "Minutes unit")
        static let retry = NSLocalizedString("common.retry", comment: "Retry button")
    }
}
