using LitXusCount.Application.Settings.Lookups;
using LitXusCount.Domain.Entities;
using LitXusCount.Infrastructure.Persistence;

namespace LitXusCount.Infrastructure.Settings.Lookups;

internal sealed class PaymentCodeService(ApplicationDbContext db) : LookupServiceBase<PaymentCode>(db), IPaymentCodeService;

internal sealed class PaymentStatusService(ApplicationDbContext db) : LookupServiceBase<PaymentStatus>(db), IPaymentStatusService;

internal sealed class CustomerTypeService(ApplicationDbContext db) : LookupServiceBase<CustomerType>(db), ICustomerTypeService;

internal sealed class CategoryService(ApplicationDbContext db) : LookupServiceBase<Category>(db), ICategoryService;

internal sealed class UnitOfMeasureService(ApplicationDbContext db) : LookupServiceBase<UnitOfMeasure>(db), IUnitOfMeasureService;
