using MongoDB.Driver;
using YskConsole.Models;

namespace YskConsole.Database
{
    public class MilletVekiliSecimBilgileriRepository
    {
        public MilletVekiliSecimBilgileriRepository()
        {
            mongoDbConnector = new MongodbConnector();
            MongodbCollection = mongoDbConnector.GetCollection<MilletVekiliSecimBilgileri>("MilletVekiliSecimBilgileri");
        }

        public MongodbConnector mongoDbConnector { get; }
        private readonly IMongoCollection<MilletVekiliSecimBilgileri> MongodbCollection;


        public async Task<List<MilletVekiliSecimBilgileri>> GetAllIlIlceSandik(string il, string ilce, string sandikno)
        {
            return await MongodbCollection.Find(x => x.Il == il && x.Ilce == ilce && x.SandikNo == sandikno).ToListAsync();
        }
        public async Task<List<MilletVekiliSecimBilgileri>> GetAll()
        {
            return await MongodbCollection.Find(x => x.Id != null).ToListAsync();
        }

        public async Task Add(IEnumerable<MilletVekiliSecimBilgileri> models)
        {
            if(models.Any())
            await MongodbCollection.InsertManyAsync(models);
        }
        public async Task Add(MilletVekiliSecimBilgileri model)
        {
            await MongodbCollection.InsertOneAsync(model);
        }

        public async Task Delete(string Id)
        {

            await MongodbCollection.DeleteOneAsync(x => x.Id == Id);
        }

        public async Task<List<MilletVekiliSecimBilgileri>> GetById(List<string> ids)
        {
            var filter = Builders<MilletVekiliSecimBilgileri>.Filter.In(x => x.Id, ids);

            return await MongodbCollection.Find(filter).ToListAsync();
        }
        public async Task<MilletVekiliSecimBilgileri> GetById(string ıd)
        {
            return await MongodbCollection.Find(x => x.Id == ıd).SingleOrDefaultAsync();
        }

    }
}
