using JustNote.Models;
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
            var valueList = new List<object>() { new ObjectId(item.UserId), new ObjectId(item.FolderId) };
            var filter = FilterService<SharedFolder>.GetFilterByTwoParam(propertyList, valueList);
            var sharedFolder = await DatabaseData.SharedFolders.Find(filter).FirstOrDefaultAsync();

            if (sharedFolder != null && sharedFolder.Role == item.Role)
            {
                throw new Exception("The user have access to this folder");
            } 
            else if (sharedFolder.Role != item.Role)
            {
                await Update(sharedFolder.Id, item);
            }
            else
            {
                await DatabaseData.SharedFolders.InsertOneAsync(item);
            }
            
            await ShareNotesFromFolder(item);
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

        public Task<IEnumerable<SharedFolder>> GetAllItemsFromFolder(string id)
        {
            throw new NotImplementedException();
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
            var folder = await _folderService.Get(id);
            //Need get Email
        }
        private async Task ShareNotesFromFolder(SharedFolder sharedFolder)
        {
            var notesFromFolder = await _noteService.GetAllItemsFromFolder(sharedFolder.FolderId);
            var userSharedNotes = await _sharedNoteService.GetAllItems(sharedFolder.UserId);
            var notesShareResult = new List<SharedNote>();

            foreach (var noteFromFolder in notesFromFolder)
            {
                foreach (var userSharedNote in userSharedNotes)
                {
                    if (noteFromFolder.Id == userSharedNote.NoteId)
                    {
                        await _sharedNoteService.Delete(userSharedNote.Id);
                        notesShareResult.Add(new SharedNote() { UserId = sharedFolder.UserId, NoteId = noteFromFolder.Id, Role = sharedFolder.Role });
                    }
                    else
                    {
                        notesShareResult.Add(new SharedNote() { UserId = sharedFolder.UserId, NoteId = noteFromFolder.Id, Role = sharedFolder.Role });
                    }
                }
            }

            await DatabaseData.SharedNotes.InsertManyAsync(notesShareResult);
        }
    }
}
