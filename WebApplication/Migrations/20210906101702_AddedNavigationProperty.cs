using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class AddedNavigationProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentGroups_UserGroups_SinglePurposeUserGroupId",
                table: "PaymentGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SinglePayments_PaymentGroups_UnidirectionalPaymentGroupId",
                table: "SinglePayments");

            migrationBuilder.RenameColumn(
                name: "UnidirectionalPaymentGroupId",
                table: "SinglePayments",
                newName: "PaymentGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_SinglePayments_UnidirectionalPaymentGroupId",
                table: "SinglePayments",
                newName: "IX_SinglePayments_PaymentGroupId");

            migrationBuilder.RenameColumn(
                name: "SinglePurposeUserGroupId",
                table: "PaymentGroups",
                newName: "UserGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentGroups_SinglePurposeUserGroupId",
                table: "PaymentGroups",
                newName: "IX_PaymentGroups_UserGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentGroups_UserGroups_UserGroupId",
                table: "PaymentGroups",
                column: "UserGroupId",
                principalTable: "UserGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SinglePayments_PaymentGroups_PaymentGroupId",
                table: "SinglePayments",
                column: "PaymentGroupId",
                principalTable: "PaymentGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentGroups_UserGroups_UserGroupId",
                table: "PaymentGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SinglePayments_PaymentGroups_PaymentGroupId",
                table: "SinglePayments");

            migrationBuilder.RenameColumn(
                name: "PaymentGroupId",
                table: "SinglePayments",
                newName: "UnidirectionalPaymentGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_SinglePayments_PaymentGroupId",
                table: "SinglePayments",
                newName: "IX_SinglePayments_UnidirectionalPaymentGroupId");

            migrationBuilder.RenameColumn(
                name: "UserGroupId",
                table: "PaymentGroups",
                newName: "SinglePurposeUserGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentGroups_UserGroupId",
                table: "PaymentGroups",
                newName: "IX_PaymentGroups_SinglePurposeUserGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentGroups_UserGroups_SinglePurposeUserGroupId",
                table: "PaymentGroups",
                column: "SinglePurposeUserGroupId",
                principalTable: "UserGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SinglePayments_PaymentGroups_UnidirectionalPaymentGroupId",
                table: "SinglePayments",
                column: "UnidirectionalPaymentGroupId",
                principalTable: "PaymentGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
