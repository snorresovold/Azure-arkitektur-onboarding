using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
public class CosmosHandler {
    private readonly string _CosmosDBConnectionString;
    private readonly string _CosmosKey;
    private readonly ILogger _logger;

    public CosmosHandler(ILogger logger) {
        _logger = logger;
    }
    public ILogger Logger { get; }
    public async Task<(Container, Database)> Init(string CosmosDbConnectionString, string CosmosKey) {
        using CosmosClient client = new(
                accountEndpoint: Environment.GetEnvironmentVariable("CosmosDbConnectionString")!,
                authKeyOrResourceToken: Environment.GetEnvironmentVariable("CosmosKey")!
            );
            // New instance of Database response class referencing the server-side database
            DatabaseResponse response = await client.CreateDatabaseIfNotExistsAsync(
                id: "TimeTrackingEntries"
            );
            // Parse additional response properties
            Database database = response.Database;
            // New instance of Container class referencing the server-side container
            ContainerResponse response2 = await database.CreateContainerIfNotExistsAsync(
                id: "TimeTrackingList",
                partitionKeyPath: "/category"
            );
            // Parse additional response properties
            Container container = response2.Container;
            return (container, database);
    }
    // fix this after lunsh :smile:
    public async Task CreateTimeTrackingEntry(Product item) {
        Product item = new(
            id: "1",
            category: "gear-surf-surfboards",
            name: "Sunnox Surfboard",
            quantity: 8,
            sale: true
        );
        Product createdItem = await container.CreateItemAsync<Product>(
            item: item,
            partitionKey: new PartitionKey("gear-surf-surfboards")
        );
    }
    public record Product(string id, string category, string name, int quantity, bool sale);
}