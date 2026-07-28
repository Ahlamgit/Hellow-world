import Foundation

enum APIEndpoints {
    static let baseURL: URL = {
        #if DEBUG
        return URL(string: ProcessInfo.processInfo.environment["KHADAMATI_API_URL"]
            ?? "http://localhost:5000/api/v1")!
        #else
        return URL(string: "https://api.khadamati.com/api/v1")!
        #endif
    }()

    enum Auth {
        static let login = baseURL.appendingPathComponent("auth/login")
        static let register = baseURL.appendingPathComponent("auth/register")
        static let refresh = baseURL.appendingPathComponent("auth/refresh")
        static let revoke = baseURL.appendingPathComponent("auth/revoke")
        static let forgotPassword = baseURL.appendingPathComponent("auth/forgot-password")
        static let resetPassword = baseURL.appendingPathComponent("auth/reset-password")
        static let changePassword = baseURL.appendingPathComponent("auth/change-password")
    }

    enum Devices {
        static let pushToken = baseURL.appendingPathComponent("devices/push-token")
    }

    enum Users {
        static let me = baseURL.appendingPathComponent("users/me")
        static let addresses = baseURL.appendingPathComponent("users/me/addresses")
    }

    enum Support {
        static let complaints = baseURL.appendingPathComponent("complaints")
        static let myComplaints = baseURL.appendingPathComponent("complaints/mine")
        static let tickets = baseURL.appendingPathComponent("support-tickets")
        static let myTickets = baseURL.appendingPathComponent("support-tickets/mine")

        static func review(_ bookingId: UUID) -> URL {
            baseURL.appendingPathComponent("bookings/\(bookingId.uuidString)/review")
        }
    }

    enum Services {
        static let categories = baseURL.appendingPathComponent("services/categories")

        static func list(categoryId: UUID? = nil) -> URL {
            var components = URLComponents(url: baseURL.appendingPathComponent("services"), resolvingAgainstBaseURL: false)!
            if let categoryId {
                components.queryItems = [URLQueryItem(name: "categoryId", value: categoryId.uuidString)]
            }
            return components.url!
        }
    }

    enum Bookings {
        static let list = baseURL.appendingPathComponent("bookings")
        static let create = baseURL.appendingPathComponent("bookings")

        static func craftsmen(serviceId: UUID) -> URL {
            var components = URLComponents(url: baseURL.appendingPathComponent("bookings/craftsmen"), resolvingAgainstBaseURL: false)!
            components.queryItems = [URLQueryItem(name: "serviceId", value: serviceId.uuidString)]
            return components.url!
        }

        static func nearbyCraftsmen(serviceId: UUID, latitude: Double, longitude: Double, radiusKm: Double = 25) -> URL {
            var components = URLComponents(url: baseURL.appendingPathComponent("bookings/craftsmen/nearby"), resolvingAgainstBaseURL: false)!
            components.queryItems = [
                URLQueryItem(name: "serviceId", value: serviceId.uuidString),
                URLQueryItem(name: "latitude", value: String(latitude)),
                URLQueryItem(name: "longitude", value: String(longitude)),
                URLQueryItem(name: "radiusKm", value: String(radiusKm)),
            ]
            return components.url!
        }

        static func availability(craftsmanId: UUID, serviceId: UUID, date: String) -> URL {
            var components = URLComponents(url: baseURL.appendingPathComponent("bookings/availability"), resolvingAgainstBaseURL: false)!
            components.queryItems = [
                URLQueryItem(name: "craftsmanId", value: craftsmanId.uuidString),
                URLQueryItem(name: "serviceId", value: serviceId.uuidString),
                URLQueryItem(name: "date", value: date),
            ]
            return components.url!
        }

