namespace Capstone7;

using FluentMigrator;

[Migration(20220930075700)]
public class AddOperationTable : Migration
{
    public override void Up()
    {
        Create.Table("Operation")
            .WithColumn("OperationId").AsInt32().NotNullable().PrimaryKey()
            .WithColumn("Description").AsString().NotNullable();

        Insert.IntoTable("Operation")
            .Row(new { OperationId = 1, Description = "Withdraw" })
            .Row(new { OperationId = 2, Description = "Deposit" });
    }

    public override void Down()
    {
        Delete.Table("Operation");
    }
}
