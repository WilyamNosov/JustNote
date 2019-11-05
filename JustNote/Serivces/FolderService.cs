﻿using JustNote.Datas;
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
        public async Task CreateFolder(Folder folder)
        {
            await Folders.InsertOneAsync(folder);

            AccessService accessService = new AccessService();
            IEnumerable<AvailableFolder> accessFolders = await accessService.GetAvailableFoldersByFolderId(folder.ParentFolderId);

            if (folder.ParentFolderId != null)
                foreach (AvailableFolder accessFolder in accessFolders)
                    await accessService.CreateNewFolderAccess(accessFolder.UserId, folder.Id, accessFolder.Role);
        }
        public async Task<Folder> GetFolder(string id)
        {
            if (!String.IsNullOrWhiteSpace(id))
                return await Folders.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();

            return null;
        }
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

            return await Folders.Find(filter).ToListAsync();
        }
        public async Task UpdateFolder(string id, Folder folder)
        {
            Folder oldFolder = GetFolder(id).GetAwaiter().GetResult();
            folder.Id = id;
            folder.UserId = oldFolder.UserId;
            folder.FolderDate = DateTime.Now;
            folder.ParentFolderId = GetFolder(id).GetAwaiter().GetResult().ParentFolderId;

            await Folders.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(folder.Id)), folder);
        }
        public async Task DeleteFolder(string parentFolderId)
        {
            NoteService noteService = new NoteService();

            IEnumerable<Note> notesInFolder = await noteService.GetAllNotesFromFolder(parentFolderId);
            IEnumerable<Folder> childFolders = await GetAllChildFolder(parentFolderId);

            foreach (Note note in notesInFolder)
                await noteService.DeleteNote(note.Id);
            
            foreach (Folder childFolder in childFolders)
                await DeleteFolder(childFolder.Id);
            

            await Folders.DeleteOneAsync(new BsonDocument("_id", new ObjectId(parentFolderId)));
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
