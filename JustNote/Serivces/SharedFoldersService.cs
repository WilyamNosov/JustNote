﻿using JustNote.Models;
using JustNote.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using JustNotes.Services;
using MongoDB.Driver;

namespace JustNote.Serivces
{
    public class SharedFoldersService : IDatabaseItemService<SharedFolder>
    {
        IDatabaseItemService<Folder> _folderService;
        IDatabaseItemService<Note> _noteService;
        IDatabaseItemService<SharedNote> _sharedNoteService;

        public SharedFoldersService(IDatabaseItemService<Folder> folderService, IDatabaseItemService<Note> noteService, IDatabaseItemService<SharedNote> sharedNoteService)
        {
            _folderService = folderService;
            _noteService = noteService;
            _sharedNoteService = sharedNoteService;
        }
        public async Task Create(SharedFolder item)
        {
            var propertyList = new List<string>() { "UserId", "FolderId" };
            var valueList = new List<object>() { new ObjectId(item.UserId), item.FolderId };
            var filter = FilterService<SharedFolder>.GetFilterByTwoParam(propertyList, valueList);
            var sharedFolder = await DatabaseData.SharedFolders.Find(filter).FirstOrDefaultAsync();

            if (sharedFolder != null && sharedFolder.Role == item.Role)
            {
                throw new Exception("The user have access to this folder");
            } 
            else if (sharedFolder != null && sharedFolder.Role != item.Role)
            {
                sharedFolder.Role = item.Role;
                await Update(sharedFolder.Id, sharedFolder);
            }
            else
            {
                await DatabaseData.SharedFolders.InsertOneAsync(item);
            }
            
            await ShareNotesFromFolder(item);
        }

        public Task CreateManyItems(List<SharedFolder> items)
        {
            throw new NotImplementedException();
        }

        public async Task<SharedFolder> Get(string id)
        {
            var result = await DatabaseData.SharedFolders.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
            return result;
        }

        public async Task<IEnumerable<SharedFolder>> GetAllItems(string id)
        {
            var filter = FilterService<SharedFolder>.GetFilterByOneParam("UserId", new ObjectId(id));
            var result = await DatabaseData.SharedFolders.Find(filter).ToListAsync();
            
            if (result != null) 
            {
                return result;
            }

            filter = FilterService<SharedFolder>.GetFilterByOneParam("FolerId", new ObjectId(id));
            result = await DatabaseData.SharedFolders.Find(filter).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<SharedFolder>> GetAllItemsFromFolder(string id)
        {
            /*
             * GET ALL ITEMS BY FOLDER ID
             */
            var result = await DatabaseData.SharedFolders.Find(new BsonDocument("FolderId", id)).ToListAsync();

            return result;
        }

        public Task<IEnumerable<SharedFolder>> GetAllItemsFromDatabase()
        {
            throw new NotImplementedException();
        }

        public async Task Update(string id, SharedFolder item)
        {
            await DatabaseData.SharedFolders.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(id)), item);
        }

        public async Task Delete(string id)
        {
            await DatabaseData.SharedFolders.DeleteOneAsync(id);
        }
        private async Task ShareNotesFromFolder(SharedFolder sharedFolder)
        {
            var notesFromFolder = await _noteService.GetAllItemsFromFolder(sharedFolder.FolderId);
            var userSharedNotes = await _sharedNoteService.GetAllItems(sharedFolder.UserId);
            var notesShareResult = new List<SharedNote>();
            bool flag = true;

            foreach (var noteFromFolder in notesFromFolder)
            {
                foreach (var userSharedNote in userSharedNotes)
                {
                    if (noteFromFolder.LocalId == userSharedNote.NoteId)
                    {
                        await _sharedNoteService.Delete(userSharedNote.Id);
                        notesShareResult.Add(new SharedNote() { UserId = sharedFolder.UserId, NoteId = noteFromFolder.LocalId, Role = sharedFolder.Role });
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    notesShareResult.Add(new SharedNote() { UserId = sharedFolder.UserId, NoteId = noteFromFolder.LocalId, Role = sharedFolder.Role });
                }
            }
            if (notesShareResult.Count > 0)
            {
                await DatabaseData.SharedNotes.InsertManyAsync(notesShareResult);
            }
        }
    }
}
