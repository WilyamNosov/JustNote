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
                SharedService sharedService = new SharedService();
                IEnumerable<SharedFolder> sharedFolders = await sharedService.GetAvailableFoldersByFolderId(item.FolderId);

                foreach (SharedFolder sharedFolder in sharedFolders)
                {
                    await sharedService.CreateNewNoteAccess(sharedFolder.UserId, item.Id, sharedFolder.Role);
                }
            }
        }

        public async Task<Note> Get(string id)
        {
            return await DatabaseData.Notes.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Note>> GetAllItems(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            FilterDefinition<Note> filter = FilterService<Note>.GetFilterByOneParam("UserId", new ObjectId(id));

            return await DatabaseData.Notes.Find(filter).ToListAsync();
        }
        public async Task<IEnumerable<Note>> GetAllItemsFromFolder(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            FilterDefinition<Note> filter = FilterService<Note>.GetFilterByOneParam("FolderId", new ObjectId(id));

            return await DatabaseData.Notes.Find(filter).ToListAsync();
        }

        public async Task Update(string id, Note item)
        {
            Note oldNote = Get(id).GetAwaiter().GetResult();
            item.Id = id;
            item.UserId = oldNote.UserId;
            item.NoteDate = DateTime.Now;
            item.FolderId = oldNote.FolderId;

            await DatabaseData.Notes.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(id)), item);
        }

        public async Task Delete(string id)
        {
            await DatabaseData.Notes.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }
    }
}