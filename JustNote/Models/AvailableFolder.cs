using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustNote.Models
{
    public class AvailableFolder
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("id")]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("folderId")]
        public string FolderId { get; set; }
        [JsonProperty("role")]
        public string Role { get; set; }
    }
}
