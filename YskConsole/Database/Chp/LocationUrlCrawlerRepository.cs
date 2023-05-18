using MongoDB.Driver;
using YskConsole.Models;

namespace YskConsole.Database.Repository
{
    public class LocationUrlCrawlerRepository
    {
        public LocationUrlCrawlerRepository()
        {
            mongoDbConnector = new MongodbConnector();
            MongodbCollection = mongoDbConnector.GetCollection<LocationUrlCrawler>("LocationUrlCrawler");
        }

        public MongodbConnector mongoDbConnector { get; }
        private readonly IMongoCollection<LocationUrlCrawler> MongodbCollection;


        public async Task<List<LocationUrlCrawler>> GetTcs(List<string> tcs)
        {
            var filter = new List<FilterDefinition<LocationUrlCrawler>>();

            foreach (var category in tcs)
            {
                filter.Add(Builders<LocationUrlCrawler>.Filter.Eq("TcKimlikNo", category));
            }



            return await MongodbCollection.Find(Builders<LocationUrlCrawler>.Filter.Or(filter)).ToListAsync();
        }

        public async Task<List<LocationUrlCrawler>> GetAll()
        {
            return await MongodbCollection.Find(x => x.Id != null).ToListAsync();
        }

        public async Task Add(IEnumerable<LocationUrlCrawler> models)
        {
            if (models.Any())
                await MongodbCollection.InsertManyAsync(models);
        }
        public async Task Add(LocationUrlCrawler model)
        {
            await MongodbCollection.InsertOneAsync(model);
        }

        public async Task Delete(string Id)
        {

            await MongodbCollection.DeleteOneAsync(x => x.Id == Id);
        }

        public async Task<List<LocationUrlCrawler>> GetById(List<string> ids)
        {
            var filter = Builders<LocationUrlCrawler>.Filter.In(x => x.Id, ids);

            return await MongodbCollection.Find(filter).ToListAsync();
        }
        public async Task<LocationUrlCrawler> GetById(string ıd)
        {
            return await MongodbCollection.Find(x => x.Id == ıd).SingleOrDefaultAsync();
        }

    }
}
