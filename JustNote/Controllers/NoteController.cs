  using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
<<<<<<< HEAD
//using JustNote.Attributes;
=======
using JustNote.Attributes;
>>>>>>> DatabaseData
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace JustNotes.Controllers
{
    [Route("api/Note")]
    [ApiController]
    public class NoteController : Controller
    {
        private IDatabaseItemService<Note> _noteService;
        private IDatabaseItemService<Picture> _pictureService;
        private TokenManagerService _tokenManagerService;

<<<<<<< HEAD
        //[JustNotesAuthorize]
        [HttpGet("{id}")]
        public async Task<Note> Get(string id)
        {
            return await noteData.GetNote(id);
=======
        public NoteController(TokenManagerService tokenManagerService, IDatabaseItemService<Note> noteService, IDatabaseItemService<Picture> pictureService)
        {
            _tokenManagerService = tokenManagerService;
            _noteService = noteService;
            _pictureService = pictureService;
        }

        [HttpGet("Site/{id}")]
        public async Task<IActionResult> GetForSite(string id)
        {
            var note = await _noteService.Get(id);
            var images = await _pictureService.GetAllItemsFromFolder(id);
            var pictures = new List<string>();
            
            foreach(var image in images)
            {
                pictures.Add(image.ImageCode);
            }
            
            var result = new List<object>() { Json(note).Value, Json(pictures).Value };
            return Ok(result);

        }

        [JustNotesAuthorize]
        [HttpGet("{id}")]
        public async Task<Note> Get(string id, string token)
        {
            return await _noteService.Get(id);
>>>>>>> DatabaseData
        }

        [JustNotesAuthorize]
        [HttpPost]
<<<<<<< HEAD
        public async Task<IActionResult> Post(string token, [FromBody] Note note, string folderId = null)
        {
            var tokenManagerService = new TokenManagerService();
            if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
            {
                var user = await new UserService().GetUser(userName, hashKey);
                note.NoteDate = DateTime.Now;
                note.UserId = user.Id;
                note.FolderId = folderId;

                await noteData.CreateNote(note);
                return Ok();
=======
        public async Task<IActionResult> Post(string token, [FromBody] IEnumerable<Object> inputData)
        {
            var note = JObject.FromObject(inputData.ElementAt(0)).ToObject<Note>();
            var inputPicturesArray = JArray.FromObject(inputData.ElementAt(1)).ToObject<List<string>>();
            note.NoteDate = DateTime.Now;
            note.UserId = _tokenManagerService.User.Id;
            var pictureArray = new List<Picture>();

            foreach(var inputPicture in inputPicturesArray)
            {
                pictureArray.Add(new Picture() { ImageCode = inputPicture, NoteId = note.LocalId, UserId = _tokenManagerService.User.Id });
>>>>>>> DatabaseData
            }

            await _noteService.Create(note);
            await _pictureService.CreateManyItems(pictureArray);

            return Ok();
        }

        [JustNotesAuthorize]
        [HttpPost("{id}")]
<<<<<<< HEAD
        public async Task<IActionResult> Post(string id, string token, [FromBody] Note note)
        {
            var tokenManagerService = new TokenManagerService();
            if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
            {
                var user = await new UserService().GetUser(userName, hashKey);
                note.NoteDate = DateTime.Now;
                note.UserId = user.Id;
                note.FolderId = id;

                await noteData.CreateNote(note);
                return Ok();
=======
        public async Task<IActionResult> Post(string id, string token, [FromBody] IEnumerable<Object> inputData)
        {
            var note = JObject.FromObject(inputData.ElementAt(0)).ToObject<Note>();
            var inputPicturesArray = JArray.FromObject(inputData.ElementAt(1)).ToObject<List<string>>();
            note.NoteDate = DateTime.Now;
            note.UserId = _tokenManagerService.User.Id;
            note.FolderId = id;
            var pictureArray = new List<Picture>();

            foreach (var inputPicture in inputPicturesArray)
            {
                pictureArray.Add(new Picture() { ImageCode = inputPicture, NoteId = note.LocalId, UserId = _tokenManagerService.User.Id });
>>>>>>> DatabaseData
            }

            await _noteService.Create(note);
            await _pictureService.CreateManyItems(pictureArray);

            return Ok();
        }

        [JustNotesAuthorize]
        [HttpPut("{id}")]
<<<<<<< HEAD
        public async Task<IActionResult> Put(string token, string id, [FromBody] Note note)
        {
            var tokenManagerService = new TokenManagerService();
            if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
            {
                await noteData.UpdateNote(id, note);
                return Ok();
=======
        public async Task<IActionResult> Put(string token, string id, [FromBody] IEnumerable<Object> inputData)
        {
            var note = JObject.FromObject(inputData.ElementAt(0)).ToObject<Note>();
            var inputPicturesArray = JArray.FromObject(inputData.ElementAt(1)).ToObject<List<string>>();
            note.NoteDate = DateTime.Now;
            note.LocalId = id;
            note.UserId = _tokenManagerService.User.Id;
            var pictureArray = new List<Picture>();

            foreach (var inputPicture in inputPicturesArray)
            {
                pictureArray.Add(new Picture() { ImageCode = inputPicture, NoteId = note.LocalId, UserId = _tokenManagerService.User.Id });
>>>>>>> DatabaseData
            }

            await _noteService.Update(id, note);
            await _pictureService.Delete(id);
            await _pictureService.CreateManyItems(pictureArray);

            return Ok();
        }

        [JustNotesAuthorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string token, string id)
        {
<<<<<<< HEAD
            var tokenManagerService = new TokenManagerService();
            if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
            {
                await noteData.DeleteNote(id);
                return Ok();
            }

            return Unauthorized();
=======
            await _noteService.Delete(id);
            return Ok();
>>>>>>> DatabaseData
        }
    }
}
