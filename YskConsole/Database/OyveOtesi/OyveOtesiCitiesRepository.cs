using MongoDB.Driver;
using YskConsole.Models;

namespace YskConsole.Database.Repository
{
    public class OyveOtesiCitiesRepository
    {
        public OyveOtesiCitiesRepository()
        {
            mongoDbConnector = new MongodbConnector();
            MongodbCollection = mongoDbConnector.GetCollection<OyveOtesiCities>("OyveOtesiCities");
        }

        public MongodbConnector mongoDbConnector { get; }
        private readonly IMongoCollection<OyveOtesiCities> MongodbCollection;



        public async Task<List<OyveOtesiCities>> GetAll()
        {
            return await MongodbCollection.Find(x => x.Id != null).ToListAsync();
        }

        public async Task Add(IEnumerable<OyveOtesiCities> models)
        {
            if (models.Any())
                await MongodbCollection.InsertManyAsync(models);
        }
        public async Task Add(OyveOtesiCities model)
        {
            await MongodbCollection.InsertOneAsync(model);
        }

        public async Task Delete(string Id)
        {

            await MongodbCollection.DeleteOneAsync(x => x.Id == Id);
        }

        public async Task<List<OyveOtesiCities>> GetById(List<string> ids)
        {
            var filter = Builders<OyveOtesiCities>.Filter.In(x => x.Id, ids);

            return await MongodbCollection.Find(filter).ToListAsync();
        }
        public async Task<OyveOtesiCities> GetById(string ıd)
        {
            return await MongodbCollection.Find(x => x.Id == ıd).SingleOrDefaultAsync();
        }

    }
}
