using JustNote.Attributes;
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
    public class NoteService : IDatabaseItemService<Note>
    {
        public async Task Create(Note item)
        {
            await DatabaseData.Notes.InsertOneAsync(item);

            if (item.FolderId != null)
            {
                var sharedFolders = await DatabaseData.SharedFolders.Find(new BsonDocument("FolderId", item.FolderId)).ToListAsync();
                var sharedNotes = new List<SharedNote>();

                foreach (var sharedFolder in sharedFolders)
                {
                    sharedNotes.Add(new SharedNote() { NoteId = item.LocalId, UserId = sharedFolder.UserId, Role = sharedFolder.Role });
                }

                if (sharedNotes.Count > 0) 
                {
                    await DatabaseData.SharedNotes.InsertManyAsync(sharedNotes);
                }
            }
        }

        public async Task CreateManyItems(List<Note> items)
        {
            await DatabaseData.Notes.InsertManyAsync(items);
        }

        public async Task<Note> Get(string id)
        {
            return await DatabaseData.Notes.Find(new BsonDocument("LocalId", id)).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Note>> GetAllItems(string id)
        {
            var filter = FilterService<Note>.GetFilterByOneParam("UserId", new ObjectId(id));
            var result = await DatabaseData.Notes.Find(filter).ToListAsync();

            return result;
        }
        public async Task<IEnumerable<Note>> GetAllItemsFromFolder(string id)
        {
            var filter = FilterService<Note>.GetFilterByOneParam("FolderId", id);
            var result = await DatabaseData.Notes.Find(filter).ToListAsync();

            return result;
        }
        public async Task<IEnumerable<Note>> GetAllItemsFromDatabase()
        {
            var result = await DatabaseData.Notes.Find(new BsonDocument()).ToListAsync();

            return result;
        }

        public async Task Update(string id, Note item)
        {
            Note oldNote = await Get(id);

            item.Id = oldNote.Id;
            item.UserId = oldNote.UserId;
            item.NoteDate = DateTime.Now;
            item.FolderId = oldNote.FolderId;
            item.LocalId = oldNote.LocalId;

            await DatabaseData.Notes.ReplaceOneAsync(new BsonDocument("LocalId", id), item);
        }

        public async Task Delete(string id)
        {
            await DatabaseData.Pictires.DeleteManyAsync(new BsonDocument("NoteId", id));
            await DatabaseData.SharedNotes.DeleteManyAsync(new BsonDocument("NoteId", id));
            await DatabaseData.Notes.DeleteOneAsync(new BsonDocument("LocalId", id));
        }
    }
}