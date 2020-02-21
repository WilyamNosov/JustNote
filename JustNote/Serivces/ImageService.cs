using JustNote.Datas;
using JustNote.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustNote.Serivces
{
    public class ImageService : IDatabaseItemService<Picture>
    {
        public async Task Create(Picture item)
        {
            await DatabaseData.Pictires.InsertOneAsync(item);
        }

        public async Task CreateManyItems(List<Picture> items)
        {
            await DatabaseData.Pictires.InsertManyAsync(items);
        }

        public async Task<Picture> Get(string id)
        {
            var result = await DatabaseData.Pictires.Find(new BsonDocument("LocalId", id)).FirstOrDefaultAsync();
            return result;
        }

        public async Task<IEnumerable<Picture>> GetAllItems(string id)
        {
            var result = await DatabaseData.Pictires.Find(new BsonDocument("UserId", id)).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Picture>> GetAllItemsFromDatabase()
        {
            var resullt = await DatabaseData.Pictires.Find(new BsonDocument()).ToListAsync();
            return resullt;
        }

        public async Task<IEnumerable<Picture>> GetAllItemsFromFolder(string id)
        {
            //FromNote
            var result = await DatabaseData.Pictires.Find(new BsonDocument("NoteId", id)).ToListAsync();
            return result;
        }

        public Task Update(string id, Picture item)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(string id)
        {
            await DatabaseData.Pictires.DeleteManyAsync(new BsonDocument("NoteId", id));
        }
    }
}
