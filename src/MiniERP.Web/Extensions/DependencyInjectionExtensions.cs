using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using MiniERP.Web.Features.Departments;
using MiniERP.Web.Features.Positions;
using MiniERP.Web.Infrastructure.Persistence;

namespace MiniERP.Web.Extensions;

internal static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMiniERP(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        // Resolve database options only when a database feature is requested.
        // The home page remains usable before local database setup is complete.
        services.AddDbContext<AppDbContext>(options =>
        {
            string? connectionString = configuration.GetConnectionString("TestingSystem")
                ?? configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Configure ConnectionStrings:TestingSystem using user secrets or environment variables.");
            }

            var sqlConnection = new SqlConnectionStringBuilder(connectionString);
            // Compose publishes only IPv4 loopback. Avoid localhost resolving
            // to ::1 in Development; retain the configured port and credentials.
            if (environment.IsDevelopment())
            {
                string server = sqlConnection.DataSource;
                if (server.StartsWith("tcp:", StringComparison.OrdinalIgnoreCase))
                    server = server[4..];

                string[] parts = server.Split(',');
                if (parts.Length <= 2 && parts[0].Equals("localhost", StringComparison.OrdinalIgnoreCase))
                {
                    sqlConnection.DataSource = "tcp:127.0.0.1" + (parts.Length == 2 ? "," + parts[1] : string.Empty);
                }
            }

            options.UseSqlServer(sqlConnection.ConnectionString);
        });

        return services.AddDepartments().AddPositions();
    }
}
