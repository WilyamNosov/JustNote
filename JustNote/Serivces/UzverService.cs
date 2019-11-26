using JustNote.Datas;
using JustNote.Models;
using JustNotes.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustNote.Serivces
{
    public class UzverService : IDatabaseItemService<User>
    {
        public async Task Create(User item)
        {
            var checkUser = await CheckUser(item);

            if (!checkUser)
            {
                await DatabaseData.Users.InsertOneAsync(item);
            }
            else
            {
                throw new Exception("Bad request.");
            }
        }

        public async Task<User> Get(string id)
        {
            var result = await DatabaseData.Users.Find(new BsonDocument("_id", new ObjectId(id))).SingleOrDefaultAsync();
            return result;
        }
        public async Task<User> GetUser(string userAuthenticator, string hashKey)
        {
            List<string> paramList = new List<string>() { "HashKey", "UserName", "Email", "PhoneNumber" };
            List<object> valueList = new List<object>() { hashKey, userAuthenticator };
            var result = new User();

            FilterDefinition<User> filter = FilterService<User>.GetFilterByTwoParam(paramList, valueList);
            if (await DatabaseData.Users.Find(filter).FirstOrDefaultAsync() != null)
            {
                result = await DatabaseData.Users.Find(filter).FirstOrDefaultAsync();
                if (result.ConfirmedEmail)
                {
                    return result;
                }
            }
            paramList.Remove("UserName");

            filter = FilterService<User>.GetFilterByTwoParam(paramList, valueList);
            if (await DatabaseData.Users.Find(filter).FirstOrDefaultAsync() != null)
            {
                result = await DatabaseData.Users.Find(filter).FirstOrDefaultAsync();
                if (result.ConfirmedEmail)
                {
                    return result;
                }
            }
            paramList.Remove("Email");

            filter = FilterService<User>.GetFilterByTwoParam(paramList, valueList);
            if (await DatabaseData.Users.Find(filter).FirstOrDefaultAsync() != null)
            {
                result = await DatabaseData.Users.Find(filter).FirstOrDefaultAsync();
                if (result.ConfirmedEmail)
                {
                    return result;
                }
            }

            throw new Exception("Bad request.");
        }
        public async Task<User> GetUserByEmail(string userEmail)
        {
            FilterDefinition<User> filter = FilterService<User>.GetFilterByOneParam("Email", userEmail);

            if (await DatabaseData.Users.Find(filter).FirstOrDefaultAsync() != null)
            {
                return await DatabaseData.Users.Find(filter).FirstOrDefaultAsync();
            }

            return null;
        }

        public async Task<IEnumerable<User>> GetAllItemsFromDatabase()
        {
            var result = await DatabaseData.Users.Find(new BsonDocument()).ToListAsync();
            return result;
        }

        public async Task Update(string id, User item)
        {
            await DatabaseData.Users.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(id)), item);
        }

        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllItems(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllItemsFromFolder(string id)
        {
            throw new NotImplementedException();
        }
        private async Task<bool> CheckUser(User newUser)
        {
            var users = await GetAllItemsFromDatabase();

            foreach (User user in users)
            {
                if (newUser.UserName == user.UserName || newUser.Email == user.Email)//|| newUser.PhoneNumber == user.PhoneNumber)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
