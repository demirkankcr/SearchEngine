using Core.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Logger;

public class ElasticSearchLogger : LoggerServiceBase
{
    public ElasticSearchLogger(IConfiguration configuration)
    {
        ElasticSearchConfiguration logConfiguration =
            configuration.GetSection("SerilogConfigurations:ElasticSearchConfiguration").Get<ElasticSearchConfiguration>()
            ?? throw new Exception("Serilog ElasticSearch configuration is missing.");

        Logger = new LoggerConfiguration()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(logConfiguration.ConnectionString))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8, // Uygun sürüm seçilmeli
                IndexFormat = "search-engine-logs-{0:yyyy.MM.dd}",
                ModifyConnectionSettings = x => x.BasicAuthentication(logConfiguration.UserName, logConfiguration.Password)
            })
            .CreateLogger();
    }
}

