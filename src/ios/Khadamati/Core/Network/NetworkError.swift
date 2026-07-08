import Foundation

enum NetworkError: LocalizedError {
    case invalidURL
    case invalidResponse
    case unauthorized
    case serverError(statusCode: Int, message: String?)
    case decodingFailed(Error)
    case encodingFailed
    case noData
    case refreshFailed

    var errorDescription: String? {
        switch self {
        case .invalidURL:
            return "Invalid URL."
        case .invalidResponse:
            return L10n.Common.error
        case .unauthorized:
            return "Session expired. Please sign in again."
        case .serverError(_, let message):
            return message ?? L10n.Common.error
        case .decodingFailed:
            return "Failed to parse server response."
        case .encodingFailed:
            return "Failed to encode request."
        case .noData:
            return "No data received from server."
        case .refreshFailed:
            return "Unable to refresh session."
        }
    }
}
