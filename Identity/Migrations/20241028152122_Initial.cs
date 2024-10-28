using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Identity.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ICNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PIN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Steps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    FlowStepType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Steps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ICNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    PIN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVerifyPhoneNumber = table.Column<bool>(type: "bit", nullable: false),
                    IsVerifyPIN = table.Column<bool>(type: "bit", nullable: false),
                    IsVerifyEmail = table.Column<bool>(type: "bit", nullable: false),
                    IsVerifyPolicy = table.Column<bool>(type: "bit", nullable: false),
                    IsVerifyBiometric = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestFlowStpes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    FlowStepType = table.Column<int>(type: "int", nullable: false),
                    IsFirstStep = table.Column<bool>(type: "bit", nullable: false),
                    IsLastStep = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestFlowStpes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestFlowStpes_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Steps",
                columns: new[] { "Id", "FlowStepType", "Name", "Type" },
                values: new object[,]
                {
                    { new Guid("2dd8b03e-edc0-4db2-8bb1-b5b327e98b8b"), 1, "Email Confrmation", 2 },
                    { new Guid("5e8569c9-6b12-4c48-ab9f-36cf196a6e6e"), 1, "Policy Confrmation", 3 },
                    { new Guid("790d83b4-4599-43e1-899b-067310f57376"), 1, "PIN Confrmation", 4 },
                    { new Guid("89c150e7-516e-4307-bc7f-8bc0c5975701"), 1, "Phone Number Confrmation", 1 },
                    { new Guid("e0321176-68f6-4bc7-8504-fd1e3584445e"), 2, "Biomtric Confirmation", 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestFlowStpes_RequestId",
                table: "RequestFlowStpes",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "IsVerifyEmail = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ICNumber",
                table: "Users",
                column: "ICNumber",
                unique: true,
                filter: "[ICNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                table: "Users",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber",
                unique: true,
                filter: "IsVerifyPhoneNumber = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestFlowStpes");

            migrationBuilder.DropTable(
                name: "Steps");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Requests");
        }
    }
}
