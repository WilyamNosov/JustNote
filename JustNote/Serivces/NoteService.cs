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
    public class NoteService
    {
        public async Task CreateNote(Note note)
        {
            await DatabaseData.Notes.InsertOneAsync(note);

            if (note.FolderId != null)
            {
                SharedService sharedService = new SharedService();
                IEnumerable<SharedFolder> accessFolders = await sharedService.GetAvailableFoldersByFolderId(note.FolderId);

                foreach (SharedFolder accessFolder in accessFolders)
                {
                    await sharedService.CreateNewNoteAccess(accessFolder.UserId, note.Id, accessFolder.Role);
                }
            }
        }
        public async Task<Note> GetNote(string id)
        {
            return await DatabaseData.Notes.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Note>> GetAllUserNotes(string userId)
        {
            if (String.IsNullOrWhiteSpace(userId))
                return null;

            FilterDefinition<Note> filter = FilterService<Note>.GetFilterByOneParam("UserId", new ObjectId(userId));

            return await DatabaseData.Notes.Find(filter).ToListAsync();
        }
        public async Task<IEnumerable<Note>> GetAllNotesFromFolder(string parentFolderId)
        {
            if (String.IsNullOrWhiteSpace(parentFolderId))
                return null;

            FilterDefinition<Note> filter = FilterService<Note>.GetFilterByOneParam("FolderId", new ObjectId(parentFolderId));

            return await DatabaseData.Notes.Find(filter).ToListAsync();
        }
        public async Task UpdateNote(string noteId, Note note)
        {
            Note oldNote = GetNote(noteId).GetAwaiter().GetResult();
            note.Id = noteId;
            note.UserId = oldNote.UserId;
            note.NoteDate = DateTime.Now;
            note.FolderId = oldNote.FolderId;

            await DatabaseData.Notes.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(noteId)), note);
        }
        public async Task DeleteNote(string noteId)
        {
            await DatabaseData.Notes.DeleteOneAsync(new BsonDocument("_id", new ObjectId(noteId)));
        }
    }
}