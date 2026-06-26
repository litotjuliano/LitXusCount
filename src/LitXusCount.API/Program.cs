using System.Security.Claims;
using System.Text;
using LitXusCount.Application.Auth;
using LitXusCount.Application.Authorization;
using LitXusCount.Application.Settings.EmailConfigs;
using LitXusCount.Domain.Entities;
using LitXusCount.Infrastructure;
using LitXusCount.Infrastructure.Identity;
using LitXusCount.Infrastructure.Persistence;
using LitXusCount.Infrastructure.Persistence.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "FrontendCorsPolicy";

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LitXusCount API", Version = "v1" });
    c.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("BearerAuth"), new List<string>() }
    });
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<MasterDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LitXusCount API v1"));
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors(FrontendCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");


// ── Database Migrations ───────────────────────────────────────────────────────
using (var migrationScope = app.Services.CreateScope())
{
    var masterDb = migrationScope.ServiceProvider.GetRequiredService<MasterDbContext>();
    await masterDb.Database.MigrateAsync();

    // Ensure the default tenant exists (first run or fresh database)
    if (!await masterDb.Tenants.AnyAsync())
    {
        var defaultConnectionString = BuildTenantConnectionString(
            masterDb.Database.GetConnectionString()!, "default");
        masterDb.Tenants.Add(new Tenant
        {
            Name = "Default Company",
            Slug = "default",
            ConnectionString = defaultConnectionString,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        });
        await masterDb.SaveChangesAsync();
    }

    // Run ApplicationDbContext migrations for all active tenant databases
    var tenants = await masterDb.Tenants.Where(t => t.IsActive).ToListAsync();
    foreach (var tenant in tenants)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(tenant.ConnectionString)
            .Options;
        await using var tenantDb = new ApplicationDbContext(options);
        await tenantDb.Database.MigrateAsync();
    }
}

// ── Dev / Demo Seeding ────────────────────────────────────────────────────────
var seedDemoData = app.Environment.IsDevelopment() ||
    string.Equals(Environment.GetEnvironmentVariable("SEED_DEMO_DATA"), "true", StringComparison.OrdinalIgnoreCase);

if (seedDemoData)
{
    using var scope = app.Services.CreateScope();
    var masterDb = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var roleName in new[] { RoleNames.SuperAdmin, RoleNames.Admin })
    {
        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new IdentityRole(roleName));
    }

    var superAdminRole = await roleManager.FindByNameAsync(RoleNames.SuperAdmin);
    if (superAdminRole is not null)
        await SyncRolePermissionsAsync(roleManager, superAdminRole, Permissions.All);

    var adminRole = await roleManager.FindByNameAsync(RoleNames.Admin);
    if (adminRole is not null)
    {
        var adminStarterPermissions = Permissions.All
            .Where(p => p.StartsWith("Settings.", StringComparison.Ordinal)
                        && p != Permissions.Settings.EmailConfig.Create
                        && p != Permissions.Settings.EmailConfig.Edit
                        && p != Permissions.Settings.EmailConfig.Delete)
            .Append(Permissions.Users.View)
            .Append(Permissions.Users.Create)
            .Append(Permissions.Users.Edit)
            .Append(Permissions.Users.Delete)
            .Append(Permissions.Roles.View)
            .Append(Permissions.Roles.Create)
            .Append(Permissions.Roles.Edit)
            .Append(Permissions.Roles.Delete)
            .Append(Permissions.Accounts.Account.View)
            .Append(Permissions.Accounts.Account.Create)
            .Append(Permissions.Accounts.Account.Edit)
            .Append(Permissions.Accounts.Account.Delete)
            .Append(Permissions.Accounts.Deposit.View)
            .Append(Permissions.Accounts.Deposit.Create)
            .Append(Permissions.Accounts.Deposit.Delete)
            .Append(Permissions.Accounts.Expense.View)
            .Append(Permissions.Accounts.Expense.Create)
            .Append(Permissions.Accounts.Expense.Delete)
            .Append(Permissions.Accounts.Transfer.View)
            .Append(Permissions.Accounts.Transfer.Create)
            .Append(Permissions.Accounts.Transfer.Delete)
            .Append(Permissions.Sales.Invoice.View)
            .Append(Permissions.Sales.Invoice.Create)
            .Append(Permissions.Sales.Invoice.Edit)
            .Append(Permissions.Sales.Invoice.Delete)
            .Append(Permissions.Sales.Invoice.Manage)
            .Append(Permissions.License.View);
        await SyncRolePermissionsAsync(roleManager, adminRole, adminStarterPermissions);
    }

    // Resolve the default tenant (Id = 1) for dev user TenantId assignment
    var defaultTenant = await masterDb.Tenants.FirstOrDefaultAsync(t => t.Slug == "default");
    var defaultTenantId = defaultTenant?.Id;

    var devUsers = new[]
    {
        (Email: "admin@litxuscount.local", DisplayName: "Super Admin", Role: RoleNames.SuperAdmin, TenantId: (long?)null),
        (Email: "companyadmin@litxuscount.local", DisplayName: "Company Admin", Role: RoleNames.Admin, TenantId: defaultTenantId),
    };

    foreach (var devUser in devUsers)
    {
        var user = await userManager.FindByEmailAsync(devUser.Email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = devUser.Email,
                Email = devUser.Email,
                DisplayName = devUser.DisplayName,
                EmailConfirmed = true,
                TenantId = devUser.TenantId,
            };
            await userManager.CreateAsync(user, "DevPassw0rd!");
        }
        else if (user.TenantId != devUser.TenantId)
        {
            user.TenantId = devUser.TenantId;
            await userManager.UpdateAsync(user);
        }

        if (!await userManager.IsInRoleAsync(user, devUser.Role))
            await userManager.AddToRoleAsync(user, devUser.Role);
    }

    // Seed the default tenant's operational database
    if (defaultTenant is not null)
    {
        var tenantOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(defaultTenant.ConnectionString)
            .Options;
        await using var tenantDb = new ApplicationDbContext(tenantOptions);
        var encryptor = scope.ServiceProvider.GetRequiredService<IEmailConfigEncryptor>();
        await SystemSettingsSeeder.SeedAsync(tenantDb, encryptor);
    }
}

app.Run();

static string BuildTenantConnectionString(string masterConnectionString, string slug)
{
    var csb = new NpgsqlConnectionStringBuilder(masterConnectionString)
    {
        Database = $"litxuscount_{slug}"
    };
    return csb.ConnectionString;
}

static async Task SyncRolePermissionsAsync(RoleManager<IdentityRole> roleManager, IdentityRole role, IEnumerable<string> permissions)
{
    var desiredSet = permissions.ToHashSet();
    var existingClaims = (await roleManager.GetClaimsAsync(role))
        .Where(c => c.Type == Permissions.ClaimType)
        .ToList();

    foreach (var claim in existingClaims.Where(c => !desiredSet.Contains(c.Value)))
        await roleManager.RemoveClaimAsync(role, claim);

    var existingValues = existingClaims.Select(c => c.Value).ToHashSet();
    foreach (var p in desiredSet.Where(p => !existingValues.Contains(p)))
        await roleManager.AddClaimAsync(role, new Claim(Permissions.ClaimType, p));
}
