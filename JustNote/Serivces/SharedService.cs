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
        private FolderService FolderService = new FolderService();
        private NoteService NoteService = new NoteService();

        public async Task<IEnumerable<SharedFolder>> GetAvailableFoldersByFolderId(string folderId)
        {
            if (String.IsNullOrWhiteSpace(folderId))
                return null;

            FilterDefinition<SharedFolder> filter = FilterService<SharedFolder>.GetFilterByOneParam("FolderId", new ObjectId(folderId));

            return await DatabaseData.SharedFolders.Find(filter).ToListAsync();
        }
        public async Task<SharedFolder> GetAvailableFolder(string folderId, string userId)
        {
            List<string> paramList = new List<string>() { "FolderId", "UserId" };
            List<object> valueList = new List<object>() { new ObjectId(folderId), new ObjectId(userId) };

            FilterDefinition<SharedFolder> filter = FilterService<SharedFolder>.GetFilterByTwoParam(paramList, valueList);

            if (await DatabaseData.SharedFolders.Find(filter).FirstOrDefaultAsync() != null)
                return await DatabaseData.SharedFolders.Find(filter).FirstOrDefaultAsync();

            return null;
        }

        public async Task<SharedNote> GetAvailableNote(string noteId, string userId)
        {
            List<string> paramList = new List<string>() { "NoteId", "UserId" };
            List<object> valueList = new List<object>() { new ObjectId(noteId), new ObjectId(userId) };

            FilterDefinition<SharedNote> filter = FilterService<SharedNote>.GetFilterByTwoParam(paramList, valueList);

            if (await DatabaseData.SharedNotes.Find(filter).FirstOrDefaultAsync() != null)
                return await DatabaseData.SharedNotes.Find(filter).FirstOrDefaultAsync();

            return null;
        }

        public async Task CreateNewFolderAccess(string userId, string folderId, string role)
        {
            IEnumerable<Note> notesInFolder = await new NoteService().GetAllNotesFromFolder(folderId);
            IEnumerable<Folder> childFolders = await new FolderService().GetAllChildFolder(folderId);

            foreach (Folder folder in childFolders)
                await CreateNewFolderAccess(userId, folder.Id, role);

            foreach (Note note in notesInFolder)
                await CreateNewNoteAccess(userId, note.Id, role);

            SharedFolder availableFolder = await GetAvailableFolder(folderId, userId);


            if (availableFolder == null)
            {
                availableFolder = new SharedFolder()
                {
                    UserId = userId,
                    FolderId = folderId,
                    Role = role
                };

                await DatabaseData.SharedFolders.InsertOneAsync(availableFolder);
            }
            else if (availableFolder.Role != role)
            {
                availableFolder.Role = role;
                await UpdateFolderAccess(availableFolder);
            }
        }
        public async Task CreateNewNoteAccess(string userId, string noteId, string role)
        {
            SharedNote availableNote = await GetAvailableNote(noteId, userId);

            if (availableNote == null)
            {
                availableNote = new SharedNote()
                {
                    UserId = userId,
                    NoteId = noteId,
                    Role = role
                };
                await DatabaseData.SharedNotes.InsertOneAsync(availableNote);
            }

            else if (availableNote.Role != role)
            {
                availableNote.Role = role;
                await UpdateNoteAccess(availableNote);
            }
        }

        public async Task<IEnumerable<Object>> GetAvailableItems(string userId)
        {
            if (String.IsNullOrWhiteSpace(userId))
                return null;

            FilterDefinition<SharedFolder> filterFolder = FilterService<SharedFolder>.GetFilterByOneParam("UserId", new ObjectId(userId));
            FilterDefinition<SharedNote> filterNote = FilterService<SharedNote>.GetFilterByOneParam("UserId", new ObjectId(userId));
            List<Object> result = new List<Object>();
            List<string> folderIds = new List<string>();

            List<SharedFolder> AvailableFolerIds = await DatabaseData.SharedFolders.Find(filterFolder).ToListAsync();
            List<SharedNote> AvailableNoteIds = await DatabaseData.SharedNotes.Find(filterNote).ToListAsync();

            foreach (SharedFolder AvailableFolderId in AvailableFolerIds)
                folderIds.Add(AvailableFolderId.FolderId);

            foreach (SharedFolder AvailableFolderId in AvailableFolerIds)
            {
                Folder folder = FolderService.GetFolder(AvailableFolderId.FolderId).GetAwaiter().GetResult();

                if (folder != null && !folderIds.Contains(folder.ParentFolderId))
                {
                    JObject addToResult = JObject.FromObject(folder);
                    addToResult.Add("Role", AvailableFolderId.Role);
                    result.Add(addToResult);
                }
            }

            foreach (SharedNote availableNote in AvailableNoteIds)
            {
                Note note = NoteService.GetNote(availableNote.NoteId).GetAwaiter().GetResult();

                if (note != null && !folderIds.Contains(note.FolderId))
                {
                    JObject addToResult = JObject.FromObject(NoteService.GetNote(availableNote.NoteId).GetAwaiter().GetResult());
                    addToResult.Add("Role", availableNote.Role);
                    result.Add(addToResult);
                }
            }

            return result;
        }
        public async Task<IEnumerable<Object>> GetAvailableItemsFromFolder(string folderId, string userId)
        {
            IEnumerable<Folder> folders = FolderService.GetAllChildFolder(folderId).GetAwaiter().GetResult();
            IEnumerable<Note> notes = NoteService.GetAllNotesFromFolder(folderId).GetAwaiter().GetResult();
            List<Object> result = new List<Object>();

            foreach (Folder folder in folders)
            {
                string role = GetAvailableFolder(folder.Id, userId).GetAwaiter().GetResult().Role;
                JObject addToResult = JObject.FromObject(folder);
                addToResult.Add("Role", role);
                result.Add(addToResult);
            }
            foreach (Note note in notes)
            {
                string role = GetAvailableNote(note.Id, userId).GetAwaiter().GetResult().Role;
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