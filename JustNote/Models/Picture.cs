using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustNote.Models
{
    public class Picture
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("imageCode")]
        public string ImageCode { get; set; }
        [JsonProperty("noteId")]
        public string NoteId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}
