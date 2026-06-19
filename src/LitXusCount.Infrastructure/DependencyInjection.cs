using LitXusCount.Application.Auth;
using LitXusCount.Application.Settings.Company;
using LitXusCount.Application.Settings.Currencies;
using LitXusCount.Application.Settings.EmailConfigs;
using LitXusCount.Application.Settings.Lookups;
using LitXusCount.Application.Settings.VatPercentages;
using LitXusCount.Application.Admin.Roles;
using LitXusCount.Application.Admin.Users;
using LitXusCount.Infrastructure.Admin.Roles;
using LitXusCount.Infrastructure.Admin.Users;
using LitXusCount.Infrastructure.Authorization;
using LitXusCount.Infrastructure.Identity;
using LitXusCount.Infrastructure.Persistence;
using LitXusCount.Infrastructure.Settings.Company;
using LitXusCount.Infrastructure.Settings.Currencies;
using LitXusCount.Infrastructure.Settings.EmailConfigs;
using LitXusCount.Infrastructure.Settings.Lookups;
using LitXusCount.Infrastructure.Settings.VatPercentages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LitXusCount.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

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
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<IAuthService, AuthService>();

        services.AddDataProtection();
        services.AddScoped<IEmailConfigEncryptor, EmailConfigEncryptor>();

        services.AddScoped<ICompanyInfoService, CompanyInfoService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<IVatPercentageService, VatPercentageService>();
        services.AddScoped<IEmailConfigService, EmailConfigService>();
        services.AddScoped<IPaymentTypeService, PaymentTypeService>();
        services.AddScoped<IPaymentStatusService, PaymentStatusService>();
        services.AddScoped<ICustomerTypeService, CustomerTypeService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IUnitOfMeasureService, UnitOfMeasureService>();

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserManagementService, UserManagementService>();

        return services;
    }
}
