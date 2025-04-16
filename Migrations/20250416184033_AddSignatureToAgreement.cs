using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agreement.Migrations
{
    /// <inheritdoc />
    public partial class AddSignatureToAgreement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "drfullname",
                table: "Agreements",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "SignatureData",
                table: "Agreements",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SignedDate",
                table: "Agreements",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignatureData",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "SignedDate",
                table: "Agreements");

            migrationBuilder.AlterColumn<string>(
                name: "drfullname",
                table: "Agreements",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
