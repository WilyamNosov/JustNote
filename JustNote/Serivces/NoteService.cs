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
    public class NoteService : DatabaseData
    {
        public NoteService() : base()
        {

        }
        public async Task CreateNote(Note note)
        {
            await Notes.InsertOneAsync(note); 
            
            if (note.FolderId != null)
            {
                AccessService accessService = new AccessService();
                IEnumerable<AvailableFolder> accessFolders = accessService.GetAvailableFoldersByFolderId(note.FolderId).GetAwaiter().GetResult();

                foreach (AvailableFolder accessFolder in accessFolders)
                {
                    accessService.CreateNewNoteAccess(accessFolder.UserId, note.Id, accessFolder.Role).GetAwaiter().GetResult();
                }
            }
        }
        public async Task<Note> GetNote(string id)
        {
            return await Notes.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Note>> GetAllUserNotes(string userId)
        {
            if (String.IsNullOrWhiteSpace(userId))
                return null;

            FilterDefinition<Note> filter = FilterService<Note>.GetFilterByOneParam("UserId", new ObjectId(userId));

            return await Notes.Find(filter).ToListAsync();
        }
        public async Task<IEnumerable<Note>> GetAllNotesFromFolder(string parentFolderId)
        {
            if (String.IsNullOrWhiteSpace(parentFolderId))
                return null;

            FilterDefinition<Note> filter = FilterService<Note>.GetFilterByOneParam("FolderId", new ObjectId(parentFolderId));

            return await Notes.Find(filter).ToListAsync();
        }
        public async Task UpdateNote(string noteId, Note note)
        {
            Note oldNote = GetNote(noteId).GetAwaiter().GetResult();
            note.Id = noteId;
            note.UserId = oldNote.UserId;
            note.NoteDate = DateTime.Now;
            note.FolderId = oldNote.FolderId;

            await Notes.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(noteId)), note);
        }
        public async Task DeleteNote(string noteId)
        {
            await Notes.DeleteOneAsync(new BsonDocument("_id", new ObjectId(noteId)));
        }
        //public async Task<IEnumerable<Note>> GetNoteBySearchString(string searchString)
        //{
        //    FilterDefinitionBuilder<Note> builder = new FilterDefinitionBuilder<Note>();
        //    FilterDefinition<Note> filter = builder.Empty;

        //    if (!String.IsNullOrWhiteSpace(searchString))
        //    {
        //        filter = filter & builder.Gte("Name", searchString);
        //    }

        //    return await Notes.Find(filter).ToListAsync();
        //}
    }
}
