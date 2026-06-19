using System.Security.Claims;
using System.Text;
using LitXusCount.Application.Auth;
using LitXusCount.Application.Authorization;
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
    .AddDbContextCheck<ApplicationDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LitXusCount API v1"));
}

app.UseHttpsRedirection();

app.UseCors(FrontendCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

using (var migrationScope = app.Services.CreateScope())
{
    var dbContext = migrationScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

var seedDemoData = app.Environment.IsDevelopment() ||
    string.Equals(Environment.GetEnvironmentVariable("SEED_DEMO_DATA"), "true", StringComparison.OrdinalIgnoreCase);

if (seedDemoData)
{
    using var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var roleName in new[] { RoleNames.SuperAdmin, RoleNames.Admin })
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    var superAdminRole = await roleManager.FindByNameAsync(RoleNames.SuperAdmin);
    if (superAdminRole is not null)
    {
        await EnsureRoleHasPermissionsAsync(roleManager, superAdminRole, Permissions.All);
    }

    var adminRole = await roleManager.FindByNameAsync(RoleNames.Admin);
    if (adminRole is not null)
    {
        var adminStarterPermissions = Permissions.All
            .Where(p => p.StartsWith("Settings.", StringComparison.Ordinal))
            .Append(Permissions.Users.View)
            .Append(Permissions.Roles.View);
        await EnsureRoleHasPermissionsAsync(roleManager, adminRole, adminStarterPermissions);
    }

    var devUsers = new[]
    {
        (Email: "admin@litxuscount.local", DisplayName: "Super Admin", Role: RoleNames.SuperAdmin),
        (Email: "companyadmin@litxuscount.local", DisplayName: "Company Admin", Role: RoleNames.Admin),
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
            };
            await userManager.CreateAsync(user, "DevPassw0rd!");
        }

        if (!await userManager.IsInRoleAsync(user, devUser.Role))
        {
            await userManager.AddToRoleAsync(user, devUser.Role);
        }
    }

    await SystemSettingsSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();

static async Task EnsureRoleHasPermissionsAsync(RoleManager<IdentityRole> roleManager, IdentityRole role, IEnumerable<string> permissions)
{
    var existing = (await roleManager.GetClaimsAsync(role))
        .Where(c => c.Type == Permissions.ClaimType)
        .Select(c => c.Value)
        .ToHashSet();

    foreach (var permission in permissions.Where(p => !existing.Contains(p)))
    {
        await roleManager.AddClaimAsync(role, new Claim(Permissions.ClaimType, permission));
    }
}
