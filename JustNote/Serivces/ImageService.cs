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
    public class ImageService : IDatabaseItemService<Image>
    {
        public async Task Create(Image item)
        {
            await DatabaseData.Image.InsertOneAsync(item);
        }

        public async Task<Image> Get(string id)
        {
            var result = await DatabaseData.Image.Find(new BsonDocument("LocalId", id)).FirstOrDefaultAsync();
            return result;
        }

        public async Task<IEnumerable<Image>> GetAllItems(string id)
        {
            var result = await DatabaseData.Image.Find(new BsonDocument("UserId", id)).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Image>> GetAllItemsFromDatabase()
        {
            var resullt = await DatabaseData.Image.Find(new BsonDocument()).ToListAsync();
            return resullt;
        }

        public Task<IEnumerable<Image>> GetAllItemsFromFolder(string id)
        {
            throw new NotImplementedException();
        }

        public Task Update(string id, Image item)
        {
            throw new NotImplementedException();
        }

        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