        static func detail(_ id: UUID) -> URL { baseURL.appendingPathComponent("bookings/\(id.uuidString)") }
        static func confirm(_ id: UUID) -> URL { baseURL.appendingPathComponent("bookings/\(id.uuidString)/confirm") }
        static func payment(_ id: UUID) -> URL { baseURL.appendingPathComponent("bookings/\(id.uuidString)/payment") }
        static func authorizePayment(_ id: UUID) -> URL { baseURL.appendingPathComponent("bookings/\(id.uuidString)/payment/authorize") }
        static func confirmPayment(_ id: UUID) -> URL { baseURL.appendingPathComponent("bookings/\(id.uuidString)/payment/confirm") }
        static func accept(_ id: UUID) -> URL { baseURL.appendingPathComponent("bookings/\(id.uuidString)/accept") }
        static func reject(_ id: UUID) -> URL { baseURL.appendingPathComponent("bookings/\(id.uuidString)/reject") }
        static func cancel(_ id: UUID) -> URL { baseURL.appendingPathComponent("bookings/\(id.uuidString)/cancel") }
        static func complete(_ id: UUID) -> URL { baseURL.appendingPathComponent("bookings/\(id.uuidString)/complete") }
        static func reschedule(_ id: UUID) -> URL { baseURL.appendingPathComponent("bookings/\(id.uuidString)/reschedule") }
        static func noShow(_ id: UUID) -> URL { baseURL.appendingPathComponent("bookings/\(id.uuidString)/no-show") }
    }

    enum Notifications {
        static let list = baseURL.appendingPathComponent("notifications")

        static func markRead(_ id: UUID) -> URL {
            baseURL.appendingPathComponent("notifications/\(id.uuidString)/read")
        }

        static func listURL(unreadOnly: Bool = false, page: Int = 1, pageSize: Int = 15) -> URL {
            var components = URLComponents(url: list, resolvingAgainstBaseURL: false)!
            components.queryItems = [
                URLQueryItem(name: "unreadOnly", value: unreadOnly ? "true" : "false"),
                URLQueryItem(name: "page", value: String(page)),
                URLQueryItem(name: "pageSize", value: String(pageSize)),
            ]
            return components.url!
        }
    }

    enum SubscriptionPlans {
        static let list = baseURL.appendingPathComponent("subscription-plans")

        static func list(targetRole: String? = nil) -> URL {
            guard let targetRole else { return list }
            var components = URLComponents(url: list, resolvingAgainstBaseURL: false)!
            components.queryItems = [URLQueryItem(name: "targetRole", value: targetRole)]
            return components.url!
        }

        static func detail(_ id: UUID) -> URL {
            baseURL.appendingPathComponent("subscription-plans/\(id.uuidString)")
        }
    }

    enum MeSubscription {
        static let current = baseURL.appendingPathComponent("me/subscription")
        static let subscribe = current
        static let autoRenew = baseURL.appendingPathComponent("me/subscription/auto-renew")

        static func history(page: Int = 1, pageSize: Int = 20) -> URL {
            var components = URLComponents(url: baseURL.appendingPathComponent("me/subscriptions"), resolvingAgainstBaseURL: false)!
            components.queryItems = [
                URLQueryItem(name: "page", value: String(page)),
                URLQueryItem(name: "pageSize", value: String(pageSize)),
            ]
            return components.url!
        }

        static func cancel(_ id: UUID) -> URL {
            baseURL.appendingPathComponent("me/subscription/\(id.uuidString)/cancel")
        }
    }

    enum Chat {
        static let conversations = baseURL.appendingPathComponent("chat/conversations")

        static func booking(_ bookingId: UUID) -> URL {
            baseURL.appendingPathComponent("chat/bookings/\(bookingId.uuidString)")
        }

        static func messages(_ conversationId: UUID, page: Int = 1, pageSize: Int = 50) -> URL {
            var components = URLComponents(
                url: baseURL.appendingPathComponent("chat/conversations/\(conversationId.uuidString)/messages"),
                resolvingAgainstBaseURL: false
            )!
            components.queryItems = [
                URLQueryItem(name: "page", value: String(page)),
                URLQueryItem(name: "pageSize", value: String(pageSize)),
            ]
            return components.url!
        }

        static func sendMessage(_ conversationId: UUID) -> URL {
            baseURL.appendingPathComponent("chat/conversations/\(conversationId.uuidString)/messages")
        }
    }

    enum Health {
        static let check = baseURL.appendingPathComponent("health")
    }

    enum CraftsmanPortal {
        static let profile = baseURL.appendingPathComponent("me/craftsman")
        static let services = baseURL.appendingPathComponent("me/craftsman/services")
    }

    enum StorePortal {
        static let profile = baseURL.appendingPathComponent("me/store")
        static let products = baseURL.appendingPathComponent("me/store/products")
    }
}
