using Core.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using Microsoft.Extensions.Configuration;
using NpgsqlTypes;
using Serilog;
using Serilog.Sinks.PostgreSQL;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Logger;

public class PostgreSqlLogger : LoggerServiceBase
{
    public PostgreSqlLogger(IConfiguration configuration)
    {
        PostgreSqlConfiguration logConfiguration =
            configuration.GetSection("SerilogConfigurations:PostgreSqlConfiguration").Get<PostgreSqlConfiguration>()
            ?? throw new Exception("Serilog PostgreSQL configuration is missing.");

        IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
            { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
            { "level", new LevelColumnWriter(true, NpgsqlDbType.Text) },
            { "time_stamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp) },
            { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
            { "log_event", new LogEventSerializedColumnWriter(NpgsqlDbType.Json) },
            { "user_name", new SinglePropertyColumnWriter("user_name", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") }
        };

        Logger = new LoggerConfiguration()
            .WriteTo.PostgreSQL(
                connectionString: logConfiguration.ConnectionString,
                tableName: logConfiguration.TableName,
                columnOptions: columnWriters,
                needAutoCreateTable: logConfiguration.AutoCreateSqlTable
            )
            .CreateLogger();
    }
}

