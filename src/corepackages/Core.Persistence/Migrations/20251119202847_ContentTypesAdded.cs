using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ContentTypesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ProviderId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: true),
                    Score = table.Column<double>(type: "double precision", nullable: false),
                    BaseScore = table.Column<double>(type: "double precision", nullable: false),
                    FreshnessScore = table.Column<double>(type: "double precision", nullable: false),
                    InteractionScore = table.Column<double>(type: "double precision", nullable: false),
                    ContentType = table.Column<int>(type: "integer", nullable: false),
                    ReadingTime = table.Column<int>(type: "integer", nullable: true),
                    Reactions = table.Column<int>(type: "integer", nullable: true),
                    Comments = table.Column<int>(type: "integer", nullable: true),
                    Views = table.Column<long>(type: "bigint", nullable: true),
                    Likes = table.Column<int>(type: "integer", nullable: true),
                    Duration = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contents_ContentType",
                table: "Contents",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_ProviderId_Source",
                table: "Contents",
                columns: new[] { "ProviderId", "Source" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contents_PublishedDate",
                table: "Contents",
                column: "PublishedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_Score",
                table: "Contents",
                column: "Score");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contents");
        }
    }
}
