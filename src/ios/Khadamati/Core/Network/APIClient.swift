import Foundation

actor APIClient {
    static let shared = APIClient()

    private let session: URLSession
    private let tokenStorage: TokenStorageProtocol
    private let decoder: JSONDecoder
    private let encoder: JSONEncoder
    private var isRefreshing = false
    private var refreshContinuations: [CheckedContinuation<Void, Error>] = []

    init(
        session: URLSession = .shared,
        tokenStorage: TokenStorageProtocol = TokenStorage.shared
    ) {
        self.session = session
        self.tokenStorage = tokenStorage

        decoder = JSONDecoder()
        decoder.keyDecodingStrategy = .convertFromSnakeCase
        decoder.dateDecodingStrategy = .custom { decoder in
            let container = try decoder.singleValueContainer()
            let value = try container.decode(String.self)

            let formatters = [
                ISO8601DateFormatter(),
                {
                    let formatter = ISO8601DateFormatter()
                    formatter.formatOptions = [.withInternetDateTime, .withFractionalSeconds]
                    return formatter
                }()
            ]

            for formatter in formatters {
                if let date = formatter.date(from: value) {
                    return date
                }
            }

            throw DecodingError.dataCorruptedError(
                in: container,
                debugDescription: "Invalid date format: \(value)"
            )
        }

        encoder = JSONEncoder()
        encoder.keyEncodingStrategy = .convertToSnakeCase
    }

    // MARK: - Public API

    func request<T: Decodable>(
        url: URL,
        method: HTTPMethod = .get,
        body: (any Encodable)? = nil,
        requiresAuth: Bool = false,
        retryOnUnauthorized: Bool = true
    ) async throws -> T {
        try await performRequest(
            url: url,
            method: method,
            body: body,
            requiresAuth: requiresAuth,
            retryOnUnauthorized: retryOnUnauthorized
        )
    }

    func requestVoid(
        url: URL,
        method: HTTPMethod = .post,
        body: (any Encodable)? = nil,
        requiresAuth: Bool = true
    ) async throws {
        let _: ApiResponse<EmptyResponse> = try await request(
            url: url,
            method: method,
            body: body,
            requiresAuth: requiresAuth,
            retryOnUnauthorized: true
        )
    }

    // MARK: - Private

    private func performRequest<T: Decodable>(
        url: URL,
        method: HTTPMethod,
        body: (any Encodable)?,
        requiresAuth: Bool,
        retryOnUnauthorized: Bool
    ) async throws -> T {
        var request = URLRequest(url: url)
        request.httpMethod = method.rawValue
        request.setValue("application/json", forHTTPHeaderField: "Accept")
        request.setValue("application/json", forHTTPHeaderField: "Content-Type")

        if requiresAuth, let token = tokenStorage.getAccessToken() {
            request.setValue("Bearer \(token)", forHTTPHeaderField: "Authorization")
        }

        if let body {
            do {
                request.httpBody = try encoder.encode(AnyEncodable(body))
            } catch {
                throw NetworkError.encodingFailed
            }
        }

        let (data, response) = try await session.data(for: request)

        guard let httpResponse = response as? HTTPURLResponse else {
            throw NetworkError.invalidResponse
        }

        if httpResponse.statusCode == 401, requiresAuth, retryOnUnauthorized {
            try await refreshTokensIfNeeded()
            return try await performRequest(
                url: url,
                method: method,
                body: body,
                requiresAuth: requiresAuth,
                retryOnUnauthorized: false
            )
        }

        guard (200...299).contains(httpResponse.statusCode) else {
            let message = parseErrorMessage(from: data)
            if httpResponse.statusCode == 401 {
                throw NetworkError.unauthorized
            }
            throw NetworkError.serverError(statusCode: httpResponse.statusCode, message: message)
        }

        do {
            return try decoder.decode(T.self, from: data)
        } catch {
            throw NetworkError.decodingFailed(error)
        }
    }

    private func refreshTokensIfNeeded() async throws {
        if isRefreshing {
            try await withCheckedThrowingContinuation { continuation in
                refreshContinuations.append(continuation)
            }
            return
        }

        isRefreshing = true
        defer {
            isRefreshing = false
            let continuations = refreshContinuations
            refreshContinuations.removeAll()
            continuations.forEach { $0.resume() }
        }

        guard let accessToken = tokenStorage.getAccessToken(),
              let refreshToken = tokenStorage.getRefreshToken() else {
            try? tokenStorage.clear()
            throw NetworkError.unauthorized
        }

        var request = URLRequest(url: APIEndpoints.Auth.refresh)
        request.httpMethod = HTTPMethod.post.rawValue
        request.setValue("application/json", forHTTPHeaderField: "Content-Type")
        request.setValue("application/json", forHTTPHeaderField: "Accept")

        let payload = RefreshTokenRequest(accessToken: accessToken, refreshToken: refreshToken)
        request.httpBody = try encoder.encode(payload)

        let (data, response) = try await session.data(for: request)

        guard let httpResponse = response as? HTTPURLResponse,
              (200...299).contains(httpResponse.statusCode) else {
            try? tokenStorage.clear()
            throw NetworkError.refreshFailed
        }

        let authResponse = try decoder.decode(ApiResponse<AuthResponse>.self, from: data)
        try tokenStorage.save(
            accessToken: authResponse.data.accessToken,
            refreshToken: authResponse.data.refreshToken
        )
    }

    private func parseErrorMessage(from data: Data) -> String? {
        struct ErrorEnvelope: Decodable {
            let message: String?
            let errors: [String]?
        }

        guard let envelope = try? decoder.decode(ErrorEnvelope.self, from: data) else {
            return nil
        }

        if let errors = envelope.errors, !errors.isEmpty {
            return errors.joined(separator: "\n")
        }

        return envelope.message
    }
}

// MARK: - Supporting Types

enum HTTPMethod: String {
    case get = "GET"
    case post = "POST"
    case put = "PUT"
    case delete = "DELETE"
}

private struct AnyEncodable: Encodable {
    private let encodeClosure: (Encoder) throws -> Void

    init(_ wrapped: any Encodable) {
        encodeClosure = wrapped.encode
    }

    func encode(to encoder: Encoder) throws {
        try encodeClosure(encoder)
    }
}
