using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class AdddedDbSets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SinglePayment_AspNetUsers_TargetId",
                table: "SinglePayment");

            migrationBuilder.DropForeignKey(
                name: "FK_SinglePayment_UnidirectionalPaymentGroup_UnidirectionalPaymentGroupId",
                table: "SinglePayment");

            migrationBuilder.DropForeignKey(
                name: "FK_UnidirectionalPaymentGroup_AspNetUsers_PaymentById",
                table: "UnidirectionalPaymentGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_UnidirectionalPaymentGroup_UserGroups_SinglePurposeUserGroupId",
                table: "UnidirectionalPaymentGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnidirectionalPaymentGroup",
                table: "UnidirectionalPaymentGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SinglePayment",
                table: "SinglePayment");

            migrationBuilder.RenameTable(
                name: "UnidirectionalPaymentGroup",
                newName: "PaymentGroups");

            migrationBuilder.RenameTable(
                name: "SinglePayment",
                newName: "SinglePayments");

            migrationBuilder.RenameIndex(
                name: "IX_UnidirectionalPaymentGroup_SinglePurposeUserGroupId",
                table: "PaymentGroups",
                newName: "IX_PaymentGroups_SinglePurposeUserGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_UnidirectionalPaymentGroup_PaymentById",
                table: "PaymentGroups",
                newName: "IX_PaymentGroups_PaymentById");

            migrationBuilder.RenameIndex(
                name: "IX_SinglePayment_UnidirectionalPaymentGroupId",
                table: "SinglePayments",
                newName: "IX_SinglePayments_UnidirectionalPaymentGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_SinglePayment_TargetId",
                table: "SinglePayments",
                newName: "IX_SinglePayments_TargetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentGroups",
                table: "PaymentGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SinglePayments",
                table: "SinglePayments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentGroups_AspNetUsers_PaymentById",
                table: "PaymentGroups",
                column: "PaymentById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentGroups_UserGroups_SinglePurposeUserGroupId",
                table: "PaymentGroups",
                column: "SinglePurposeUserGroupId",
                principalTable: "UserGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SinglePayments_AspNetUsers_TargetId",
                table: "SinglePayments",
                column: "TargetId",
                principalTable: "AspNetUsers",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentGroups_AspNetUsers_PaymentById",
                table: "PaymentGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentGroups_UserGroups_SinglePurposeUserGroupId",
                table: "PaymentGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SinglePayments_AspNetUsers_TargetId",
                table: "SinglePayments");

            migrationBuilder.DropForeignKey(
                name: "FK_SinglePayments_PaymentGroups_UnidirectionalPaymentGroupId",
                table: "SinglePayments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SinglePayments",
                table: "SinglePayments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentGroups",
                table: "PaymentGroups");

            migrationBuilder.RenameTable(
                name: "SinglePayments",
                newName: "SinglePayment");

            migrationBuilder.RenameTable(
                name: "PaymentGroups",
                newName: "UnidirectionalPaymentGroup");

            migrationBuilder.RenameIndex(
                name: "IX_SinglePayments_UnidirectionalPaymentGroupId",
                table: "SinglePayment",
                newName: "IX_SinglePayment_UnidirectionalPaymentGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_SinglePayments_TargetId",
                table: "SinglePayment",
                newName: "IX_SinglePayment_TargetId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentGroups_SinglePurposeUserGroupId",
                table: "UnidirectionalPaymentGroup",
                newName: "IX_UnidirectionalPaymentGroup_SinglePurposeUserGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentGroups_PaymentById",
                table: "UnidirectionalPaymentGroup",
                newName: "IX_UnidirectionalPaymentGroup_PaymentById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SinglePayment",
                table: "SinglePayment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnidirectionalPaymentGroup",
                table: "UnidirectionalPaymentGroup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SinglePayment_AspNetUsers_TargetId",
                table: "SinglePayment",
                column: "TargetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SinglePayment_UnidirectionalPaymentGroup_UnidirectionalPaymentGroupId",
                table: "SinglePayment",
                column: "UnidirectionalPaymentGroupId",
                principalTable: "UnidirectionalPaymentGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnidirectionalPaymentGroup_AspNetUsers_PaymentById",
                table: "UnidirectionalPaymentGroup",
                column: "PaymentById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnidirectionalPaymentGroup_UserGroups_SinglePurposeUserGroupId",
                table: "UnidirectionalPaymentGroup",
                column: "SinglePurposeUserGroupId",
                principalTable: "UserGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
