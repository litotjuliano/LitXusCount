using LitXusCount.Application.Accounts;
using LitXusCount.Application.Auth;
using LitXusCount.Application.Tenants;
using LitXusCount.Application.Sales;
using LitXusCount.Application.Settings.Company;
using LitXusCount.Application.Settings.Currencies;
using LitXusCount.Application.Settings.EmailConfigs;
using LitXusCount.Application.Settings.Lookups;
using LitXusCount.Application.Settings.VatPercentages;
using LitXusCount.Application.Settings.GlAccounts;
using LitXusCount.Application.Settings.Customers;
using LitXusCount.Application.Settings.Suppliers;
using LitXusCount.Application.Settings.Products;
using LitXusCount.Application.Admin.Roles;
using LitXusCount.Application.Admin.Users;
using LitXusCount.Infrastructure.Accounts;
using LitXusCount.Infrastructure.Sales;
using LitXusCount.Infrastructure.Admin.Roles;
using LitXusCount.Infrastructure.Admin.Users;
using LitXusCount.Infrastructure.Authorization;
using LitXusCount.Infrastructure.Tenants;
using LitXusCount.Infrastructure.Identity;
using LitXusCount.Infrastructure.Persistence;
using LitXusCount.Infrastructure.Settings.Company;
using LitXusCount.Infrastructure.Settings.Currencies;
using LitXusCount.Infrastructure.Settings.EmailConfigs;
using LitXusCount.Infrastructure.Settings.Lookups;
using LitXusCount.Infrastructure.Settings.VatPercentages;
using LitXusCount.Infrastructure.Settings.GlAccounts;
using LitXusCount.Infrastructure.Settings.Customers;
using LitXusCount.Infrastructure.Settings.Suppliers;
using LitXusCount.Infrastructure.Settings.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LitXusCount.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // ── Master Database (Identity + Tenant registry) ──────────────────────
        services.AddDbContext<MasterDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                   .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

        // ── Tenant Connection Cache (singleton — used to invalidate cached CS) ──
        services.AddMemoryCache();
        services.AddSingleton<ITenantConnectionCache, TenantConnectionCache>();

        // ── Tenant Context (scoped per request, resolves from JWT + MasterDbContext) ─
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, TenantContext>();

        // ── Tenant-Scoped Operational Database ────────────────────────────────
        // Each request resolves the ApplicationDbContext using the tenant's
        // connection string from ITenantContext. An empty options object is used
        // when no tenant is available (SuperAdmin platform-level requests).
        services.AddScoped<ApplicationDbContext>(sp =>
        {
            var tenantCtx = sp.GetRequiredService<ITenantContext>();
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            if (tenantCtx.ConnectionString is { } cs)
                optionsBuilder.UseNpgsql(cs);
            else
                // No tenant resolved — do not fall back to the master DB.
                // Controllers will receive a 400/403 before hitting the DB;
                // if somehow a service is called without a tenant context it
                // will fail fast here rather than silently querying wrong data.
                throw new InvalidOperationException(
                    "No tenant connection string is available for this request. " +
                    "Ensure the request carries a valid JWT with a tenant_id claim " +
                    "and that the tenant is active.");

            return new ApplicationDbContext(optionsBuilder.Options);
        });

        // ── Identity (backed by MasterDbContext) ──────────────────────────────
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<MasterDbContext>()
            .AddDefaultTokenProviders();

        // ── Auth ──────────────────────────────────────────────────────────────
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<IAuthService, AuthService>();

        // ── Tenant Management ─────────────────────────────────────────────────
        services.AddScoped<ITenantService, TenantService>();
        services.Configure<DeploymentOptions>(configuration.GetSection(DeploymentOptions.SectionName));

        // ── Data Protection / Encryption ──────────────────────────────────────
        services.AddDataProtection();
        services.AddScoped<IEmailConfigEncryptor, EmailConfigEncryptor>();

        // ── Settings Services ─────────────────────────────────────────────────
        services.AddScoped<ICompanyInfoService, CompanyInfoService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<IVatPercentageService, VatPercentageService>();
        services.AddScoped<IEmailConfigService, EmailConfigService>();
        services.AddScoped<IPaymentCodeService, PaymentCodeService>();
        services.AddScoped<IPaymentStatusService, PaymentStatusService>();
        services.AddScoped<ICustomerTypeService, CustomerTypeService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IUnitOfMeasureService, UnitOfMeasureService>();
        services.AddScoped<IGlAccountService, GlAccountService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IProductService, ProductService>();

        // ── Authorization Policies ────────────────────────────────────────────
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        // ── Accounting & Sales ────────────────────────────────────────────────
        services.AddScoped<IAccAccountService, AccAccountService>();
        services.AddScoped<IAccDepositService, AccDepositService>();
        services.AddScoped<IAccExpenseService, AccExpenseService>();
        services.AddScoped<IAccTransferService, AccTransferService>();

        services.AddScoped<IStockService, StockService>();
        services.AddScoped<ISalesInvoiceService, SalesInvoiceService>();
        services.AddScoped<ISalesInvoiceLineService, SalesInvoiceLineService>();
        services.AddScoped<ISalesPaymentService, SalesPaymentService>();

        // ── Admin ─────────────────────────────────────────────────────────────
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserManagementService, UserManagementService>();

        return services;
    }
}
