using JustNote.Datas;
using JustNote.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustNote.Serivces
{
    public class FolderService
    {
        public async Task CreateFolder(Folder folder)
        {
            await DatabaseData.Folders.InsertOneAsync(folder);

<<<<<<< Updated upstream
            AccessService accessService = new AccessService();
            IEnumerable<AvailableFolder> accessFolders = accessService.GetAvailableFoldersByFolderId(folder.ParentFolderId).GetAwaiter().GetResult();
            
            foreach (AvailableFolder accessFolder in accessFolders)
            {
                accessService.CreateNewFolderAccess(accessFolder.UserId, folder.Id, accessFolder.Role).GetAwaiter().GetResult();
=======
            SharedService accessService = new SharedService();
            IEnumerable<SharedFolder> accessFolders = await accessService.GetAvailableFoldersByFolderId(folder.ParentFolderId);

            if (folder.ParentFolderId != null)
            {
                foreach (SharedFolder accessFolder in accessFolders)
                {
                    await accessService.CreateNewFolderAccess(accessFolder.UserId, folder.Id, accessFolder.Role);
                }
>>>>>>> Stashed changes
            }
        }
        public async Task<Folder> GetFolder(string id)
        {
            if (!String.IsNullOrWhiteSpace(id))
                return await DatabaseData.Folders.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();

            return null;
        }
        public async Task<IEnumerable<Folder>> GetAllUserFolders(string userId)
        {
            FilterDefinitionBuilder<Folder> builder = new FilterDefinitionBuilder<Folder>();
            FilterDefinition<Folder> filter = builder.Empty;

            if (!String.IsNullOrWhiteSpace(userId))
            {
                filter = filter & builder.Eq("UserId", new ObjectId(userId));
            }

<<<<<<< Updated upstream
            return await Folders.Find(filter).ToListAsync();
=======
            FilterDefinition<Folder> filter = FilterService<Folder>.GetFilterByOneParam("UserId", new ObjectId(userId));

            return await DatabaseData.Folders.Find(filter).ToListAsync();
>>>>>>> Stashed changes
        }
        public async Task<IEnumerable<Folder>> GetAllChildFolder(string folderId)
        {
            FilterDefinitionBuilder<Folder> builder = new FilterDefinitionBuilder<Folder>();
            FilterDefinition<Folder> filter = builder.Empty;

            if (!String.IsNullOrWhiteSpace(folderId))
            {
                filter = filter & builder.Eq("ParentFolderId", new ObjectId(folderId));
            }

            return await Folders.Find(filter).ToListAsync();
        }
        public async Task<IEnumerable<Folder>> GetFolderBySearchString(string searchString)
        {
            FilterDefinitionBuilder<Folder> builder = new FilterDefinitionBuilder<Folder>();
            FilterDefinition<Folder> filter = builder.Empty;

            if (!String.IsNullOrWhiteSpace(searchString))
            {
                filter = filter & builder.Gte("Name", searchString);
            }

            return await DatabaseData.Folders.Find(filter).ToListAsync();
        }
        public async Task UpdateFolder(string id, string userId, Folder folder)
        {
            folder.Id = id;
            folder.UserId = userId;
            folder.FolderDate = DateTime.Now;
            folder.ParentFolderId = GetFolder(id).GetAwaiter().GetResult().ParentFolderId;

            await DatabaseData.Folders.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(folder.Id)), folder);
        }
        public async Task DeleteFolder(string parentFolderId)
        {
            NoteService noteService = new NoteService();
            FolderService folderService = new FolderService();
            IEnumerable<Note> notesInFolder = await noteService.GetAllNotesFromFolder(parentFolderId);
            IEnumerable<Folder> childFolders = await folderService.GetAllChildFolder(parentFolderId);

            foreach (Note note in notesInFolder)
            {
                await noteService.DeleteNote(note.Id);
            }
<<<<<<< Updated upstream
            foreach (Folder childFolder in childFolders)
            {
                await folderService.DeleteFolder(childFolder.Id);
=======

            foreach (Folder childFolder in childFolders)
            {
                await DeleteFolder(childFolder.Id);
>>>>>>> Stashed changes
            }

            await DatabaseData.Folders.DeleteOneAsync(new BsonDocument("_id", new ObjectId(parentFolderId)));
        }
    }
}
