using JustNote.Datas;
using JustNote.Models;
using JustNotes.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustNote.Serivces
{
    public class FolderService : IDatabaseItemService<Folder>
    {
        public async Task Create(Folder item)
        {
            await DatabaseData.Folders.InsertOneAsync(item);
        }

        public async Task<Folder> Get(string id)
        {
            if (!String.IsNullOrWhiteSpace(id))
            {
                var result = await DatabaseData.Folders.Find(new BsonDocument("LocalId", id)).FirstOrDefaultAsync();

                return result;
            }

            return null;
        }

        public async Task<IEnumerable<Folder>> GetAllItems(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            FilterDefinition<Folder> filter = FilterService<Folder>.GetFilterByOneParam("UserId", new ObjectId(id));
            var result = await DatabaseData.Folders.Find(filter).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<Folder>> GetAllItemsFromFolder(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            FilterDefinition<Folder> filter = FilterService<Folder>.GetFilterByOneParam("ParentFolderId", new ObjectId(id));
            var result = await DatabaseData.Folders.Find(filter).ToListAsync();

            return result;
        }
        public async Task<IEnumerable<Folder>> GetAllItemsFromDatabase()
        {
            var result = await DatabaseData.Folders.Find(new BsonDocument()).ToListAsync(); 

            return result;
        }

        public async Task Update(string id, Folder item)
        {
            Folder oldFolder = await Get(id);

            oldFolder.FolderName = item.FolderName;
            oldFolder.FolderDate = DateTime.Now;

            await DatabaseData.Folders.ReplaceOneAsync(new BsonDocument("LocalId", oldFolder.LocalId), oldFolder);
        }

        public async Task Delete(string id)
        {
            await DatabaseData.Notes.DeleteManyAsync(new BsonDocument("FolderId", id));
            await DatabaseData.Folders.DeleteOneAsync(new BsonDocument("LocalId", id));
        }
    }
}
