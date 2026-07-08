namespace Khadamati.Domain.Constants;

public static class RoleNames
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string SupportAgent = "SupportAgent";
    public const string Moderator = "Moderator";
    public const string Customer = "Customer";
    public const string Craftsman = "Craftsman";
    public const string StoreOwner = "StoreOwner";
    public const string StoreEmployee = "StoreEmployee";
    public const string Accountant = "Accountant";

    public static readonly string[] All =
    [
        SuperAdmin, Admin, SupportAgent, Moderator, Customer,
        Craftsman, StoreOwner, StoreEmployee, Accountant
    ];

    public static readonly string[] SelfRegistrationRoles =
        [Customer, Craftsman, StoreOwner];

    /// <summary>Maps legacy enum names to current role names.</summary>
    public static string MapLegacyRole(string legacyRole) => legacyRole switch
    {
        "Administrator" => Admin,
        "Store" => StoreOwner,
        _ => legacyRole
    };
}
