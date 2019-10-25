using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustNote.Datas
{
    public class DatabaseData
    {

        private string connect = "mongodb://localhost:27017";
        private static MongoClient client;
        
        private static IMongoDatabase database;
        public DatabaseData()
        {
            client = new MongoClient(connect);
            database = client.GetDatabase("JustNote");
        }
        public IMongoDatabase Database
        {
            get
            {
                return database;
            }
        }
    }
}
