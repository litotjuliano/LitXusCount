using LitXusCount.Application.Authorization;
using LitXusCount.Application.Settings.Lookups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LitXusCount.API.Controllers.Settings;


[Route("api/settings/payment-statuses")]
public class PaymentStatusesController(IPaymentStatusService service, IAuthorizationService authorizationService)
    : LookupControllerBase<IPaymentStatusService>(service, authorizationService)
{
    protected override string ResourceName => nameof(Permissions.Settings.PaymentStatus);
}

[Route("api/settings/customer-types")]
public class CustomerTypesController(ICustomerTypeService service, IAuthorizationService authorizationService)
    : LookupControllerBase<ICustomerTypeService>(service, authorizationService)
{
    protected override string ResourceName => nameof(Permissions.Settings.CustomerType);
}

[Route("api/settings/categories")]
public class CategoriesController(ICategoryService service, IAuthorizationService authorizationService)
    : LookupControllerBase<ICategoryService>(service, authorizationService)
{
    protected override string ResourceName => nameof(Permissions.Settings.Category);
}

