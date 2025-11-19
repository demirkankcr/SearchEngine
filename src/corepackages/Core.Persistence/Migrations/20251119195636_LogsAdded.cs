using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LogsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GetLog = table.Column<string>(type: "text", nullable: true),
                    GetErrorLog = table.Column<string>(type: "text", nullable: true),
                    EventId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    UserId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    LogDomain = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Host = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Path = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Scheme = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    QueryString = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RemoteIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Headers = table.Column<string>(type: "text", nullable: true),
                    RequestBody = table.Column<string>(type: "text", nullable: true),
                    RequestMethod = table.Column<string>(type: "text", nullable: true),
                    UserAgent = table.Column<string>(type: "text", nullable: true),
                    ResponseHeaders = table.Column<string>(type: "text", nullable: true),
                    ResponseBody = table.Column<string>(type: "text", nullable: true),
                    StatusCode = table.Column<int>(type: "integer", nullable: true),
                    ResponseTime = table.Column<long>(type: "bigint", nullable: true),
                    Exception = table.Column<string>(type: "text", nullable: true),
                    ExceptionMessage = table.Column<string>(type: "text", nullable: true),
                    InnerException = table.Column<string>(type: "text", nullable: true),
                    InnerExceptionMessage = table.Column<string>(type: "text", nullable: true),
                    LogDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
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
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
