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
    public class SharedService
    {
        //private IDatabaseItemService<Folder> _folderService;
        //private IDatabaseItemService<Note> _noteService; 
        private FolderService _folderService = new FolderService();
        private NoteService _noteService = new NoteService();

        //public SharedService(IDatabaseItemService<Folder> folderService, IDatabaseItemService<Note> noteService)
        //{
        //    _folderService = folderService;
        //    _noteService = noteService;
        //}

        public async Task<IEnumerable<SharedFolder>> GetAvailableFoldersByFolderId(string folderId)
        {
            if (String.IsNullOrWhiteSpace(folderId))
            {
                return null;
            }

            FilterDefinition<SharedFolder> filter = FilterService<SharedFolder>.GetFilterByOneParam("FolderId", new ObjectId(folderId));

            return await DatabaseData.SharedFolders.Find(filter).ToListAsync();
        }
        public async Task<SharedFolder> GetSharedFolder(string folderId, string userId)
        {
            List<string> paramList = new List<string>() { "FolderId", "UserId" };
            List<object> valueList = new List<object>() { new ObjectId(folderId), new ObjectId(userId) };

            FilterDefinition<SharedFolder> filter = FilterService<SharedFolder>.GetFilterByTwoParam(paramList, valueList);

            if (await DatabaseData.SharedFolders.Find(filter).FirstOrDefaultAsync() != null)
            {
                return await DatabaseData.SharedFolders.Find(filter).FirstOrDefaultAsync();
            }

            return null;
        }

        public async Task<SharedNote> GetSharedNote(string noteId, string userId)
        {
            List<string> paramList = new List<string>() { "NoteId", "UserId" };
            List<object> valueList = new List<object>() { new ObjectId(noteId), new ObjectId(userId) };

            FilterDefinition<SharedNote> filter = FilterService<SharedNote>.GetFilterByTwoParam(paramList, valueList);

            if (await DatabaseData.SharedNotes.Find(filter).FirstOrDefaultAsync() != null)
            {
                return await DatabaseData.SharedNotes.Find(filter).FirstOrDefaultAsync();
            }

            return null;
        }

        public async Task CreateNewFolderAccess(string userId, string folderId, string role)
        {
            IEnumerable<Note> notesInFolder = await _noteService.GetAllItemsFromFolder(folderId);

            foreach (Note note in notesInFolder)
            {
                await CreateNewNoteAccess(userId, note.Id, role);
            }

            SharedFolder sharedFolder = await GetSharedFolder(folderId, userId);

            if (sharedFolder == null)
            {
                sharedFolder = new SharedFolder()
                {
                    UserId = userId,
                    FolderId = folderId,
                    Role = role
                };

                await DatabaseData.SharedFolders.InsertOneAsync(sharedFolder);
            }
            else if (sharedFolder.Role != role)
            {
                sharedFolder.Role = role;
                await UpdateFolderAccess(sharedFolder);
            }
        }
        public async Task CreateNewNoteAccess(string userId, string noteId, string role)
        {
            SharedNote sharedNote = await GetSharedNote(noteId, userId);

            if (sharedNote == null)
            {
                sharedNote = new SharedNote()
                {
                    UserId = userId,
                    NoteId = noteId,
                    Role = role
                };
                await DatabaseData.SharedNotes.InsertOneAsync(sharedNote);
            }

            else if (sharedNote.Role != role)
            {
                sharedNote.Role = role;
                await UpdateNoteAccess(sharedNote);
            }
        }

        public async Task<IEnumerable<Object>> GetSharedItems(string userId)
        {
            if (String.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            FilterDefinition<SharedFolder> filterFolder = FilterService<SharedFolder>.GetFilterByOneParam("UserId", new ObjectId(userId));
            FilterDefinition<SharedNote> filterNote = FilterService<SharedNote>.GetFilterByOneParam("UserId", new ObjectId(userId));
            List<Object> result = new List<Object>();
            List<string> folderIds = new List<string>();

            List<SharedFolder> AvailableFolerIds = await DatabaseData.SharedFolders.Find(filterFolder).ToListAsync();
            List<SharedNote> AvailableNoteIds = await DatabaseData.SharedNotes.Find(filterNote).ToListAsync();

            foreach (SharedFolder AvailableFolderId in AvailableFolerIds)
            {
                folderIds.Add(AvailableFolderId.FolderId);
            }

            foreach (SharedFolder AvailableFolderId in AvailableFolerIds)
            {
                Folder folder = await _folderService.Get(AvailableFolderId.FolderId);

                if (folder != null && !folderIds.Contains(folder.ParentFolderId))
                {
                    JObject addToResult = JObject.FromObject(folder);
                    addToResult.Add("Role", AvailableFolderId.Role);
                    result.Add(addToResult);
                }
            }

            foreach (SharedNote availableNote in AvailableNoteIds)
            {
                Note note = await _noteService.Get(availableNote.NoteId);

                if (note != null && !folderIds.Contains(note.FolderId))
                {
                    JObject addToResult = JObject.FromObject(await _noteService.Get(availableNote.NoteId));
                    addToResult.Add("Role", availableNote.Role);
                    result.Add(addToResult);
                }
            }

            return result;
        }
        public async Task<IEnumerable<Object>> GetAvailableItemsFromFolder(string folderId, string userId)
        {
            IEnumerable<Folder> folders = await _folderService.GetAllItemsFromFolder(folderId);
            IEnumerable<Note> notes = await _noteService.GetAllItemsFromFolder(folderId);
            List<Object> result = new List<Object>();

            foreach (Folder folder in folders)
            {
                string role = GetSharedFolder(folder.Id, userId).GetAwaiter().GetResult().Role;
                JObject addToResult = JObject.FromObject(folder);
                addToResult.Add("Role", role);
                result.Add(addToResult);
            }
            foreach (Note note in notes)
            {
                string role = GetSharedNote(note.Id, userId).GetAwaiter().GetResult().Role;
                JObject addToResult = JObject.FromObject(note);
                addToResult.Add("Role", role);
                result.Add(addToResult);
            }

            return result;
        }
        public async Task UpdateNoteAccess(SharedNote availableNote)
        {
            await DatabaseData.SharedNotes.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(availableNote.Id)), availableNote);
        }
        public async Task UpdateFolderAccess(SharedFolder availableFolder)
        {
            await DatabaseData.SharedFolders.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(availableFolder.Id)), availableFolder);
        }
    }
}