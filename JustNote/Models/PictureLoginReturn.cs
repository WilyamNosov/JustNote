using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustNote.Models
{
    public class PictureLoginReturn
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("imageArray")]
        public IEnumerable<string> ImageArray { get; set; }
    }
}
