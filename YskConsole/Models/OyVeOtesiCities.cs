using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YskConsole.Database;

namespace YskConsole.Models
{
    public class OyveOtesiCities: MongoDbIdModel
    {
        [JsonProperty("id")]
        public int OyveOtesiId { get; set; }
        public string name { get; set; }
    }

    public class OyveOtesiDistricts : MongoDbIdModel
    {
        [JsonProperty("id")]
        public int OyveOtesiId { get; set; }
        public int? CityId { get; set; }
        public string name { get; set; }
    }

    public class OyveOtesiNeighborhoods : MongoDbIdModel
    {
        [JsonProperty("id")]
        public int OyveOtesiId { get; set; }
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public string name { get; set; }
    }
    public class OyveItesiOylar : MongoDbIdModel
    {
        public string TcKimlikNo { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }

        public int DistrictId { get; set; }
        public string DistrictName { get; set; }

        public int NeighborhoodId { get; set; }
        public string NeighborhoodName { get; set; }

        public int ballot_box_number { get; set; }
        public CmResult cm_result { get; set; }
        public MvResult mv_result { get; set; }
        public string school_name { get; set; }
    }


    public class MvResult
    {
        public string image_url { get; set; }
        public int submission_id { get; set; }
        public int total_vote { get; set; }
        public Votes votes { get; set; }
    }

    public class CmResult
    {
        public string image_url { get; set; }
        public int submission_id { get; set; }
        public int total_vote { get; set; }
        public Votes votes { get; set; }
    }

    public class Votes
    {
        [BsonElement("_1")]
        [JsonProperty("1")]
        public int _1 { get; set; }

        [BsonElement("_2")]
        [JsonProperty("2")]
        public int _2 { get; set; }

        [BsonElement("_3")]
        [JsonProperty("3")]
        public int _3 { get; set; }

        [BsonElement("_4")]
        [JsonProperty("4")]
        public int _4 { get; set; }



        [BsonElement("_10")]
        [JsonProperty("10")]
        public int _10 { get; set; }


        [BsonElement("_11")]
        [JsonProperty("11")]
        public int _11 { get; set; }

        [BsonElement("_12")]
        [JsonProperty("12")]
        public int _12 { get; set; }


        [BsonElement("_17")]
        [JsonProperty("17")]
        public int _17 { get; set; }


        [BsonElement("_21")]
        [JsonProperty("21")]
        public int _21 { get; set; }

        [BsonElement("_22")]
        [JsonProperty("22")]
        public int _22 { get; set; }

        [BsonElement("_24")]
        [JsonProperty("24")]
        public int _24 { get; set; }

        [BsonElement("_7")]
        [JsonProperty("7")]
        public int _7 { get; set; }

        [BsonElement("_8")]
        [JsonProperty("8")]
        public int _8 { get; set; }

        [BsonElement("_9")]
        [JsonProperty("9")]
        public int _9 { get; set; }

        [BsonElement("_13")]
        [JsonProperty("13")]
        public int? _13 { get; set; }

        [BsonElement("_16")]
        [JsonProperty("16")]
        public int? _16 { get; set; }

        [BsonElement("_19")]
        [JsonProperty("19")]
        public int? _19 { get; set; }

        [BsonElement("_20")]
        [JsonProperty("20")]
        public int? _20 { get; set; }

        [BsonElement("_5")]
        [JsonProperty("5")]
        public int? _5 { get; set; }
    }
     
}
