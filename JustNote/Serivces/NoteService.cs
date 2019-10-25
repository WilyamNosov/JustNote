﻿using JustNote.Datas;
using JustNote.Models;
using MongoDB.Bson;
using MongoDB.Driver;
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
        private IMongoCollection<Note> Notes
        {
            get { return base.Database.GetCollection<Note>("note"); }
        }

        public async Task CreateNote(Note note)
        {
            await Notes.InsertOneAsync(note);
        }
        public async Task<Note> GetNote(string id)
        {
            return await Notes.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Note>> GetAllUserNotes(string userId)
        {
            FilterDefinitionBuilder<Note> builder = new FilterDefinitionBuilder<Note>();
            FilterDefinition<Note> filter = builder.Empty;

            if (!String.IsNullOrWhiteSpace(userId))
            {
                filter = filter & builder.Eq("UserId", new ObjectId(userId));
            }

            return await Notes.Find(filter).ToListAsync();
        }
        public async Task<IEnumerable<Note>> GetAllNotesFromFolder(string parentFolderId)
        {
            FilterDefinitionBuilder<Note> builder = new FilterDefinitionBuilder<Note>();
            FilterDefinition<Note> filter = builder.Empty;

            if (!String.IsNullOrWhiteSpace(parentFolderId))
            {
                filter = filter & builder.Eq("FolderId", new ObjectId(parentFolderId));
            }

            return await Notes.Find(filter).ToListAsync();
        }
        public async Task<IEnumerable<Note>> GetFolderBySearchString(string searchString)
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

            await Notes.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(noteId)), note);
        }
        public async Task DeleteNote(string noteId)
        {
            await Notes.DeleteOneAsync(new BsonDocument("_id", new ObjectId(noteId)));
        }
    }
}