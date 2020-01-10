using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Attributes;
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace JustNote.Controllers
{
    [Route("api/Shared")]
    public class SharedController : Controller
    {
        private TokenManagerService _tokenManagerService;
        private IDatabaseItemService<Folder> _folderService;
        private IDatabaseItemService<Note> _noteService;
        private IDatabaseItemService<SharedFolder> _sharedFolderService;
        private IDatabaseItemService<SharedNote> _sharedNoteService;
        private IDatabaseItemService<Picture> _pictureService;
        private IDatabaseItemService<User> _userService;

        public SharedController(TokenManagerService tokenManagerService, IDatabaseItemService<Folder> folderService, IDatabaseItemService<Note> noteService,
            IDatabaseItemService<SharedFolder> sharedFolderService, IDatabaseItemService<SharedNote> sharedNoteService, IDatabaseItemService<User> userService,
            IDatabaseItemService<Picture> pictureService)
        {
            _tokenManagerService = tokenManagerService;
            _folderService = folderService;
            _noteService = noteService;
            _sharedFolderService = sharedFolderService;
            _sharedNoteService = sharedNoteService;
            _pictureService = pictureService;
            _userService = userService;
        }

        [JustNotesAuthorize]
        [HttpGet]
        public async Task<IActionResult> GetItems(string token)
        {
            try
            {
                var user = await _userService.Get(_tokenManagerService.User.Id);//new UserService().GetUser(_tokenManagerService.UserName, _tokenManagerService.UserHashKey);
                var x = _tokenManagerService.User.Id;

                var folders = await _folderService.GetAllItemsFromDatabase();
                var notes = await _noteService.GetAllItemsFromDatabase();
                var sharedFolders = await _sharedFolderService.GetAllItems(user.Id);
                var sharedNotes = await _sharedNoteService.GetAllItems(user.Id);

                var foldersResult = new List<Folder>();
                var notesResult = new List<Note>();
                var result = new List<object>();

                foreach(var sharedFolder in sharedFolders)
                {
                    foreach (var folder in folders)
                    {
                        if (sharedFolder.FolderId == folder.LocalId)
                        {
                            folder.Role = sharedFolder.Role;
                            foldersResult.Add(folder);
                        }
                    }
                }

                foreach (var sharedNote in sharedNotes)
                {
                    foreach (var note in notes)
                    {
                        if (sharedNote.NoteId == note.LocalId)
                        {
                            note.Role = sharedNote.Role;
                            notesResult.Add(note);
                        }
                    }
                }

                result.Add(Json(foldersResult).Value);
                result.Add(Json(notesResult).Value);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        [JustNotesAuthorize]
        [HttpGet("Note/{id}")]
        public async Task<IActionResult> GetSharedNote(string id, string token)
        {
            var user = await _userService.Get(_tokenManagerService.User.Id);
            var sharedNotes = await _sharedNoteService.GetAllItems(user.Id);
            var note = await _noteService.Get(id);
            var pictures = await _pictureService.GetAllItemsFromFolder(id);
            var picturesResult = new List<string>();

            foreach (var sharedNote in sharedNotes)
            {
                if (sharedNote.NoteId == note.LocalId)
                {
                    note.Role = sharedNote.Role;
                }
            }
            foreach (var picture in pictures)
            {
                picturesResult.Add(picture.ImageCode);
            }

            var result = new List<object>() { Json(note).Value, Json(picturesResult).Value };

            return Ok(result);
        }

        [JustNotesAuthorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemsFromFolder(string id, string token)
        {
            try
            {
                var user = await _userService.Get(_tokenManagerService.User.Id);
                var notes = await _noteService.GetAllItemsFromFolder(id);
                var sharedNotes = await _sharedNoteService.GetAllItems(user.Id);
                var result = new List<Note>();

                foreach (var note in notes)
                {
                    foreach (var sharedNote in sharedNotes)
                    {
                        if (note.LocalId == sharedNote.NoteId)
                        {
                            note.Role = sharedNote.Role;
                            result.Add(note);
                        }
                    }
                }

                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        [JustNotesAuthorize]
        [HttpPost("Folder/{id}")]
        public async Task<IActionResult> CreateFolderAccess(string id, string token, [FromBody]Object inputValue)
        {
            try
            {
                var userEmail = JObject.Parse(inputValue.ToString()).Value<String>("UserEmail");
                var user = await new UserService().GetUserByEmail(userEmail);
                var role = JObject.Parse(inputValue.ToString()).Value<String>("Role");

                var sharedFolderModel = new SharedFolder()
                {
                    UserId = user.Id,
                    FolderId = id,
                    Role = role
                };

                await _sharedFolderService.Create(sharedFolderModel);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [JustNotesAuthorize]
        [HttpPost("Create/Note/{id}")]
        public async Task<IActionResult> CreateNewNote(string id, string token, [FromBody]Note note)
        {
            try
            {
                var parentFolder = await _folderService.Get(id);

                note.NoteDate = DateTime.Now;
                note.UserId = parentFolder.UserId;
                note.FolderId = id;

                await _noteService.Create(note);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [JustNotesAuthorize]
        [HttpPost("Note/{id}")]
        public async Task<IActionResult> CreateNewNoteAccess(string id, string token, [FromBody]Object inputValue)
        {
            try
            {
                var userEmail = JObject.Parse(inputValue.ToString()).Value<String>("UserEmail");
                var user = await new UserService().GetUserByEmail(userEmail);

                var sharedNoteModel = new SharedNote()
                {
                    UserId = user.Id,
                    NoteId = id,
                    Role = JObject.Parse(inputValue.ToString()).Value<String>("Role")
                };

                await _sharedNoteService.Create(sharedNoteModel);
                return Ok();
            }
            catch
            {
                var senderEmai = _tokenManagerService.User.Email;
                var setterEmail = JObject.Parse(inputValue.ToString()).Value<String>("UserEmail");
                var emailService = new EmailService();
                var result = await emailService.ShareItemMessageBuild(senderEmai, setterEmail, id);

                return Ok(result);
            }
        }
    }
}