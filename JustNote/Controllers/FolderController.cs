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
            var notes = Json(await _noteService.GetAllItemsFromFolder(id));
            //var result = new List<Object>() { notes.Value, new TimedModel() { PreviouseParent = id } };

            return Ok(notes);
        }

        [JustNotesAuthorize]
        [HttpPost]
        public async Task<IActionResult> Post(string token, [FromBody] Folder folder)
        {
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
            }

            return Ok();
        }

        [JustNotesAuthorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, string token, [FromBody] Folder folder)
        {
            await _folderService.Update(id, folder);
            return Ok();
        }

        [JustNotesAuthorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, string token)
        {
            await _folderService.Delete(id);
            return Ok();
        }
    }
}
