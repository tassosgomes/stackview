using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackShare.Infrastructure.Data;
using Testcontainers.PostgreSql;
using Respawn;
using StackShare.API;

namespace StackShare.IntegrationTests.Infrastructure;

public class StackShareWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .WithDatabase("stackshare_test")
        .WithUsername("test")
        .WithPassword("test")
        .WithCleanUp(true)
        .Build();

    private Respawner _respawner = null!;
    private string _connectionString = null!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        _connectionString = _dbContainer.GetConnectionString();

        // Initialize Respawner
        using var services = Services.CreateScope();
        var context = services.ServiceProvider.GetRequiredService<StackShareDbContext>();
        await context.Database.MigrateAsync();

        // Use DbConnection overload for PostgreSQL
        using var connection = context.Database.GetDbConnection();
        await connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" }
        });
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        using var services = Services.CreateScope();
        var context = services.ServiceProvider.GetRequiredService<StackShareDbContext>();
        using var connection = context.Database.GetDbConnection();
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StackShareDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add the test database
            services.AddDbContext<StackShareDbContext>(options =>
            {
                options.UseNpgsql(_connectionString);
            });

            // Reduce logging noise in tests
            services.Configure<LoggerFilterOptions>(options =>
            {
                options.MinLevel = LogLevel.Warning;
            });
        });

        builder.UseEnvironment("Testing");
    }

    public StackShareDbContext CreateDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<StackShareDbContext>();
    }
}