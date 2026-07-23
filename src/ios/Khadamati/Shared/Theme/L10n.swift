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
        static let forgotPassword = NSLocalizedString("auth.forgotPassword", comment: "Forgot password link")
        static let forgotPasswordTitle = NSLocalizedString("auth.forgotPasswordTitle", comment: "Forgot password title")
        static let forgotPasswordHint = NSLocalizedString("auth.forgotPasswordHint", comment: "Forgot password hint")
        static let sendResetLink = NSLocalizedString("auth.sendResetLink", comment: "Send reset link")
        static let resetPasswordTitle = NSLocalizedString("auth.resetPasswordTitle", comment: "Reset password title")
        static let resetToken = NSLocalizedString("auth.resetToken", comment: "Reset token")
        static let newPassword = NSLocalizedString("auth.newPassword", comment: "New password")
        static let confirmPassword = NSLocalizedString("auth.confirmPassword", comment: "Confirm password")
        static let resetPassword = NSLocalizedString("auth.resetPassword", comment: "Reset password button")
        static let passwordMismatch = NSLocalizedString("auth.passwordMismatch", comment: "Password mismatch")
        static let resetSuccess = NSLocalizedString("auth.resetSuccess", comment: "Reset success")
        static let forgotSuccess = NSLocalizedString("auth.forgotSuccess", comment: "Forgot success")
        static let changePassword = NSLocalizedString("auth.changePassword", comment: "Change password")
        static let changePasswordTitle = NSLocalizedString("auth.changePasswordTitle", comment: "Change password title")
        static let changePasswordHint = NSLocalizedString("auth.changePasswordHint", comment: "Change password hint")
        static let currentPassword = NSLocalizedString("auth.currentPassword", comment: "Current password")
        static let changePasswordSuccess = NSLocalizedString("auth.changePasswordSuccess", comment: "Change password success")
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
        static let editTitle = NSLocalizedString("profile.editTitle", comment: "Edit profile title")
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
        static let findNearby = NSLocalizedString("booking.findNearby", comment: "Find nearby")
        static let payNow = NSLocalizedString("booking.payNow", comment: "Pay now")
        static let accept = NSLocalizedString("booking.accept", comment: "Accept booking")
        static let reject = NSLocalizedString("booking.reject", comment: "Reject booking")
        static let complete = NSLocalizedString("booking.complete", comment: "Complete booking")
        static let cancel = NSLocalizedString("booking.cancel", comment: "Cancel booking")
        static let cancelReason = NSLocalizedString("booking.cancelReason", comment: "Cancel reason")
        static let confirmCancel = NSLocalizedString("booking.confirmCancel", comment: "Confirm cancel")
        static let reschedule = NSLocalizedString("booking.reschedule", comment: "Reschedule booking")
        static let rescheduleReason = NSLocalizedString("booking.rescheduleReason", comment: "Reschedule reason")
        static let confirmReschedule = NSLocalizedString("booking.confirmReschedule", comment: "Confirm reschedule")
        static let noShow = NSLocalizedString("booking.noShow", comment: "No show")
        static let noShowHint = NSLocalizedString("booking.noShowHint", comment: "No show hint")
        static let confirmNoShow = NSLocalizedString("booking.confirmNoShow", comment: "Confirm no show")

        static func distanceKm(_ km: Double) -> String {
            String(format: NSLocalizedString("booking.distanceKm", comment: "Distance"), km)
        }
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

    enum Notifications {
        static let title = NSLocalizedString("notifications.title", comment: "Notifications title")
        static let all = NSLocalizedString("notifications.all", comment: "All notifications")
        static let unread = NSLocalizedString("notifications.unread", comment: "Unread notifications")
        static let unreadBadge = NSLocalizedString("notifications.unreadBadge", comment: "Unread badge")
        static let empty = NSLocalizedString("notifications.empty", comment: "Empty notifications")
        static let emptyUnread = NSLocalizedString("notifications.emptyUnread", comment: "Empty unread")
        static let markAllRead = NSLocalizedString("notifications.markAllRead", comment: "Mark all read")
        static let viewBooking = NSLocalizedString("notifications.viewBooking", comment: "View booking link")
    }

    enum Subscription {
        static let nav = NSLocalizedString("subscription.nav", comment: "Subscriptions nav")
        static let plansTitle = NSLocalizedString("subscription.plansTitle", comment: "Plans title")
        static let mySubscription = NSLocalizedString("subscription.mySubscription", comment: "My subscription")
        static let subscribeTitle = NSLocalizedString("subscription.subscribeTitle", comment: "Subscribe title")
        static let choosePlan = NSLocalizedString("subscription.choosePlan", comment: "Choose plan")
        static let alreadySubscribed = NSLocalizedString("subscription.alreadySubscribed", comment: "Already subscribed")
        static let noPlans = NSLocalizedString("subscription.noPlans", comment: "No plans")
        static let billingCycle = NSLocalizedString("subscription.billingCycle", comment: "Billing cycle")
        static let autoRenew = NSLocalizedString("subscription.autoRenew", comment: "Auto renew")
        static let confirmSubscribe = NSLocalizedString("subscription.confirmSubscribe", comment: "Confirm subscribe")
        static let noActive = NSLocalizedString("subscription.noActive", comment: "No active subscription")
        static let browsePlans = NSLocalizedString("subscription.browsePlans", comment: "Browse plans")
        static let startDate = NSLocalizedString("subscription.startDate", comment: "Start date")
        static let endDate = NSLocalizedString("subscription.endDate", comment: "End date")
        static let cancel = NSLocalizedString("subscription.cancel", comment: "Cancel subscription")
        static let cancelTitle = NSLocalizedString("subscription.cancelTitle", comment: "Cancel title")
        static let cancelHint = NSLocalizedString("subscription.cancelHint", comment: "Cancel hint")
        static let cancelReason = NSLocalizedString("subscription.cancelReason", comment: "Cancel reason")
        static let confirmCancel = NSLocalizedString("subscription.confirmCancel", comment: "Confirm cancel")
        static let history = NSLocalizedString("subscription.history", comment: "History")
        static let manage = NSLocalizedString("subscription.manage", comment: "Manage")
        static let featured = NSLocalizedString("subscription.featured", comment: "Featured")
        static let cycleMonthly = NSLocalizedString("subscription.cycle.Monthly", comment: "Monthly")
        static let cycleQuarterly = NSLocalizedString("subscription.cycle.Quarterly", comment: "Quarterly")
        static let cycleSemiAnnual = NSLocalizedString("subscription.cycle.SemiAnnual", comment: "Semi annual")
        static let cycleAnnual = NSLocalizedString("subscription.cycle.Annual", comment: "Annual")
        static let cycleLifetime = NSLocalizedString("subscription.cycle.Lifetime", comment: "Lifetime")

        static func activePlan(plan: String, status: String) -> String {
            String(format: NSLocalizedString("subscription.activePlan", comment: "Active plan"), plan, status)
        }

        static func maxServices(count: Int) -> String {
            String(format: NSLocalizedString("subscription.maxServices", comment: "Max services"), count)
        }
    }

    enum Chat {
        static let title = NSLocalizedString("chat.title", comment: "Chat title")
        static let empty = NSLocalizedString("chat.empty", comment: "Empty chat")
        static let open = NSLocalizedString("chat.open", comment: "Open chat")
        static let messageHint = NSLocalizedString("chat.messageHint", comment: "Message hint")
    }

    enum Addresses {
        static let title = NSLocalizedString("addresses.title", comment: "Addresses title")
        static let add = NSLocalizedString("addresses.add", comment: "Add address")
        static let label = NSLocalizedString("addresses.label", comment: "Label")
        static let street = NSLocalizedString("addresses.street", comment: "Street")
        static let city = NSLocalizedString("addresses.city", comment: "City")
        static let country = NSLocalizedString("addresses.country", comment: "Country")
        static let useLocation = NSLocalizedString("addresses.useLocation", comment: "Use location")
        static let empty = NSLocalizedString("addresses.empty", comment: "Empty addresses")

        static func coordinates(lat: Double, lng: Double) -> String {
            String(format: NSLocalizedString("addresses.coordinates", comment: "Coordinates"), lat, lng)
        }
    }

    enum Support {
        static let title = NSLocalizedString("support.title", comment: "Support title")
        static let submit = NSLocalizedString("support.submit", comment: "Submit tab")
        static let history = NSLocalizedString("support.history", comment: "History tab")
        static let complaint = NSLocalizedString("support.complaint", comment: "Complaint type")
        static let ticket = NSLocalizedString("support.ticket", comment: "Ticket type")
        static let subject = NSLocalizedString("support.subject", comment: "Subject field")
        static let description = NSLocalizedString("support.description", comment: "Description field")
        static let category = NSLocalizedString("support.category", comment: "Category field")
        static let submitComplaint = NSLocalizedString("support.submitComplaint", comment: "Submit complaint")
        static let submitTicket = NSLocalizedString("support.submitTicket", comment: "Submit ticket")
        static let complaintSubmitted = NSLocalizedString("support.complaintSubmitted", comment: "Complaint submitted")
        static let ticketSubmitted = NSLocalizedString("support.ticketSubmitted", comment: "Ticket submitted")
        static let myComplaints = NSLocalizedString("support.myComplaints", comment: "My complaints")
        static let myTickets = NSLocalizedString("support.myTickets", comment: "My tickets")
        static let noComplaints = NSLocalizedString("support.noComplaints", comment: "No complaints")
        static let noTickets = NSLocalizedString("support.noTickets", comment: "No tickets")
    }

    enum Review {
        static let title = NSLocalizedString("review.title", comment: "Review title")
        static let rating = NSLocalizedString("review.rating", comment: "Rating label")
        static let comment = NSLocalizedString("review.comment", comment: "Comment field")
        static let submit = NSLocalizedString("review.submit", comment: "Submit review")
        static let leave = NSLocalizedString("review.leave", comment: "Leave review")
        static let yourReview = NSLocalizedString("review.yourReview", comment: "Your review")
    }

    enum Portal {
        static let craftsmanTitle = NSLocalizedString("portal.craftsmanTitle", comment: "Craftsman portal")
        static let storeTitle = NSLocalizedString("portal.storeTitle", comment: "Store portal")
        static let profile = NSLocalizedString("portal.profile", comment: "Profile section")
        static let saved = NSLocalizedString("portal.saved", comment: "Saved message")
        static let serviceAdded = NSLocalizedString("portal.serviceAdded", comment: "Service added")
        static let productAdded = NSLocalizedString("portal.productAdded", comment: "Product added")
        static let specialization = NSLocalizedString("portal.specialization", comment: "Specialization")
        static let years = NSLocalizedString("portal.years", comment: "Years")
        static let radius = NSLocalizedString("portal.radius", comment: "Radius")
        static let available = NSLocalizedString("portal.available", comment: "Available")
        static let jobs = NSLocalizedString("portal.jobs", comment: "Jobs")
        static let offeredServices = NSLocalizedString("portal.offeredServices", comment: "Offered services")
        static let addService = NSLocalizedString("portal.addService", comment: "Add service")
        static let storeProfile = NSLocalizedString("portal.storeProfile", comment: "Store profile")
        static let storeName = NSLocalizedString("portal.storeName", comment: "Store name")
        static let storeDescription = NSLocalizedString("portal.storeDescription", comment: "Store description")
        static let opens = NSLocalizedString("portal.opens", comment: "Opens")
        static let closes = NSLocalizedString("portal.closes", comment: "Closes")
        static let storeOpen = NSLocalizedString("portal.storeOpen", comment: "Store open")
        static let products = NSLocalizedString("portal.products", comment: "Products")
        static let productNameEn = NSLocalizedString("portal.productNameEn", comment: "Product EN")
        static let productNameAr = NSLocalizedString("portal.productNameAr", comment: "Product AR")
        static let price = NSLocalizedString("portal.price", comment: "Price")
        static let stock = NSLocalizedString("portal.stock", comment: "Stock")
        static let addProduct = NSLocalizedString("portal.addProduct", comment: "Add product")
    }
}
