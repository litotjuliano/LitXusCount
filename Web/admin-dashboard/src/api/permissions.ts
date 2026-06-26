/** Mirrors LitXusCount.Application.Authorization.Permissions on the backend. */
export const Permissions = {
  Users: {
    View: "Users.View",
    Create: "Users.Create",
    Edit: "Users.Edit",
    Delete: "Users.Delete",
  },
  Roles: {
    View: "Roles.View",
    Create: "Roles.Create",
    Edit: "Roles.Edit",
    Delete: "Roles.Delete",
  },
  Settings: {
    CompanyInfo: { View: "Settings.CompanyInfo.View", Edit: "Settings.CompanyInfo.Edit" },
    Currency: {
      View: "Settings.Currency.View",
      Create: "Settings.Currency.Create",
      Edit: "Settings.Currency.Edit",
      Delete: "Settings.Currency.Delete",
    },
    VatPercentage: {
      View: "Settings.VatPercentage.View",
      Create: "Settings.VatPercentage.Create",
      Edit: "Settings.VatPercentage.Edit",
      Delete: "Settings.VatPercentage.Delete",
    },
    EmailConfig: {
      View: "Settings.EmailConfig.View",
      Create: "Settings.EmailConfig.Create",
      Edit: "Settings.EmailConfig.Edit",
      Delete: "Settings.EmailConfig.Delete",
    },
    PaymentType: {
      View: "Settings.PaymentType.View",
      Create: "Settings.PaymentType.Create",
      Edit: "Settings.PaymentType.Edit",
      Delete: "Settings.PaymentType.Delete",
    },
    PaymentStatus: {
      View: "Settings.PaymentStatus.View",
      Create: "Settings.PaymentStatus.Create",
      Edit: "Settings.PaymentStatus.Edit",
      Delete: "Settings.PaymentStatus.Delete",
    },
    CustomerType: {
      View: "Settings.CustomerType.View",
      Create: "Settings.CustomerType.Create",
      Edit: "Settings.CustomerType.Edit",
      Delete: "Settings.CustomerType.Delete",
    },
    Category: {
      View: "Settings.Category.View",
      Create: "Settings.Category.Create",
      Edit: "Settings.Category.Edit",
      Delete: "Settings.Category.Delete",
    },
    UnitOfMeasure: {
      View: "Settings.UnitOfMeasure.View",
      Create: "Settings.UnitOfMeasure.Create",
      Edit: "Settings.UnitOfMeasure.Edit",
      Delete: "Settings.UnitOfMeasure.Delete",
    },
    GlAccount: {
      View: "Settings.GlAccount.View",
      Create: "Settings.GlAccount.Create",
      Edit: "Settings.GlAccount.Edit",
      Delete: "Settings.GlAccount.Delete",
    },
    Customer: {
      View: "Settings.Customer.View",
      Create: "Settings.Customer.Create",
      Edit: "Settings.Customer.Edit",
      Delete: "Settings.Customer.Delete",
    },
    Supplier: {
      View: "Settings.Supplier.View",
      Create: "Settings.Supplier.Create",
      Edit: "Settings.Supplier.Edit",
      Delete: "Settings.Supplier.Delete",
    },
    Product: {
      View: "Settings.Product.View",
      Create: "Settings.Product.Create",
      Edit: "Settings.Product.Edit",
      Delete: "Settings.Product.Delete",
    },
  },
  Accounts: {
    Account: {
      View: "Accounts.Account.View",
      Create: "Accounts.Account.Create",
      Edit: "Accounts.Account.Edit",
      Delete: "Accounts.Account.Delete",
    },
    Deposit: {
      View: "Accounts.Deposit.View",
      Create: "Accounts.Deposit.Create",
      Delete: "Accounts.Deposit.Delete",
    },
    Expense: {
      View: "Accounts.Expense.View",
      Create: "Accounts.Expense.Create",
      Delete: "Accounts.Expense.Delete",
    },
    Transfer: {
      View: "Accounts.Transfer.View",
      Create: "Accounts.Transfer.Create",
      Delete: "Accounts.Transfer.Delete",
    },
  },
  License: {
    View: "License.View",
    Manage: "License.Manage",
  },
  Tenants: {
    View: "Tenants.View",
    Create: "Tenants.Create",
    Edit: "Tenants.Edit",
    Delete: "Tenants.Delete",
  },
  Sales: {
    Invoice: {
      View: "Sales.Invoice.View",
      Create: "Sales.Invoice.Create",
      Edit: "Sales.Invoice.Edit",
      Delete: "Sales.Invoice.Delete",
      Manage: "Sales.Invoice.Manage",
    },
  },
} as const;
