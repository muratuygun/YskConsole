using MongoDB.Driver;
using YskConsole.Models;
using YskConsole.Utility;

namespace YskConsole.Database.Repository
{
    public class OyveOtesiDistrictsRepository
    {
        public OyveOtesiDistrictsRepository()
        {
            mongoDbConnector = new MongodbConnector();
            MongodbCollection = mongoDbConnector.GetCollection<OyveOtesiDistricts>("OyveOtesiDistricts");
        }

        public MongodbConnector mongoDbConnector { get; }
        private readonly IMongoCollection<OyveOtesiDistricts> MongodbCollection;


        public async Task<List<OyveOtesiDistricts>> GetAll(int CityId)
        {
            return await MongodbCollection.Find(x => x.CityId == CityId).ToListAsync();
        }

        public async Task<List<OyveOtesiDistricts>> GetAll()
        {
            return await MongodbCollection.Find(x => x.Id != null).ToListAsync();
        }

        public async Task Add(IEnumerable<OyveOtesiDistricts> models)
        {
            if (models.Any())
                await MongodbCollection.InsertManyAsync(models);
        }
        public async Task Add(OyveOtesiDistricts model)
        {
            await MongodbCollection.InsertOneAsync(model);
        }

        public async Task Delete(string Id)
        {

            await MongodbCollection.DeleteOneAsync(x => x.Id == Id);
        }

        public async Task<List<OyveOtesiDistricts>> GetById(List<string> ids)
        {
            var filter = Builders<OyveOtesiDistricts>.Filter.In(x => x.Id, ids);

            return await MongodbCollection.Find(filter).ToListAsync();
        }
        public async Task<OyveOtesiDistricts> GetById(string ıd)
        {
            return await MongodbCollection.Find(x => x.Id == ıd).SingleOrDefaultAsync();
        }

        public async Task UpdateBulkCityDistrict(List<OyveOtesiDistricts> models)
        {

            foreach (var item in models.Partition(1000))
            {

                var listWrites = new List<WriteModel<OyveOtesiDistricts>>();

                foreach (var model in item)
                {
                    var filter = Builders<OyveOtesiDistricts>.Filter.Eq(x => x.OyveOtesiId, model.OyveOtesiId);
                    var update = Builders<OyveOtesiDistricts>.Update
                        .Set(x => x.CityId, model.CityId);

                    listWrites.Add(new UpdateManyModel<OyveOtesiDistricts>(filter, update));
                }


                await MongodbCollection.BulkWriteAsync(listWrites);
            }

        }
    }
}
