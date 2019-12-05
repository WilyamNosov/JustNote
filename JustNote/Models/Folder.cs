using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustNote.Models
{
    public class Folder
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("folderName")]
        public string FolderName { get; set; }
        [JsonProperty("folderDate")]
        public DateTime FolderDate { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("localId")]
        public string LocalId { get; set; }
        [BsonIgnoreIfNull]
        public string Role { get; set; }
    }
}
