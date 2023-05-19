using MongoDB.Driver;
using YskConsole.Models;
using YskConsole.Utility;

namespace YskConsole.Database
{
    
    public class sqlRepository
    {
        public sqlRepository()
        {
            mongoDbConnector = new MongodbConnector();
            MongodbCollection = mongoDbConnector.GetCollection<sql>("sql");
        }

        public MongodbConnector mongoDbConnector { get; }
        private readonly IMongoCollection<sql> MongodbCollection;


        public async Task<List<sql>> Get(string Field_11, string Field_12, string Field_13, string Field_13_1)
        {
            return await MongodbCollection.Find(x => x.Field_11 == Field_11 && x.Field_12 == Field_12 && (x.Field_13 == Field_13 || x.Field_13 == Field_13_1)).ToListAsync();
        }


        public async Task<List<sql>> GetAll()
        {
            return await MongodbCollection.Find(x => x.Id != null).ToListAsync();
        }

        public async Task Add(List<sql> models)
        {
            if (models.Any())
                foreach (var item in models.Partition(10000))
                {
                    await MongodbCollection.InsertManyAsync(item);
                }

        }
        public async Task Add(sql model)
        {
            await MongodbCollection.InsertOneAsync(model);
        }

        public async Task Delete(string Id)
        {

            await MongodbCollection.DeleteOneAsync(x => x.Id == Id);
        }

        public async Task<List<sql>> GetById(List<string> ids)
        {
            var filter = Builders<sql>.Filter.In(x => x.Id, ids);

            return await MongodbCollection.Find(filter).ToListAsync();
        }
        public async Task<sql> GetById(string ıd)
        {
            return await MongodbCollection.Find(x => x.Id == ıd).SingleOrDefaultAsync();
        }

    }
}
