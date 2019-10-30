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
    public class FolderService : DatabaseData
    {
        public FolderService() : base()
        {

        }
        private IMongoCollection<Folder> Folders
        {
            get { return base.Database.GetCollection<Folder>("folder"); }
        }
        public async Task CreateFolder(Folder folder)
        {


            await Folders.InsertOneAsync(folder);
        }
        public async Task<Folder> GetFolder(string id)
        {
            if (!String.IsNullOrWhiteSpace(id))
                return await Folders.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();

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

            return await Folders.Find(filter).ToListAsync();
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

            return await Folders.Find(filter).ToListAsync();
        }
        public async Task UpdateFolder(string id, string userId, Folder folder)
        {
            folder.Id = id;
            folder.UserId = userId;
            folder.FolderDate = DateTime.Now;
            folder.ParentFolderId = GetFolder(id).GetAwaiter().GetResult().ParentFolderId;

            await Folders.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(folder.Id)), folder);
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
            foreach (Folder childFolder in childFolders)
            {
                await folderService.DeleteFolder(childFolder.Id);
            }

            await Folders.DeleteOneAsync(new BsonDocument("_id", new ObjectId(parentFolderId)));
        }
    }
}
