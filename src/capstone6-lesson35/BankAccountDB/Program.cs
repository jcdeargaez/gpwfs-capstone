using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.DependencyInjection;
using Capstone6;

var serviceProvider = CreateServices();
using (var scope = serviceProvider.CreateScope())
{
    UpdateDatabase(scope.ServiceProvider);
}

static IServiceProvider CreateServices() =>
    new ServiceCollection()
        .AddFluentMigratorCore()
        .ConfigureRunner(rb => rb
            .AddSQLite()
            .WithGlobalConnectionString("Data Source=BankAccountDB.db")
            .ScanIn(typeof(AddAccountTable).Assembly).For.Migrations())
        .AddLogging(lb => lb.AddFluentMigratorConsole())
        .Configure<RunnerOptions>(opt => opt.Profile = "Development")
        .BuildServiceProvider(false);

static void UpdateDatabase(IServiceProvider serviceProvider) =>
    serviceProvider
        .GetRequiredService<IMigrationRunner>()
        .MigrateUp();
