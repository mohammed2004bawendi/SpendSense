using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpendSense.Migrations
{
    /// <inheritdoc />
    public partial class AddBillAccountTransactionRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Bills",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransactionId",
                table: "Bills",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bills_AccountId",
                table: "Bills",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_TransactionId",
                table: "Bills",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_Accounts_AccountId",
                table: "Bills",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_Transactions_TransactionId",
                table: "Bills",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bills_Accounts_AccountId",
                table: "Bills");

            migrationBuilder.DropForeignKey(
                name: "FK_Bills_Transactions_TransactionId",
                table: "Bills");

            migrationBuilder.DropIndex(
                name: "IX_Bills_AccountId",
                table: "Bills");

            migrationBuilder.DropIndex(
                name: "IX_Bills_TransactionId",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Bills");
        }
    }
}
