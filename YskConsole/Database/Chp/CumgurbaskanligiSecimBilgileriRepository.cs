using MongoDB.Driver;
using YskConsole.Models;

namespace YskConsole.Database.Repository
{
    public class CumgurbaskanligiSecimBilgileriRepository
    {
        public CumgurbaskanligiSecimBilgileriRepository()
        {
            mongoDbConnector = new MongodbConnector();
            MongodbCollection = mongoDbConnector.GetCollection<CumgurbaskanligiSecimBilgileri>("CumgurbaskanligiSecimBilgileri");
        }

        public MongodbConnector mongoDbConnector { get; }
        private readonly IMongoCollection<CumgurbaskanligiSecimBilgileri> MongodbCollection;

        public async Task<CumgurbaskanligiSecimBilgileri> GetFirstIlIlceSandik(string il, string ilce, string sandikno)
        {
            return await MongodbCollection.Find(x => x.Il == il && x.Ilce == ilce && x.SandikNo == sandikno).FirstOrDefaultAsync();
        }

        public async Task<List<CumgurbaskanligiSecimBilgileri>> GetAllIlIlceSandik(string il,string ilce,string sandikno)
        {
            return await MongodbCollection.Find(x => x.Il == il && x.Ilce == ilce && x.SandikNo == sandikno).ToListAsync();
        }

        public async Task<List<CumgurbaskanligiSecimBilgileri>> GetAll()
        {
            return await MongodbCollection.Find(x => x.Id != null).ToListAsync();
        }

        public async Task Add(IEnumerable<CumgurbaskanligiSecimBilgileri> models)
        {
            if (models.Any())
                await MongodbCollection.InsertManyAsync(models);
        }
        public async Task Add(CumgurbaskanligiSecimBilgileri model)
        {
            await MongodbCollection.InsertOneAsync(model);
        }

        public async Task Delete(string Id)
        {

            await MongodbCollection.DeleteOneAsync(x => x.Id == Id);
        }

        public async Task<List<CumgurbaskanligiSecimBilgileri>> GetById(List<string> ids)
        {
            var filter = Builders<CumgurbaskanligiSecimBilgileri>.Filter.In(x => x.Id, ids);

            return await MongodbCollection.Find(filter).ToListAsync();
        }
        public async Task<CumgurbaskanligiSecimBilgileri> GetById(string ıd)
        {
            return await MongodbCollection.Find(x => x.Id == ıd).SingleOrDefaultAsync();
        }

    }
}
