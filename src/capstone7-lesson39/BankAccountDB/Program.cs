using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.DependencyInjection;
using Capstone7;

var serviceProvider = CreateServices();
using (var scope = serviceProvider.CreateScope())
{
    UpdateDatabase(scope.ServiceProvider);
}

static IServiceProvider CreateServices() =>
    new ServiceCollection()
        .AddFluentMigratorCore()
        .ConfigureRunner(rb => rb
            .AddSqlServer()
            .WithGlobalConnectionString(@"Data Source=(localdb)\MSSQLLocalDB;Database=BankAccountDB;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True")
            .ScanIn(typeof(AddAccountTable).Assembly).For.Migrations())
        .AddLogging(lb => lb.AddFluentMigratorConsole())
        .Configure<RunnerOptions>(opt => opt.Profile = "Development")
        .BuildServiceProvider(false);

static void UpdateDatabase(IServiceProvider serviceProvider) =>
    serviceProvider
        .GetRequiredService<IMigrationRunner>()
        .MigrateUp();
