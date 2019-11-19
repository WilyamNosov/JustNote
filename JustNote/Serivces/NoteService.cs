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
    public class NoteService
    {
        public async Task CreateNote(Note note)
        {
<<<<<<< Updated upstream
            await Notes.InsertOneAsync(note); 
            
            AccessService accessService = new AccessService();
            IEnumerable<AvailableFolder> accessFolders = accessService.GetAvailableFoldersByFolderId(note.FolderId).GetAwaiter().GetResult();

            foreach (AvailableFolder accessFolder in accessFolders)
            {
                accessService.CreateNewNoteAccess(accessFolder.UserId, note.Id, accessFolder.Role).GetAwaiter().GetResult();
=======
            await DatabaseData.Notes.InsertOneAsync(note);

            if (note.FolderId != null)
            {
                SharedService accessService = new SharedService();
                IEnumerable<SharedFolder> accessFolders = await accessService.GetAvailableFoldersByFolderId(note.FolderId);

                foreach (SharedFolder accessFolder in accessFolders)
                {
                    await accessService.CreateNewNoteAccess(accessFolder.UserId, note.Id, accessFolder.Role);
                }
>>>>>>> Stashed changes
            }
        }
        public async Task<Note> GetNote(string id)
        {
            return await DatabaseData.Notes.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Note>> GetAllUserNotes(string userId)
        {
            FilterDefinitionBuilder<Note> builder = new FilterDefinitionBuilder<Note>();
            FilterDefinition<Note> filter = builder.Empty;

            if (!String.IsNullOrWhiteSpace(userId))
            {
                filter = filter & builder.Eq("UserId", new ObjectId(userId));
            }

            return await DatabaseData.Notes.Find(filter).ToListAsync();
        }
        public async Task<IEnumerable<Note>> GetAllNotesFromFolder(string parentFolderId)
        {
            FilterDefinitionBuilder<Note> builder = new FilterDefinitionBuilder<Note>();
            FilterDefinition<Note> filter = builder.Empty;

            if (!String.IsNullOrWhiteSpace(parentFolderId))
            {
                filter = filter & builder.Eq("FolderId", new ObjectId(parentFolderId));
            }

            return await DatabaseData.Notes.Find(filter).ToListAsync();
        }
        public async Task<IEnumerable<Note>> GetNoteBySearchString(string searchString)
        {
            FilterDefinitionBuilder<Note> builder = new FilterDefinitionBuilder<Note>();
            FilterDefinition<Note> filter = builder.Empty;

            if (!String.IsNullOrWhiteSpace(searchString))
            {
                filter = filter & builder.Gte("Name", searchString);
            }

            return await Notes.Find(filter).ToListAsync();
        }
        public async Task UpdateNote(string noteId, string userId, Note note)
        {
            note.Id = noteId;
            note.UserId = userId;
            note.NoteDate = DateTime.Now;
            note.FolderId = GetNote(noteId).GetAwaiter().GetResult().FolderId;

            await DatabaseData.Notes.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(noteId)), note);
        }
        public async Task DeleteNote(string noteId)
        {
            await DatabaseData.Notes.DeleteOneAsync(new BsonDocument("_id", new ObjectId(noteId)));
        }
    }
}
