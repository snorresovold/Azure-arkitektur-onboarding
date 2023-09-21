using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CosmosHandler
{
    private static CosmosClient _client;

    static CosmosHandler()
    {
        _client = new CosmosClient(
            accountEndpoint: Environment.GetEnvironmentVariable("CosmosDbConnectionString")!,
            authKeyOrResourceToken: Environment.GetEnvironmentVariable("CosmosKey")!
        );
    }

    public static async Task<(Container, Database)> Init()
    {
        // New instance of Database response class referencing the server-side database
        DatabaseResponse response = await _client.CreateDatabaseIfNotExistsAsync(
            id: "TimeTrackingEntries"
        );

        // Parse additional response properties
        Database database = response.Database;

        // New instance of Container class referencing the server-side container
        ContainerResponse response2 = await database.CreateContainerIfNotExistsAsync(
            id: "TimeTrackingList",
            partitionKeyPath: "/Consultant"
        );

        // Parse additional response properties
        Container container = response2.Container;

        return (container, database);
    }

    public async static Task<Product> CreateTimeTrackingEntry(Product item, Container container)
    {
        Product createdItem = await container.UpsertItemAsync(
            item: item
        );

        return createdItem;
    }

    public async static Task<T> ReadTimeTrackingEntry<T>(Container container, string id, string partitionKey)
    {
        T readItem = await container.ReadItemAsync<T>(
            id: id,
            partitionKey: new PartitionKey(partitionKey)
        );

        Console.WriteLine(readItem);

        return readItem;
    }

    public async static Task<List<TimeTrackingEntry>> ReadMultipleEntries<T>(Container container, List<string> ids, string partition)
    {
        // Create partition key object
        PartitionKey partitionKey = new(partition);

        // Create list of tuples for each item
        List<(string, PartitionKey)> itemsToFind = new() { };
        foreach (var id in ids)
        {
            itemsToFind.Add((id, partitionKey));
        }

        // Read multiple items
        FeedResponse<TimeTrackingEntry> feedResponse = await container.ReadManyItemsAsync<TimeTrackingEntry>(
            items: itemsToFind
        );

        foreach (TimeTrackingEntry item in feedResponse)
        {
            Console.WriteLine($"Found item:\t{item.Account}");
        }

        List<TimeTrackingEntry> resultList = feedResponse.ToList();

        return resultList;
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
