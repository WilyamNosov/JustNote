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

            SharedService sharedService = new SharedService();
            IEnumerable<SharedFolder> sharedFolders = await sharedService.GetAvailableFoldersByFolderId(item.ParentFolderId);

            if (item.ParentFolderId != null)
            {
                foreach (SharedFolder sharedFolder in sharedFolders)
                {
                    await sharedService.CreateNewFolderAccess(sharedFolder.UserId, item.Id, sharedFolder.Role);
                }
            }
        }

        public async Task<Folder> Get(string id)
        {
            if (!String.IsNullOrWhiteSpace(id))
            {
                return await DatabaseData.Folders.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
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

            return await DatabaseData.Folders.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Folder>> GetAllItemsFromFolder(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            FilterDefinition<Folder> filter = FilterService<Folder>.GetFilterByOneParam("ParentFolderId", new ObjectId(id));

            return await DatabaseData.Folders.Find(filter).ToListAsync();
        }

        public async Task Update(string id, Folder item)
        {
            Folder oldFolder = await Get(id);

            item.Id = id;
            item.UserId = oldFolder.UserId;
            item.FolderDate = DateTime.Now;
            item.ParentFolderId = oldFolder.ParentFolderId;

            await DatabaseData.Folders.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(item.Id)), item);
        }

        public async Task Delete(string id)
        {
            NoteService noteService = new NoteService();

            IEnumerable<Note> notesInFolder = await new NoteService().GetAllItemsFromFolder(id);
            IEnumerable<Folder> childFolders = await GetAllItemsFromFolder(id);

            foreach (Note note in notesInFolder)
            {
                await noteService.Delete(note.Id);
            }

            foreach (Folder childFolder in childFolders)
            {
                await Delete(childFolder.Id);
            }

            await DatabaseData.Folders.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }
    }
}
