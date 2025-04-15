using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agreement.Migrations
{
    /// <inheritdoc />
    public partial class InitCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Agreements",
                newName: "timecaptured");

            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "Agreements",
                newName: "ppiiexpire");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Agreements",
                newName: "physemail");

            migrationBuilder.AddColumn<string>(
                name: "active",
                table: "Agreements",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "bhff",
                table: "Agreements",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "bohfexpire",
                table: "Agreements",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "bohffile",
                table: "Agreements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bohffileStoredname",
                table: "Agreements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "datecaputred",
                table: "Agreements",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "draddress",
                table: "Agreements",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "drcell",
                table: "Agreements",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "dremail",
                table: "Agreements",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "dridnr",
                table: "Agreements",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "drname",
                table: "Agreements",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "drphysicaddrs",
                table: "Agreements",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "drsurname",
                table: "Agreements",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "emerfile",
                table: "Agreements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "emerfileStoredName",
                table: "Agreements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "formid",
                table: "Agreements",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "formname",
                table: "Agreements",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "hosigned",
                table: "Agreements",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "hpcsaexpire",
                table: "Agreements",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "hpcsafile",
                table: "Agreements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hpcsafileStoredName",
                table: "Agreements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "idfile",
                table: "Agreements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "idfileStoredName",
                table: "Agreements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ppiifile",
                table: "Agreements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ppiifileStoredName",
                table: "Agreements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "qsfile",
                table: "Agreements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "qsfileStoredName",
                table: "Agreements",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "active",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "bhff",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "bohfexpire",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "bohffile",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "bohffileStoredname",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "datecaputred",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "draddress",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "drcell",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "dremail",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "dridnr",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "drname",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "drphysicaddrs",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "drsurname",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "emerfile",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "emerfileStoredName",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "formid",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "formname",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "hosigned",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "hpcsaexpire",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "hpcsafile",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "hpcsafileStoredName",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "idfile",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "idfileStoredName",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "ppiifile",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "ppiifileStoredName",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "qsfile",
                table: "Agreements");

            migrationBuilder.DropColumn(
                name: "qsfileStoredName",
                table: "Agreements");

            migrationBuilder.RenameColumn(
                name: "timecaptured",
                table: "Agreements",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "ppiiexpire",
                table: "Agreements",
                newName: "Surname");

            migrationBuilder.RenameColumn(
                name: "physemail",
                table: "Agreements",
                newName: "Email");
        }
    }
}
