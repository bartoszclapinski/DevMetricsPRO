using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevMetricsPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGitHubFieldsToRepository : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ForksCount",
                table: "Repositories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Repositories",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFork",
                table: "Repositories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Repositories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Repositories",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OpenIssuesCount",
                table: "Repositories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PushedAt",
                table: "Repositories",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StargazersCount",
                table: "Repositories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForksCount",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "IsFork",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "OpenIssuesCount",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "PushedAt",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "StargazersCount",
                table: "Repositories");
        }
    }
}
