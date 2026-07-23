import Foundation

struct Service: Codable, Identifiable, Equatable {
    let id: UUID
    let categoryId: UUID
    let nameAr: String
    let nameEn: String
    let descriptionAr: String?
    let descriptionEn: String?
    let basePrice: Decimal
    let imageUrl: String?
    let estimatedDurationMinutes: Int

    func localizedName(locale: Locale = .current) -> String {
        locale.language.languageCode?.identifier == "ar" ? nameAr : nameEn
    }

    func localizedDescription(locale: Locale = .current) -> String? {
        locale.language.languageCode?.identifier == "ar" ? descriptionAr : descriptionEn
    }

    var formattedPrice: String {
        let formatter = NumberFormatter()
        formatter.numberStyle = .currency
        formatter.currencyCode = "USD"
        formatter.locale = Locale(identifier: "en_LB")
        return formatter.string(from: basePrice as NSDecimalNumber) ?? "\(basePrice)"
    }
}
