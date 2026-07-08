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

    enum Health {
        static let check = baseURL.appendingPathComponent("health")
    }
}
