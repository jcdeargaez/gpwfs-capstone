namespace Capstone6;

using FluentMigrator;

[Profile("Development")]
public class SeedDevData : Migration
{
    public override void Up()
    {
        var accountId = Guid.NewGuid();

        Insert.IntoTable("Account")
            .Row(new { Owner = "juan", AccountId = accountId });

        Insert.IntoTable("AccountTransaction")
            .Row(new { AccountId = accountId, Timestamp = DateTime.UtcNow, OperationId = 2, Amount = 10 })
            .Row(new { AccountId = accountId, Timestamp = DateTime.UtcNow, OperationId = 2, Amount = 10 })
            .Row(new { AccountId = accountId, Timestamp = DateTime.UtcNow, OperationId = 2, Amount = 10 })
            .Row(new { AccountId = accountId, Timestamp = DateTime.UtcNow, OperationId = 2, Amount = 10 })
            .Row(new { AccountId = accountId, Timestamp = DateTime.UtcNow, OperationId = 2, Amount = 50 })
            .Row(new { AccountId = accountId, Timestamp = DateTime.UtcNow, OperationId = 1, Amount = 100 })
            .Row(new { AccountId = accountId, Timestamp = DateTime.UtcNow, OperationId = 2, Amount = 50 });
    }

    public override void Down()
    {
        // Nothing
    }
}
