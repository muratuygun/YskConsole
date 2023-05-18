using MongoDB.Driver;
using YskConsole.Models;
using YskConsole.Utility;

namespace YskConsole.Database
{
    public class ResultCompareModelRepository
    {
        public ResultCompareModelRepository()
        {
            mongoDbConnector = new MongodbConnector();
            MongodbCollection = mongoDbConnector.GetCollection<ResultCompareModel>("ResultCompareModel");
        }

        public MongodbConnector mongoDbConnector { get; }
        private readonly IMongoCollection<ResultCompareModel> MongodbCollection;



        public async Task<List<ResultCompareModel>> GetAll(List<int> NeighborhoodIds)
        {
            var filter = new List<FilterDefinition<ResultCompareModel>>();

            foreach (var category in NeighborhoodIds)
            {
                filter.Add(Builders<ResultCompareModel>.Filter.Eq("NeighborhoodId", category));
            }



            return await MongodbCollection.Find(Builders<ResultCompareModel>.Filter.Or(filter)).ToListAsync();
        }

        public async Task<List<ResultCompareModel>> GetAll()
        {
            return await MongodbCollection.Find(x => x.Id != null).ToListAsync();
        }

        public async Task Add(List<ResultCompareModel> models)
        {
            if (models.Any())
                foreach (var item in models.Partition(10000))
                {
                    await MongodbCollection.InsertManyAsync(item);
                }
                    
        }
        public async Task Add(ResultCompareModel model)
        {
            await MongodbCollection.InsertOneAsync(model);
        }

        public async Task Delete(string Id)
        {

            await MongodbCollection.DeleteOneAsync(x => x.Id == Id);
        }

        public async Task<List<ResultCompareModel>> GetById(List<string> ids)
        {
            var filter = Builders<ResultCompareModel>.Filter.In(x => x.Id, ids);

            return await MongodbCollection.Find(filter).ToListAsync();
        }
        public async Task<ResultCompareModel> GetById(string ıd)
        {
            return await MongodbCollection.Find(x => x.Id == ıd).SingleOrDefaultAsync();
        }

    }
}
