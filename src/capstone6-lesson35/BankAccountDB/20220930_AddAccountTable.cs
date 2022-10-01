namespace Capstone6;

using FluentMigrator;

[Migration(20220930072411)]
public class AddAccountTable : Migration
{
    public override void Up()
    {
        Create.Table("Account")
            .WithColumn("Owner").AsString(size: 256).NotNullable().PrimaryKey()
            .WithColumn("AccountId").AsGuid().NotNullable().Unique("IX_Account_AccountId");
    }

    public override void Down()
    {
        Delete.Index("IX_Account_AccountId").OnTable("Account");
        Delete.Table("Account");
    }
}
