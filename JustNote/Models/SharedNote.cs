using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustNote.Models
{
    public class SharedNote
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("id")]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("userId")]
        public string UserId { get; set; }
<<<<<<< HEAD:JustNote/Models/AvailableNote.cs
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("noteId")]
=======
>>>>>>> DatabaseData:JustNote/Models/SharedNote.cs
        public string NoteId { get; set; }
        [JsonProperty("role")]
        public string Role { get; set; }
    }
}
