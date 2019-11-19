using JustNote.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustNote.Datas
{
    public static class DatabaseData
    {

        private static string connect = "mongodb+srv://DeadApolon:Pa$$w0Rd@cluster0-pdwof.mongodb.net/test?retryWrites=true&w=majority";
        private static MongoClient client;

        private static IMongoDatabase database;
        static DatabaseData()
        {
            client = new MongoClient(connect);
            database = client.GetDatabase("JustNote");
        }
        public static IMongoDatabase Database
        {
            get
            {
                return database;
            }
        }
        public static IMongoCollection<User> Users
        {
            get { return Database.GetCollection<User>("user"); }
        }
        public static IMongoCollection<Folder> Folders
        {
            get { return Database.GetCollection<Folder>("folder"); }
        }
        public static IMongoCollection<Note> Notes
        {
            get { return Database.GetCollection<Note>("note"); }
        }
        public static IMongoCollection<SharedFolder> SharedFolders
        {
            get { return Database.GetCollection<SharedFolder>("availablefolder"); }
        }
        public static IMongoCollection<SharedNote> SharedNotes
        {
            get { return Database.GetCollection<SharedNote>("availablenote"); }
        }
    }
}
