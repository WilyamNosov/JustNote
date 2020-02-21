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
        private IDatabaseItemService<Note> _noteService;

        public FolderService(IDatabaseItemService<Note> noteService)
        {
            _noteService = noteService;
        }

        public async Task Create(Folder item)
        {
            await DatabaseData.Folders.InsertOneAsync(item);
        }

<<<<<<< HEAD
            AccessService accessService = new AccessService();
            IEnumerable<AvailableFolder> accessFolders = await accessService.GetAvailableFoldersByFolderId(folder.ParentFolderId);

            if (folder.ParentFolderId != null)
                foreach (AvailableFolder accessFolder in accessFolders)
                    await accessService.CreateNewFolderAccess(accessFolder.UserId, folder.Id, accessFolder.Role);
=======
        public async Task CreateManyItems(List<Folder> items)
        {
             await DatabaseData.Folders.InsertManyAsync(items);
>>>>>>> DatabaseData
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
<<<<<<< HEAD
        public async Task<IEnumerable<Folder>> GetAllUserFolders(string userId)
        {
            if (String.IsNullOrWhiteSpace(userId))
                return null;

            FilterDefinition<Folder> filter = FilterService<Folder>.GetFilterByOneParam("UserId", new ObjectId(userId));
            
            return await Folders.Find(filter).ToListAsync();
        }
        public async Task<IEnumerable<Folder>> GetAllChildFolder(string folderId)
        {
            if (String.IsNullOrWhiteSpace(folderId))
                return null;

            FilterDefinition<Folder> filter = FilterService<Folder>.GetFilterByOneParam("ParentFolderId", new ObjectId(folderId));
=======

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
>>>>>>> DatabaseData

            return result;
        }
<<<<<<< HEAD
        public async Task UpdateFolder(string id, Folder folder)
        {
            Folder oldFolder = GetFolder(id).GetAwaiter().GetResult();
            folder.Id = id;
            folder.UserId = oldFolder.UserId;
            folder.FolderDate = DateTime.Now;
            folder.ParentFolderId = GetFolder(id).GetAwaiter().GetResult().ParentFolderId;
=======

        public async Task Update(string id, Folder item)
        {
            Folder oldFolder = await Get(id);

            oldFolder.FolderName = item.FolderName;
            oldFolder.FolderDate = DateTime.Now;
>>>>>>> DatabaseData

            await DatabaseData.Folders.ReplaceOneAsync(new BsonDocument("LocalId", oldFolder.LocalId), oldFolder);
        }

        public async Task Delete(string id)
        {
<<<<<<< HEAD
            NoteService noteService = new NoteService();

            IEnumerable<Note> notesInFolder = await noteService.GetAllNotesFromFolder(parentFolderId);
            IEnumerable<Folder> childFolders = await GetAllChildFolder(parentFolderId);

            foreach (Note note in notesInFolder)
                await noteService.DeleteNote(note.Id);
            
            foreach (Folder childFolder in childFolders)
                await DeleteFolder(childFolder.Id);
            
=======
            var notes = await _noteService.GetAllItemsFromFolder(id);

            foreach(var note in notes)
            {
                await DatabaseData.SharedNotes.DeleteManyAsync(new BsonDocument("NoteId", note.LocalId));
            }
>>>>>>> DatabaseData

            await DatabaseData.SharedFolders.DeleteManyAsync(new BsonDocument("FolderId", id));
            await DatabaseData.Notes.DeleteManyAsync(new BsonDocument("FolderId", id));
            await DatabaseData.Folders.DeleteOneAsync(new BsonDocument("LocalId", id));
        }
    }
    /*public async Task<IEnumerable<Folder>> GetFolderBySearchString(string searchString)
    {
        FilterDefinitionBuilder<Folder> builder = new FilterDefinitionBuilder<Folder>();
        FilterDefinition<Folder> filter = builder.Empty;

        if (!String.IsNullOrWhiteSpace(searchString))
        {
            filter = filter & builder.Gte("Name", searchString);
        }

        return await Folders.Find(filter).ToListAsync();
    }*/
}
