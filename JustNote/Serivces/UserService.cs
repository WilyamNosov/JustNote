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
    public class UserService
    {
<<<<<<< Updated upstream
        public UserService() : base()
        {

        }
=======
>>>>>>> Stashed changes
        public async Task<bool> CreateUser(Registration newUser, string hashKey)
        {
            User generateUser = new User()
            {
                UserName = newUser.UserName,
                HashKey = hashKey,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                PhoneNumber = newUser.PhoneNumber
            };
            if (CheckUserInDB(generateUser))
            {
                await DatabaseData.Users.InsertOneAsync(generateUser);
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

<<<<<<< Updated upstream
            if (await Users.Find(filterByName).FirstOrDefaultAsync() != null)
                return await Users.Find(filterByName).FirstOrDefaultAsync();
=======
            if (await DatabaseData.Users.Find(filter).FirstOrDefaultAsync() != null)
            {
                return await DatabaseData.Users.Find(filter).FirstOrDefaultAsync();
            }

            paramList.Remove("UserName");
>>>>>>> Stashed changes

            FilterDefinition<User> filterByEmail = Builders<User>.Filter.And(
                new List<FilterDefinition<User>> {
                    Builders<User>.Filter.Eq("Email", username),
                    Builders<User>.Filter.Eq("HashKey", hashkey)
                });

<<<<<<< Updated upstream
            if (await Users.Find(filterByEmail).FirstOrDefaultAsync() != null)
                return await Users.Find(filterByEmail).FirstOrDefaultAsync();
=======
            if (await DatabaseData.Users.Find(filter).FirstOrDefaultAsync() != null)
            {
                return await DatabaseData.Users.Find(filter).FirstOrDefaultAsync();
            }

            paramList.Remove("Email");
>>>>>>> Stashed changes

            FilterDefinition<User> filterByNumber = Builders<User>.Filter.And(
                new List<FilterDefinition<User>> {
                    Builders<User>.Filter.Eq("PhoneNumber", username),
                    Builders<User>.Filter.Eq("HashKey", hashkey)
                });

<<<<<<< Updated upstream
            if (await Users.Find(filterByNumber).FirstOrDefaultAsync() != null)
                return await Users.Find(filterByNumber).FirstOrDefaultAsync();
=======
            if (await DatabaseData.Users.Find(filter).FirstOrDefaultAsync() != null)
            {
                return await DatabaseData.Users.Find(filter).FirstOrDefaultAsync();
            }
>>>>>>> Stashed changes

            return null;
        }
        public async Task<User> GetUserByEmail(string userEmail)
        {
            FilterDefinition<User> filterByEmail = Builders<User>.Filter.Eq("Email", userEmail);

<<<<<<< Updated upstream
            if (await Users.Find(filterByEmail).FirstOrDefaultAsync() != null)
                return await Users.Find(filterByEmail).FirstOrDefaultAsync();
=======
            if (await DatabaseData.Users.Find(filter).FirstOrDefaultAsync() != null)
            {
                return await DatabaseData.Users.Find(filter).FirstOrDefaultAsync();
            }
>>>>>>> Stashed changes

            return null;
        }

        private bool CheckUserInDB(User newUser)
        {
            IEnumerable<User> users = GetAllUsers().GetAwaiter().GetResult();

            foreach (User user in users)
            {
<<<<<<< Updated upstream
                if (newUser.UserName == user.UserName || newUser.Email == user.Email || newUser.PhoneNumber == user.PhoneNumber)
=======
                if (newUser.UserName == user.UserName || newUser.Email == user.Email)//|| newUser.PhoneNumber == user.PhoneNumber)
>>>>>>> Stashed changes
                    return false;
            }

            return true;
        }
<<<<<<< Updated upstream
        private async Task<IEnumerable<User>> GetAllUsers() => await Users.Find(new BsonDocument()).ToListAsync();
=======

        public async Task UpdateUser(User user)
        {
            await DatabaseData.Users.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(user.Id)), user);
        }

        private async Task<IEnumerable<User>> GetAllUsers()
        {
            return await DatabaseData.Users.Find(new BsonDocument()).ToListAsync();
        }
>>>>>>> Stashed changes
    }
}
