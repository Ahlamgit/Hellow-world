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
    }

    enum Users {
        static let me = baseURL.appendingPathComponent("users/me")
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
    }

    enum Health {
        static let check = baseURL.appendingPathComponent("health")
    }
}
