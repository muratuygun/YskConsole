using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YskConsole.Database
{
    public class MongodbConnector
    {
        public static string DatabaseName = "ysk";
        public static string ConnectionString = "mongodb://murat:Xum1tnx51@192.168.1.118/";
        private readonly IMongoDatabase mongoDatabase;
        public MongodbConnector()
        {

            var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));
            clientSettings.ConnectTimeout = TimeSpan.FromSeconds(300);
            clientSettings.SocketTimeout = TimeSpan.FromSeconds(300);
            clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(300);
            clientSettings.HeartbeatTimeout = TimeSpan.FromSeconds(300);
            clientSettings.WaitQueueTimeout = TimeSpan.FromSeconds(300);

            clientSettings.DirectConnection = true;

            clientSettings.RetryReads = true; clientSettings.RetryWrites = true;
            clientSettings.ClusterConfigurator = cb =>
            {
                cb.Subscribe<CommandStartedEvent>(e =>
                {
                    //logger.LogInformation($"{e.CommandName} - {e.Command.ToJson()}");
                });
            };
            var mongoClient = new MongoClient(clientSettings);
            //mongoClient.StartSession();


            mongoDatabase = mongoClient.GetDatabase(DatabaseName);
        }
        public IMongoDatabase GetDatabase()
        {
            return mongoDatabase;
        }

        public IMongoCollection<TDocument> GetCollection<TDocument>(string name)
        {
            return mongoDatabase.GetCollection<TDocument>(name);
        }
    }
}
