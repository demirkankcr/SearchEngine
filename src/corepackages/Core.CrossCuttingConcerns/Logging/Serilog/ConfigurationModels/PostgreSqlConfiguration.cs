namespace Core.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;

public class PostgreSqlConfiguration
{
    public string ConnectionString { get; set; }
    public string TableName { get; set; }
    public bool AutoCreateSqlTable { get; set; }

    public PostgreSqlConfiguration()
    {
        ConnectionString = string.Empty;
        TableName = string.Empty;
        AutoCreateSqlTable = true;
    }

    public PostgreSqlConfiguration(string connectionString, string tableName, bool autoCreateSqlTable)
    {
        ConnectionString = connectionString;
        TableName = tableName;
        AutoCreateSqlTable = autoCreateSqlTable;
    }
}

