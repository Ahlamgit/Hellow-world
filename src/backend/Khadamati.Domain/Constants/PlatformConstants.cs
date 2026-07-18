namespace Khadamati.Domain.Constants;

public static class PlatformConstants
{
    public const string DefaultCountryCode = "SA";
    public const string DefaultCurrency = "SAR";
    public const string DefaultTimezone = "Asia/Riyadh";

    public static readonly string[] SupportedCountryCodes = [DefaultCountryCode];
    public static readonly string[] SupportedCurrencies = [DefaultCurrency];

    public static bool IsSupportedCountry(string? countryCode) =>
        !string.IsNullOrWhiteSpace(countryCode) &&
        SupportedCountryCodes.Contains(countryCode.Trim().ToUpperInvariant());

    public static bool IsSupportedCurrency(string? currency) =>
        !string.IsNullOrWhiteSpace(currency) &&
        SupportedCurrencies.Contains(currency.Trim().ToUpperInvariant());
}
