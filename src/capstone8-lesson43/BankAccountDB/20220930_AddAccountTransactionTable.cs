namespace Capstone8;

using FluentMigrator;

[Migration(20220930075900)]
public class AddAccountTransactionTable : Migration
{
    public override void Up()
    {
        Create.Table("AccountTransaction")
            .WithColumn("AccountTransactionId").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("AccountId").AsGuid().NotNullable().ForeignKey("Account", "AccountId").Indexed("IX_AccountTransaction_AccountId")
            .WithColumn("Timestamp").AsDateTime().NotNullable()
            .WithColumn("OperationId").AsInt32().NotNullable().ForeignKey("Operation", "OperationId")
            .WithColumn("Amount").AsDecimal().NotNullable();
    }

    public override void Down()
    {
        Delete.Index("IX_AccountTransaction_AccountId").OnTable("AccountTransaction");
        Delete.Table("AccountTransaction");
    }
}
