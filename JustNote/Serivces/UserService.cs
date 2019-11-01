using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Datas;
using JustNote.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JustNote.Serivces
{
    public class UserService : DatabaseData
    {
        public UserService() : base()
        {

        }
        public async Task<bool> CreateUser(Registration newUser, string hashKey)
        {
            User generateUser = new User()
            {
                UserName = newUser.UserName,
                HashKey = hashKey,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                PhoneNumber = newUser.PhoneNumber,
                ConfirmedEmail = false
            };
            if (CheckUserInDB(generateUser))
            {
                await Users.InsertOneAsync(generateUser);
                return true;
            }
            return false;
        }
        public async Task<User> GetUser(string username, string hashkey)
        {
            FilterDefinition<User> filterByName = Builders<User>.Filter.And(
                new List<FilterDefinition<User>> {
                    Builders<User>.Filter.Eq("UserName", username),
                    Builders<User>.Filter.Eq("HashKey", hashkey)
                });

            if (await Users.Find(filterByName).FirstOrDefaultAsync() != null)
                return await Users.Find(filterByName).FirstOrDefaultAsync();

            FilterDefinition<User> filterByEmail = Builders<User>.Filter.And(
                new List<FilterDefinition<User>> {
                    Builders<User>.Filter.Eq("Email", username),
                    Builders<User>.Filter.Eq("HashKey", hashkey)
                });

            if (await Users.Find(filterByEmail).FirstOrDefaultAsync() != null)
                return await Users.Find(filterByEmail).FirstOrDefaultAsync();

            FilterDefinition<User> filterByNumber = Builders<User>.Filter.And(
                new List<FilterDefinition<User>> {
                    Builders<User>.Filter.Eq("PhoneNumber", username),
                    Builders<User>.Filter.Eq("HashKey", hashkey)
                });

            if (await Users.Find(filterByNumber).FirstOrDefaultAsync() != null)
                return await Users.Find(filterByNumber).FirstOrDefaultAsync();

            return null;
        }
        public async Task<User> GetUserByEmail(string userEmail)
        {
            FilterDefinition<User> filterByEmail = Builders<User>.Filter.Eq("Email", userEmail);

            if (await Users.Find(filterByEmail).FirstOrDefaultAsync() != null)
                return await Users.Find(filterByEmail).FirstOrDefaultAsync();

            return null;
        }

        private bool CheckUserInDB(User newUser)
        {
            IEnumerable<User> users = GetAllUsers().GetAwaiter().GetResult();

            foreach (User user in users)
            {
                if (newUser.UserName == user.UserName || newUser.Email == user.Email || newUser.PhoneNumber == user.PhoneNumber)
                    return false;
            }
            
            return true;
        }
        public async Task UpdateUser(User user)
        {
            await Users.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(user.Id)), user);
        }

        private async Task<IEnumerable<User>> GetAllUsers() => await Users.Find(new BsonDocument()).ToListAsync();
    }
}
