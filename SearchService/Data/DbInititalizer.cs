using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using Newtonsoft.Json;

namespace SearchService.Data
{
    public class DbInititalizer
    {
        public static async Task InitDb(WebApplication app)
        {
            await DB.InitAsync("SearchDb", MongoClientSettings
                .FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

            await DB.Index<Item>()
                .Key(x => x.Make, KeyType.Text)
                .Key(x => x.Model, KeyType.Text)
                .Key(x => x.Color, KeyType.Text)
                .CreateAsync();

            var count = await DB.CountAsync<Item>();

            if (count == 0)
            {
                Console.WriteLine("No Data - will attempt to seed");
                var itemData = await File.ReadAllTextAsync("Data/auctions.json");

                var items = JsonConvert.DeserializeObject<List<Item>>(itemData);

                await DB.SaveAsync(items);
            }
        }
    }
}
