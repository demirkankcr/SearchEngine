using Core.CrossCuttingConcerns.Caching.Redis;
using Core.Persistence.Paging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace SearchEngine.Application.Tests.Infrastructure;

public class RedisSerializationTests
{
    // Bizim RedisCacheManager'da kullandığımız ayarların aynısı
    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        TypeNameHandling = TypeNameHandling.All
    };

    [Fact]
    public void Serialize_And_Deserialize_Interface_Should_Work()
    {
        // Arrange
        IPaginate<string> originalData = new Paginate<string>
        {
            Items = new List<string> { "Item1", "Item2" },
            Index = 0,
            Size = 10,
            Count = 2,
            Pages = 1
        };

        // Act
        // 1. Serialize
        string json = JsonConvert.SerializeObject(originalData, _jsonSettings);

        // 2. Deserialize
        var deserializedData = JsonConvert.DeserializeObject<IPaginate<string>>(json, _jsonSettings);

        // Assert
        Assert.NotNull(deserializedData);
        Assert.IsType<Paginate<string>>(deserializedData); // Somut tipe dönüşebilmeli
        Assert.Equal(originalData.Count, deserializedData.Count);
        Assert.Equal(originalData.Items.Count, deserializedData.Items.Count);
        Assert.Equal(originalData.Items[0], deserializedData.Items[0]);
    }

    [Fact]
    public void Serialize_And_Deserialize_ConcreteClass_Should_Work()
    {
        // Arrange
        var originalData = new TestData { Id = 1, Name = "Test" };

        // Act
        string json = JsonConvert.SerializeObject(originalData, _jsonSettings);
        var deserializedData = JsonConvert.DeserializeObject<TestData>(json, _jsonSettings);

        // Assert
        Assert.NotNull(deserializedData);
        Assert.Equal(originalData.Id, deserializedData.Id);
        Assert.Equal(originalData.Name, deserializedData.Name);
    }

    private class TestData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

