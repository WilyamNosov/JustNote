using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Datas;
using JustNote.Models;
using JustNotes.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JustNote.Serivces
{
    public class UserService
    {
<<<<<<< HEAD
        public UserService() : base()
        {

        }

=======
>>>>>>> DatabaseData
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

<<<<<<< HEAD
            if (CheckUserInDB(generateUser))
=======
            if (await CheckUserInDB(generateUser))
>>>>>>> DatabaseData
            {
                await DatabaseData.Users.InsertOneAsync(generateUser);
                return true;
            }

            return false;
        }

        public async Task<User> GetUser(string username, string hashkey)
        {
            List<string> paramList = new List<string>() { "HashKey", "UserName", "Email", "PhoneNumber" };
            List<object> valueList = new List<object>() { hashkey, username };
<<<<<<< HEAD

            FilterDefinition<User> filter = FilterService<User>.GetFilterByTwoParam(paramList, valueList);

            if (await Users.Find(filter).FirstOrDefaultAsync() != null)
                return await Users.Find(filter).FirstOrDefaultAsync();            
            paramList.Remove("UserName");

            filter = FilterService<User>.GetFilterByTwoParam(paramList, valueList);

            if (await Users.Find(filter).FirstOrDefaultAsync() != null)
                return await Users.Find(filter).FirstOrDefaultAsync();
            paramList.Remove("Email");

            filter = FilterService<User>.GetFilterByTwoParam(paramList, valueList);

            if (await Users.Find(filter).FirstOrDefaultAsync() != null)
                return await Users.Find(filter).FirstOrDefaultAsync();
=======

            FilterDefinition<User> filter = FilterService<User>.GetFilterByTwoParam(paramList, valueList);

            if (await DatabaseData.Users.Find(filter).FirstOrDefaultAsync() != null)
            {
                return await DatabaseData.Users.Find(filter).FirstOrDefaultAsync();
            }

            paramList.Remove("UserName");

            filter = FilterService<User>.GetFilterByTwoParam(paramList, valueList);

            if (await DatabaseData.Users.Find(filter).FirstOrDefaultAsync() != null)
            {
                return await DatabaseData.Users.Find(filter).FirstOrDefaultAsync();
            }

            paramList.Remove("Email");

            filter = FilterService<User>.GetFilterByTwoParam(paramList, valueList);

            if (await DatabaseData.Users.Find(filter).FirstOrDefaultAsync() != null)
            {
                return await DatabaseData.Users.Find(filter).FirstOrDefaultAsync();
            }
>>>>>>> DatabaseData

            return null;
        }

        public async Task<User> GetUserByEmail(string userEmail)
        {
            FilterDefinition<User> filter = FilterService<User>.GetFilterByOneParam("Email", userEmail);

<<<<<<< HEAD
            if (await Users.Find(filter).FirstOrDefaultAsync() != null)
                return await Users.Find(filter).FirstOrDefaultAsync();
=======
            if (await DatabaseData.Users.Find(filter).FirstOrDefaultAsync() != null)
            {
                return await DatabaseData.Users.Find(filter).FirstOrDefaultAsync();
            }
>>>>>>> DatabaseData

            return null;
        }

        public async Task UpdateUser(User user)
        {
            await DatabaseData.Users.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(user.Id)), user);
        }

        private async Task<bool> CheckUserInDB(User newUser)
        {
            IEnumerable<User> users = await GetAllUsers();

            foreach (User user in users)
            {
<<<<<<< HEAD
                if (newUser.UserName == user.UserName || newUser.Email == user.Email )//|| newUser.PhoneNumber == user.PhoneNumber)
=======
                if (newUser.UserName == user.UserName || newUser.Email == user.Email)//|| newUser.PhoneNumber == user.PhoneNumber)
                {
>>>>>>> DatabaseData
                    return false;
                }
            }

            return true;
        }
<<<<<<< HEAD
        
        public async Task UpdateUser(User user) => await Users.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(user.Id)), user);
        
        private async Task<IEnumerable<User>> GetAllUsers() => await Users.Find(new BsonDocument()).ToListAsync();
=======

        private async Task<IEnumerable<User>> GetAllUsers()
        {
            return await DatabaseData.Users.Find(new BsonDocument()).ToListAsync();
        }
>>>>>>> DatabaseData
    }
}
