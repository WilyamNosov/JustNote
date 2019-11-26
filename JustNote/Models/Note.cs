using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustNote.Models
{
    public class Note
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("noteDate")]
        public DateTime NoteDate { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("folderId")]
        public string FolderId { get; set; }
        [JsonProperty("localId")]
        public string LocalId { get; set; }
    }
}
