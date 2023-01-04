using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MysqlConsoleApp.Migrations
{
    /// <inheritdoc />
    public partial class Create : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "token_id1",
                table: "bpm_proc_inst",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "token_id2",
                table: "bpm_proc_inst",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "token_id1",
                table: "bpm_proc_inst");

            migrationBuilder.DropColumn(
                name: "token_id2",
                table: "bpm_proc_inst");
        }
    }
}
