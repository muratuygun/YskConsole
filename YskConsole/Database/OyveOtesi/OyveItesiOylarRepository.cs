using MongoDB.Driver;
using YskConsole.Models;
using YskConsole.Utility;

namespace YskConsole.Database.Repository
{
    public class OyveItesiOylarRepository
    {
        public OyveItesiOylarRepository()
        {
            mongoDbConnector = new MongodbConnector();
            MongodbCollection = mongoDbConnector.GetCollection<OyveItesiOylar>("OyveItesiOylar");
        }

        public MongodbConnector mongoDbConnector { get; }
        private readonly IMongoCollection<OyveItesiOylar> MongodbCollection;



        public async Task<List<OyveItesiOylar>> GetAll(List<int> NeighborhoodIds)
        {
            var filter = new List<FilterDefinition<OyveItesiOylar>>();

            foreach (var category in NeighborhoodIds)
            {
                filter.Add(Builders<OyveItesiOylar>.Filter.Eq("NeighborhoodId", category));
            }



            return await MongodbCollection.Find(Builders<OyveItesiOylar>.Filter.Or(filter)).ToListAsync();
        }

        public async Task<List<OyveItesiOylar>> GetAll()
        {
            return await MongodbCollection.Find(x => x.Id != null).ToListAsync();
        }

        public async Task Add(IEnumerable<OyveItesiOylar> models)
        {
            if (models.Any())
                await MongodbCollection.InsertManyAsync(models);
        }
        public async Task Add(OyveItesiOylar model)
        {
            await MongodbCollection.InsertOneAsync(model);
        }

        public async Task Delete(string Id)
        {

            await MongodbCollection.DeleteOneAsync(x => x.Id == Id);
        }

        public async Task<List<OyveItesiOylar>> GetById(List<string> ids)
        {
            var filter = Builders<OyveItesiOylar>.Filter.In(x => x.Id, ids);

            return await MongodbCollection.Find(filter).ToListAsync();
        }
        public async Task<OyveItesiOylar> GetById(string ıd)
        {
            return await MongodbCollection.Find(x => x.Id == ıd).SingleOrDefaultAsync();
        }

        public async Task UpdateBulkCityDistrict(List<OyveItesiOylar> models)
        {

            foreach (var item in models.Partition(1000))
            {

                var listWrites = new List<WriteModel<OyveItesiOylar>>();

                foreach (var model in item)
                {
                    var filter = Builders<OyveItesiOylar>.Filter.Eq(x => x.Id, model.Id);
                    var update = Builders<OyveItesiOylar>.Update
                        .Set(x => x.CityName, model.CityName)
                        .Set(x => x.DistrictName, model.DistrictName)
                        .Set(x => x.NeighborhoodName, model.NeighborhoodName);

                    listWrites.Add(new UpdateManyModel<OyveItesiOylar>(filter, update));
                }


                await MongodbCollection.BulkWriteAsync(listWrites);
            }

        }
    }
}
