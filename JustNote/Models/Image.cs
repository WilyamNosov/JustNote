using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustNote.Models
{
    public class Image
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("imageName")]
        public string ImageName { get; set; }
        [JsonProperty("imageCode")]
        public string ImageCode { get; set; }
        [JsonProperty("localId")]
        public string LocalId { get; set; }
        [JsonProperty("noteId")]
        public string NoteId { get; set; }
        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}
