using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevMetricsPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGitHubFieldsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GitHubAccessToken",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GitHubConnectedAt",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GitHubUserId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GitHubUsername",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GitHubAccessToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GitHubConnectedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GitHubUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GitHubUsername",
                table: "AspNetUsers");
        }
    }
}
