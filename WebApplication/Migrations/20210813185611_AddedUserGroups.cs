using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class AddedUserGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserSinglePurposeUserGroup",
                columns: table => new
                {
                    GroupUsersId = table.Column<string>(type: "TEXT", nullable: false),
                    PaymentGroupsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserSinglePurposeUserGroup", x => new { x.GroupUsersId, x.PaymentGroupsId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserSinglePurposeUserGroup_AspNetUsers_GroupUsersId",
                        column: x => x.GroupUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserSinglePurposeUserGroup_UserGroups_PaymentGroupsId",
                        column: x => x.PaymentGroupsId,
                        principalTable: "UserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnidirectionalPaymentGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PaymentById = table.Column<string>(type: "TEXT", nullable: true),
                    SinglePurposeUserGroupId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidirectionalPaymentGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnidirectionalPaymentGroup_AspNetUsers_PaymentById",
                        column: x => x.PaymentById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnidirectionalPaymentGroup_UserGroups_SinglePurposeUserGroupId",
                        column: x => x.SinglePurposeUserGroupId,
                        principalTable: "UserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SinglePayment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    TargetId = table.Column<string>(type: "TEXT", nullable: true),
                    UnidirectionalPaymentGroupId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinglePayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SinglePayment_AspNetUsers_TargetId",
                        column: x => x.TargetId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SinglePayment_UnidirectionalPaymentGroup_UnidirectionalPaymentGroupId",
                        column: x => x.UnidirectionalPaymentGroupId,
                        principalTable: "UnidirectionalPaymentGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserSinglePurposeUserGroup_PaymentGroupsId",
                table: "ApplicationUserSinglePurposeUserGroup",
                column: "PaymentGroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_SinglePayment_TargetId",
                table: "SinglePayment",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_SinglePayment_UnidirectionalPaymentGroupId",
                table: "SinglePayment",
                column: "UnidirectionalPaymentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UnidirectionalPaymentGroup_PaymentById",
                table: "UnidirectionalPaymentGroup",
                column: "PaymentById");

            migrationBuilder.CreateIndex(
                name: "IX_UnidirectionalPaymentGroup_SinglePurposeUserGroupId",
                table: "UnidirectionalPaymentGroup",
                column: "SinglePurposeUserGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserSinglePurposeUserGroup");

            migrationBuilder.DropTable(
                name: "SinglePayment");

            migrationBuilder.DropTable(
                name: "UnidirectionalPaymentGroup");

            migrationBuilder.DropTable(
                name: "UserGroups");
        }
    }
}
