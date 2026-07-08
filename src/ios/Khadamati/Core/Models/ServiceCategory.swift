import Foundation

struct ServiceCategory: Codable, Identifiable, Equatable {
    let id: UUID
    let nameAr: String
    let nameEn: String
    let descriptionAr: String?
    let descriptionEn: String?
    let iconUrl: String?
    let displayOrder: Int
    let subCategories: [ServiceCategory]?

    func localizedName(locale: Locale = .current) -> String {
        locale.language.languageCode?.identifier == "ar" ? nameAr : nameEn
    }

    func localizedDescription(locale: Locale = .current) -> String? {
        locale.language.languageCode?.identifier == "ar" ? descriptionAr : descriptionEn
    }
}
