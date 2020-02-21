using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Attributes;
using JustNote.Datas;
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace JustNotes.Controllers
{
    [Route("api/Folder")]
    [ApiController]
    public class FolderController : Controller
    {

<<<<<<< HEAD
        private string userName;
        private string hashKey;
        private FolderService folderData = new FolderService();
        [HttpGet]
        public async Task<IActionResult> Get(string token)
        {
            var tokenManagerService = new TokenManagerService();
            if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
            {
                var notedata = new NoteService();
                var user = await new UserService().GetUser(userName, hashKey);

                IEnumerable<Object> folders = await folderData.GetAllUserFolders(user.Id);
                IEnumerable<Object> notes = await notedata.GetAllUserNotes(user.Id);
=======
        private TokenManagerService _tokenManagerService;
        private IDatabaseItemService<Picture> _pictureService;
        private IDatabaseItemService<Folder> _folderService;
        private IDatabaseItemService<Note> _noteService;

        public FolderController(TokenManagerService tokenManagerService, IDatabaseItemService<Folder> folderService, IDatabaseItemService<Note> noteService, IDatabaseItemService<Picture> pictureService)
        {
            _tokenManagerService = tokenManagerService;
            _folderService = folderService;
            _noteService = noteService;
            _pictureService = pictureService;
        }
>>>>>>> DatabaseData

        [JustNotesAuthorize]
        [HttpGet]
        public async Task<IActionResult> Get(string token)
        {
            var folders = Json(await _folderService.GetAllItems(_tokenManagerService.User.Id));
            var notes = Json(await _noteService.GetAllItems(_tokenManagerService.User.Id));
            var result = new List<Object>() { folders.Value, notes.Value };

            return Ok(result);
        }

        [JustNotesAuthorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, string token)
        {
<<<<<<< HEAD
            var tokenManagerService = new TokenManagerService();
            if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
            {
                var noteData = new NoteService();
                var parentFolder = await folderData.GetFolder(id);

                IEnumerable<Object> folders = await folderData.GetAllChildFolder(id);
                IEnumerable<Object> notes = await noteData.GetAllNotesFromFolder(id);
                IEnumerable<Object> result = folders.Concat(notes);

                IEnumerable<Object> x = new List<Object>() { new TimedModel() { PreviouseParent = parentFolder.ParentFolderId } };

                return Ok(result.Concat(x));
            }
            return Unauthorized();
=======
            var notes = Json(await _noteService.GetAllItemsFromFolder(id));
            //var result = new List<Object>() { notes.Value, new TimedModel() { PreviouseParent = id } };

            return Ok(notes);
>>>>>>> DatabaseData
        }

        [JustNotesAuthorize]
        [HttpPost]
        public async Task<IActionResult> Post(string token, [FromBody] Folder folder)
        {
<<<<<<< HEAD
            var tokenManagerService = new TokenManagerService();
            if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
            {
                var user = await new UserService().GetUser(userName, hashKey);
                folder.FolderDate = DateTime.Now;
                folder.UserId = user.Id;
                folder.ParentFolderId = null;

                await folderData.CreateFolder(folder);
                return Ok();
            }

            return Unauthorized();
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(string id, string token, [FromBody] Folder folder)
        {
            var tokenManagerService = new TokenManagerService();
            if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
            {
                var user = await new UserService().GetUser(userName, hashKey);
                folder.FolderDate = DateTime.Now;
                folder.UserId = user.Id;
                folder.ParentFolderId = id;

                await folderData.CreateFolder(folder);
                return Ok();
=======
            folder.FolderDate = DateTime.Now;
            folder.UserId = _tokenManagerService.User.Id;

            await _folderService.Create(folder);
            return Ok();
        }
        [JustNotesAuthorize]
        [HttpPost("Synchronize")]
        public async Task<IActionResult> Synchronize(string token, [FromBody] IEnumerable<Object> items)
        {
            var pictureArray = new List<Picture>();

            var folders = JArray.FromObject(items.ElementAt(0)).ToObject<List<Folder>>();
            var notes = JArray.FromObject(items.ElementAt(1)).ToObject<List<Note>>();
            var images = JArray.FromObject(items.ElementAt(2)).ToObject<List<Object>>();

            foreach(var folder in folders)
            {
                folder.UserId = _tokenManagerService.User.Id;
            }
            foreach (var note in notes)
            {
                note.UserId = _tokenManagerService.User.Id;
            }

            for (int i = 0; i < images.Count; i++)
            {
                var noteId = JObject.Parse(images.ElementAt(i).ToString()).Value<string>("id");
                var imageArray = JObject.Parse(images.ElementAt(i).ToString()).Value<Object>("imageArray");
                var noteImages = JArray.FromObject(imageArray).ToObject<List<string>>();

                foreach (var noteImage in noteImages)
                {
                    pictureArray.Add(new Picture() { ImageCode = noteImage, NoteId = noteId, UserId = _tokenManagerService.User.Id });
                }
            }

            await DatabaseData.Folders.DeleteManyAsync(new BsonDocument("UserId", new ObjectId(_tokenManagerService.User.Id)));
            await DatabaseData.Notes.DeleteManyAsync(new BsonDocument("UserId", new ObjectId(_tokenManagerService.User.Id)));
            await DatabaseData.Pictires.DeleteManyAsync(new BsonDocument("UserId", new ObjectId(_tokenManagerService.User.Id)));
            
            if (folders.Count > 0)
            {
                await _folderService.CreateManyItems(folders);
            }
            if (notes.Count > 0)
            {
                await _noteService.CreateManyItems(notes);
            }
            if (pictureArray.Count > 0)
            {
                await _pictureService.CreateManyItems(pictureArray);
>>>>>>> DatabaseData
            }

            return Ok();
        }

        [JustNotesAuthorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, string token, [FromBody] Folder folder)
        {
<<<<<<< HEAD
            var tokenManagerService = new TokenManagerService();
            if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
            {
                await folderData.UpdateFolder(id, folder);

                return Ok();
            }

            return Unauthorized();
=======
            await _folderService.Update(id, folder);
            return Ok();
>>>>>>> DatabaseData
        }

        [JustNotesAuthorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, string token)
        {
<<<<<<< HEAD
            var tokenManagerService = new TokenManagerService();
            if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
            {
                await folderData.DeleteFolder(id);

                return Ok();
            }

            return Unauthorized();
=======
            await _folderService.Delete(id);
            return Ok();
>>>>>>> DatabaseData
        }
    }
}
