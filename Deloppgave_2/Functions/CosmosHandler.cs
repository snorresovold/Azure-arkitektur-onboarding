using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
public class CosmosHandler
{
    private readonly ILogger _logger;

    public CosmosHandler(ILogger logger)
    {
        _logger = logger;
    }
    public static async Task<(Container, Database)> Init()
    {
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
    public static async Task<Product> CreateTimeTrackingEntry(Product item, Container container)
    {
        Product createdItem = await container.CreateItemAsync(
            item: item,
            partitionKey: new PartitionKey(item.category)
        );
        _logger.LogInformation(createdItem.ToString());
        return createdItem;
    }

    public static Product GenerateRandomProduct()
    {
        Random random = new Random();
        string id = random.Next(1, 1000).ToString(); // Generating a random number between 1 and 1000 for id
        string[] categories = { "gear-surf-surfboards", "jetskis", "catalouges" }; // List of possible categories
        string category = categories[random.Next(categories.Length)]; // Selecting a random category from the list
        string[] names = { "Sunnox Surfboard", "jetski", "catalouge" }; // List of possible names
        string name = names[random.Next(names.Length)]; // Selecting a random name from the list
        int quantity = random.Next(1, 11); // Generating a random number between 1 and 10 for quantity
        bool sale = random.Next(2) == 0; // Generating a random boolean value for sale (true or false)

        return new Product(id, category, name, quantity, sale);
    }
    public record Product(string id, string category, string name, int quantity, bool sale);
}