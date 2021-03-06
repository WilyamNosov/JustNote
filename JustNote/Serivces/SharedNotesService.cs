﻿using JustNote.Models;
using JustNote.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using JustNotes.Services;

namespace JustNote.Serivces
{
    public class SharedNotesService : IDatabaseItemService<SharedNote>
    {
        public async Task Create(SharedNote item)
        {
            var propertyList = new List<string>() { "UserId", "NoteId" };
            var valueList = new List<object>() { new ObjectId(item.UserId), item.NoteId};

            var filter = FilterService<SharedNote>.GetFilterByTwoParam(propertyList, valueList);
            var sharedNote = await DatabaseData.SharedNotes.Find(filter).FirstOrDefaultAsync();
            
            if (sharedNote != null && sharedNote.Role == item.Role)
            {
                throw new Exception("The user have access to this note");
            }
            else if (sharedNote != null && sharedNote.Role != item.Role)
            {
                item.Id = sharedNote.Id;
                await Update(item.Id, item);
            }
            else {
                await DatabaseData.SharedNotes.InsertOneAsync(item);
            }
        }

        public Task CreateManyItems(List<SharedNote> items)
        {
            throw new NotImplementedException();
        }

        public async Task<SharedNote> Get(string id)
        {
            return await DatabaseData.SharedNotes.Find(new BsonDocument("_id", new ObjectId(id))).FirstAsync();
        }

        public async Task<IEnumerable<SharedNote>> GetAllItems(string id)
        {
            FilterDefinition<SharedNote> filter = FilterService<SharedNote>.GetFilterByOneParam("UserId", id);

            return await DatabaseData.SharedNotes.Find(filter).ToListAsync();
        }

        public Task<IEnumerable<SharedNote>> GetAllItemsFromFolder(string id)
        {
            throw new NotImplementedException();
        }

        public async Task Update(string id, SharedNote item)
        {
            await DatabaseData.SharedNotes.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(id)), item);
        }

        public async Task Delete(string id)
        {
            await DatabaseData.SharedNotes.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }

        public Task<IEnumerable<SharedNote>> GetAllItemsFromDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
