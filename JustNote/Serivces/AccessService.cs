using JustNote.Datas;
using JustNote.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustNote.Serivces
{
    public class AccessService : DatabaseData
    {
        public AccessService() : base()
        {

        }
        public IMongoCollection<AvailableFolder> AccessFolders
        {
            get { return base.Database.GetCollection<AvailableFolder>("availablefolder"); }
        }
        public IMongoCollection<AvailableNote> AccessNotes
        {
            get { return base.Database.GetCollection<AvailableNote>("availablenote"); }
        }
        public async Task CreateNewFolderAccess(string userId, string folderId, string role)
        {
            IEnumerable<Note> notesInFolder = await new NoteService().GetAllNotesFromFolder(folderId);
            IEnumerable<Folder> childFolders = await new FolderService().GetAllChildFolder(folderId);

            foreach (Note note in notesInFolder)
            {
                CreateNewNoteAccess(userId, note.Id, role).GetAwaiter().GetResult();
            }

            foreach(Folder folder in childFolders)
            {
                CreateNewFolderAccess(userId, folder.Id, role).GetAwaiter().GetResult();
            }

            AvailableFolder availableFolder = new AvailableFolder()
            {
                UserId = userId,
                FolderId = folderId,
                Role = role
            };

            await AccessFolders.InsertOneAsync(availableFolder);
        }
        public async Task CreateNewNoteAccess(string userId, string noteId, string role)
        {
            AvailableNote availableNote = new AvailableNote()
            {
                UserId = userId,
                NoteId = noteId,
                Role = role
            };
            await AccessNotes.InsertOneAsync(availableNote);
        }
    }
}
