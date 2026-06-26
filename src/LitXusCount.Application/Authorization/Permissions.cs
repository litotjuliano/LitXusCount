using System.Reflection;

namespace LitXusCount.Application.Authorization;

/// <summary>
/// Compile-time catalog of every permission in the system. The set of available permissions
/// ships with code; which permissions a role has is the only thing that's actually
/// database-configurable (via Identity role claims of type <see cref="ClaimType"/>).
/// </summary>
public static class Permissions
{
    public const string ClaimType = "permission";

    public static class Users
    {
        public const string View = "Users.View";
        public const string Create = "Users.Create";
        public const string Edit = "Users.Edit";
        public const string Delete = "Users.Delete";
    }

    public static class Roles
    {
        public const string View = "Roles.View";
        public const string Create = "Roles.Create";
        public const string Edit = "Roles.Edit";
        public const string Delete = "Roles.Delete";
    }

    public static class Settings
    {
        public static class CompanyInfo
        {
            public const string View = "Settings.CompanyInfo.View";
            public const string Edit = "Settings.CompanyInfo.Edit";
        }

        public static class Currency
        {
            public const string View = "Settings.Currency.View";
            public const string Create = "Settings.Currency.Create";
            public const string Edit = "Settings.Currency.Edit";
            public const string Delete = "Settings.Currency.Delete";
        }

        public static class VatPercentage
        {
            public const string View = "Settings.VatPercentage.View";
            public const string Create = "Settings.VatPercentage.Create";
            public const string Edit = "Settings.VatPercentage.Edit";
            public const string Delete = "Settings.VatPercentage.Delete";
        }

        public static class EmailConfig
        {
            public const string View = "Settings.EmailConfig.View";
            public const string Create = "Settings.EmailConfig.Create";
            public const string Edit = "Settings.EmailConfig.Edit";
            public const string Delete = "Settings.EmailConfig.Delete";
        }

        public static class PaymentType
        {
            public const string View = "Settings.PaymentType.View";
            public const string Create = "Settings.PaymentType.Create";
            public const string Edit = "Settings.PaymentType.Edit";
            public const string Delete = "Settings.PaymentType.Delete";
        }

        public static class PaymentStatus
        {
            public const string View = "Settings.PaymentStatus.View";
            public const string Create = "Settings.PaymentStatus.Create";
            public const string Edit = "Settings.PaymentStatus.Edit";
            public const string Delete = "Settings.PaymentStatus.Delete";
        }

        public static class CustomerType
        {
            public const string View = "Settings.CustomerType.View";
            public const string Create = "Settings.CustomerType.Create";
            public const string Edit = "Settings.CustomerType.Edit";
            public const string Delete = "Settings.CustomerType.Delete";
        }

        public static class Category
        {
            public const string View = "Settings.Category.View";
            public const string Create = "Settings.Category.Create";
            public const string Edit = "Settings.Category.Edit";
            public const string Delete = "Settings.Category.Delete";
        }

        public static class UnitOfMeasure
        {
            public const string View = "Settings.UnitOfMeasure.View";
            public const string Create = "Settings.UnitOfMeasure.Create";
            public const string Edit = "Settings.UnitOfMeasure.Edit";
            public const string Delete = "Settings.UnitOfMeasure.Delete";
        }

        public static class GlAccount
        {
            public const string View = "Settings.GlAccount.View";
            public const string Create = "Settings.GlAccount.Create";
            public const string Edit = "Settings.GlAccount.Edit";
            public const string Delete = "Settings.GlAccount.Delete";
        }

        public static class Customer
        {
            public const string View = "Settings.Customer.View";
            public const string Create = "Settings.Customer.Create";
            public const string Edit = "Settings.Customer.Edit";
            public const string Delete = "Settings.Customer.Delete";
        }

        public static class Supplier
        {
            public const string View = "Settings.Supplier.View";
            public const string Create = "Settings.Supplier.Create";
            public const string Edit = "Settings.Supplier.Edit";
            public const string Delete = "Settings.Supplier.Delete";
        }

        public static class Product
        {
            public const string View = "Settings.Product.View";
            public const string Create = "Settings.Product.Create";
            public const string Edit = "Settings.Product.Edit";
            public const string Delete = "Settings.Product.Delete";
        }
    }

    public static class License
    {
        public const string View   = "License.View";
        public const string Manage = "License.Manage";
    }

    public static class Tenants
    {
        public const string View   = "Tenants.View";
        public const string Create = "Tenants.Create";
        public const string Edit   = "Tenants.Edit";
        public const string Delete = "Tenants.Delete";
    }

    public static class Sales
    {
        public static class Invoice
        {
            public const string View   = "Sales.Invoice.View";
            public const string Create = "Sales.Invoice.Create";
            public const string Edit   = "Sales.Invoice.Edit";
            public const string Delete = "Sales.Invoice.Delete";
            public const string Manage = "Sales.Invoice.Manage";
        }
    }

    public static class Accounts
    {
        public static class Account
        {
            public const string View   = "Accounts.Account.View";
            public const string Create = "Accounts.Account.Create";
            public const string Edit   = "Accounts.Account.Edit";
            public const string Delete = "Accounts.Account.Delete";
        }

        public static class Deposit
        {
            public const string View   = "Accounts.Deposit.View";
            public const string Create = "Accounts.Deposit.Create";
            public const string Delete = "Accounts.Deposit.Delete";
        }

        public static class Expense
        {
            public const string View   = "Accounts.Expense.View";
            public const string Create = "Accounts.Expense.Create";
            public const string Delete = "Accounts.Expense.Delete";
        }

        public static class Transfer
        {
            public const string View   = "Accounts.Transfer.View";
            public const string Create = "Accounts.Transfer.Create";
            public const string Delete = "Accounts.Transfer.Delete";
        }
    }

    /// <summary>
    /// Every permission value in the catalog, discovered by recursively walking nested classes.
    /// Starts from the nested classes (Users, Roles, Settings...), not from <see cref="Permissions"/>
    /// itself, since <see cref="ClaimType"/> is also a public const string field here but is not a permission.
    /// </summary>
    public static IReadOnlyList<string> All { get; } = typeof(Permissions)
        .GetNestedTypes(BindingFlags.Public)
        .SelectMany(Collect)
        .ToList();

    private static IReadOnlyList<string> Collect(Type type)
    {
        var values = new List<string>();

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.FieldType == typeof(string) && field.IsLiteral)
            {
                values.Add((string)field.GetRawConstantValue()!);
            }
        }

        foreach (var nested in type.GetNestedTypes(BindingFlags.Public))
        {
            values.AddRange(Collect(nested));
        }

        return values;
    }
}
