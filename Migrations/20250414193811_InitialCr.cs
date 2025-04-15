using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agreement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MouFileName",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "MouStoredName",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "NdaFileName",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "NdaStoredName",
                table: "Agreements");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ppiiexpire",
                table: "Agreements",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "hpcsaexpire",
                table: "Agreements",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "drphysicaddrs",
                table: "Agreements",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "bohfexpire",
                table: "Agreements",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ppiiexpire",
                table: "Agreements",
                type: "text",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "hpcsaexpire",
                table: "Agreements",
                type: "text",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "drphysicaddrs",
                table: "Agreements",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "bohfexpire",
                table: "Agreements",
                type: "text",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "MouFileName",
                table: "Agreements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MouStoredName",
                table: "Agreements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NdaFileName",
                table: "Agreements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NdaStoredName",
                table: "Agreements",
                type: "text",
                nullable: true);
        }
    }
}
