using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceApi.Migrations
{
    /// <inheritdoc />
    public partial class pp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailOtp",
                table: "StudentTable",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailOtpExpiry",
                table: "StudentTable",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "StudentTable",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EmailOtp",
                table: "AdminTable",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailOtpExpiry",
                table: "AdminTable",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "AdminTable",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailOtp",
                table: "StudentTable");

            migrationBuilder.DropColumn(
                name: "EmailOtpExpiry",
                table: "StudentTable");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "StudentTable");

            migrationBuilder.DropColumn(
                name: "EmailOtp",
                table: "AdminTable");

            migrationBuilder.DropColumn(
                name: "EmailOtpExpiry",
                table: "AdminTable");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "AdminTable");
        }
    }
}
