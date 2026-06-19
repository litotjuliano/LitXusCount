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
  },
} as const;
