using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace YskConsole.Database
{
    public class MongoDbIdModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public string Id { get; set; }

        public MongoDbIdModel()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

    }
}
