using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace LitXusCount.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for EF Core tooling (dotnet ef migrations).
/// Uses master connection string with a dev-tenant database name for migration generation.
/// </summary>
public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var apiPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "LitXusCount.API");
        var config = new ConfigurationBuilder()
            .SetBasePath(apiPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var masterCs = config.GetConnectionString("DefaultConnection")!;
        var builder = new NpgsqlConnectionStringBuilder(masterCs)
        {
            Database = "litxuscount_dev_tenant"
        };

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(builder.ConnectionString)
            .Options;

        return new ApplicationDbContext(options);
    }
}
