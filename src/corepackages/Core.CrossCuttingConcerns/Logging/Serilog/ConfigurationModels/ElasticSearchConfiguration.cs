namespace Core.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;

public class ElasticSearchConfiguration
{
    public string ConnectionString { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    public ElasticSearchConfiguration()
    {
        ConnectionString = string.Empty;
        UserName = string.Empty;
        Password = string.Empty;
    }

    public ElasticSearchConfiguration(string connectionString, string userName, string password)
    {
        ConnectionString = connectionString;
        UserName = userName;
        Password = password;
    }
}

