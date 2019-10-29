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
    public class AccessService : DatabaseData
    {
        private FolderService FolderService = new FolderService();
        private NoteService NoteService = new NoteService();

        public AccessService() : base()
        {

        }
        public IMongoCollection<AvailableFolder> AccessFolders
        {
            get { return base.Database.GetCollection<AvailableFolder>("availablefolder"); }
        }
        public IMongoCollection<AvailableNote> AccessNotes
        {
            get { return base.Database.GetCollection<AvailableNote>("availablenote"); }
        }

        public async Task<AvailableFolder> GetAvailableFolder(string folderId, string userId)
        {
            FilterDefinition<AvailableFolder> filter = Builders<AvailableFolder>.Filter.And(
                new List<FilterDefinition<AvailableFolder>> {
                    Builders<AvailableFolder>.Filter.Eq("FolderId", folderId),
                    Builders<AvailableFolder>.Filter.Eq("UserId", userId)
                });

            if (await AccessFolders.Find(filter).FirstOrDefaultAsync() != null)
                return await AccessFolders.Find(filter).FirstOrDefaultAsync();

            return null;
        }

        public async Task<AvailableNote> GetAvailableNote(string noteId, string userId)
        {
            FilterDefinition<AvailableNote> filter = Builders<AvailableNote>.Filter.And(
                new List<FilterDefinition<AvailableNote>> {
                    Builders<AvailableNote>.Filter.Eq("NoteId", noteId),
                    Builders<AvailableNote>.Filter.Eq("UserId", userId)
                });

            if (await AccessNotes.Find(filter).FirstOrDefaultAsync() != null)
                return await AccessNotes.Find(filter).FirstOrDefaultAsync();

            return null;
        }

        public async Task CreateNewFolderAccess(string userId, string folderId, string role)
        {
            IEnumerable<Note> notesInFolder = await new NoteService().GetAllNotesFromFolder(folderId);
            IEnumerable<Folder> childFolders = await new FolderService().GetAllChildFolder(folderId);

            foreach (Note note in notesInFolder)
            {
                if(GetAvailableNote(note.Id, userId) == null)
                    CreateNewNoteAccess(userId, note.Id, role).GetAwaiter().GetResult();
            }

            foreach(Folder folder in childFolders)
            {
                CreateNewFolderAccess(userId, folder.Id, role).GetAwaiter().GetResult();
            }

            AvailableFolder availableFolder = new AvailableFolder()
            {
                UserId = userId,
                FolderId = folderId,
                Role = role
            };

            if (GetAvailableFolder(folderId, userId) == null)
                await AccessFolders.InsertOneAsync(availableFolder);
        }
        public async Task CreateNewNoteAccess(string userId, string noteId, string role)
        {
            AvailableNote availableNote = new AvailableNote()
            {
                UserId = userId,
                NoteId = noteId,
                Role = role
            };

            if (GetAvailableNote(noteId, userId) == null)
                await AccessNotes.InsertOneAsync(availableNote);
        }

        public async Task<IEnumerable<Object>> GetAvailableItems(string userId)
        {
            FilterDefinitionBuilder<AvailableFolder> builderFolder = new FilterDefinitionBuilder<AvailableFolder>();
            FilterDefinitionBuilder<AvailableNote> builderNote = new FilterDefinitionBuilder<AvailableNote>();
            FilterDefinition<AvailableFolder> filterFolder = builderFolder.Empty;
            FilterDefinition<AvailableNote> filterNote = builderNote.Empty;
            List<Object> result = new List<Object>();
            List<string> folderIds = new List<string>();

            if (!string.IsNullOrWhiteSpace(userId))
            {
                filterFolder = filterFolder & builderFolder.Eq("UserId", new ObjectId(userId));
                filterNote = filterNote & builderNote.Eq("UserId", new ObjectId(userId));
            }

            List<AvailableFolder> AvailableFolerIds = await AccessFolders.Find(filterFolder).ToListAsync();
            List<AvailableNote> AvailableNoteIds = await AccessNotes.Find(filterNote).ToListAsync();

            foreach (AvailableFolder AvailableFolderId in AvailableFolerIds)
                folderIds.Add(AvailableFolderId.FolderId);

            foreach (AvailableFolder AvailableFolderId in AvailableFolerIds)
            {
                Folder folder = FolderService.GetFolder(AvailableFolderId.FolderId).GetAwaiter().GetResult();

                if (!folderIds.Contains(folder.ParentFolderId))
                {
                    JObject addToResult = JObject.FromObject(folder);
                    addToResult.Add("Role", AvailableFolderId.Role);
                    result.Add(addToResult);
                }
            }

            foreach (AvailableNote availableNote in AvailableNoteIds)
            {
                Note note = NoteService.GetNote(availableNote.NoteId).GetAwaiter().GetResult();

                if (!folderIds.Contains(note.FolderId))
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

            foreach(Folder folder in folders)
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
    }
}
