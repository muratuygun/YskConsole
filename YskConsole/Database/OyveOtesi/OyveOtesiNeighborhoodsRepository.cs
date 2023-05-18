using MongoDB.Driver;
using YskConsole.Models;
using YskConsole.Utility;

namespace YskConsole.Database.Repository
{
    public class OyveOtesiNeighborhoodsRepository
    {
        public OyveOtesiNeighborhoodsRepository()
        {
            mongoDbConnector = new MongodbConnector();
            MongodbCollection = mongoDbConnector.GetCollection<OyveOtesiNeighborhoods>("OyveOtesiNeighborhoods");
        }

        public MongodbConnector mongoDbConnector { get; }
        private readonly IMongoCollection<OyveOtesiNeighborhoods> MongodbCollection;



        public async Task<List<OyveOtesiNeighborhoods>> GetAll(int CityId,int DistrictId)
        {
            return await MongodbCollection.Find(x => x.CityId == CityId && x.DistrictId == DistrictId).ToListAsync();
        }


        public async Task<List<OyveOtesiNeighborhoods>> GetAll()
        {
            return await MongodbCollection.Find(x => x.Id != null).ToListAsync();
        }

        public async Task Add(IEnumerable<OyveOtesiNeighborhoods> models)
        {
            if (models != null && models.Any())
                await MongodbCollection.InsertManyAsync(models);
        }
        public async Task Add(OyveOtesiNeighborhoods model)
        {
            await MongodbCollection.InsertOneAsync(model);
        }

        public async Task Delete(string Id)
        {

            await MongodbCollection.DeleteOneAsync(x => x.Id == Id);
        }

        public async Task<List<OyveOtesiNeighborhoods>> GetById(List<string> ids)
        {
            var filter = Builders<OyveOtesiNeighborhoods>.Filter.In(x => x.Id, ids);

            return await MongodbCollection.Find(filter).ToListAsync();
        }
        public async Task<OyveOtesiNeighborhoods> GetById(string ıd)
        {
            return await MongodbCollection.Find(x => x.Id == ıd).SingleOrDefaultAsync();
        }

        public async Task UpdateBulkCityDistrict(List<OyveOtesiNeighborhoods> models)
        {

            foreach (var item in models.Partition(1000))
            {

                var listWrites = new List<WriteModel<OyveOtesiNeighborhoods>>();

                foreach (var model in item)
                {
                    var filter = Builders<OyveOtesiNeighborhoods>.Filter.Eq(x => x.OyveOtesiId, model.OyveOtesiId);
                    var update = Builders<OyveOtesiNeighborhoods>.Update
                        .Set(x => x.CityId, model.CityId)
                        .Set(x => x.DistrictId, model.DistrictId);

                    listWrites.Add(new UpdateManyModel<OyveOtesiNeighborhoods>(filter, update));
                }


                await MongodbCollection.BulkWriteAsync(listWrites);
            }

        }
    }
}
