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
        private IDatabaseItemService<User> _userService;

        public SharedController(TokenManagerService tokenManagerService, IDatabaseItemService<Folder> folderService, IDatabaseItemService<Note> noteService,
            IDatabaseItemService<SharedFolder> sharedFolderService, IDatabaseItemService<SharedNote> sharedNoteService, IDatabaseItemService<User> userService)
        {
            _tokenManagerService = tokenManagerService;
            _folderService = folderService;
            _noteService = noteService;
            _sharedFolderService = sharedFolderService;
            _sharedNoteService = sharedNoteService;
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
                        if (sharedFolder.FolderId == folder.Id)
                        {
                            foldersResult.Add(folder);
                        }
                    }
                }

                foreach (var sharedNote in sharedNotes)
                {
                    foreach (var note in notes)
                    {
                        if (sharedNote.NoteId == note.Id)
                        {
                            notesResult.Add(note);
                        }
                    }
                }

                result.Add(Json(foldersResult).Value);
                result.Add(Json(notesResult).Value);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var x = ex.Message;
                return BadRequest();
            }
        }

        [JustNotesAuthorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemsFromFolder(string id, string token)
        {
            try
            {
                var user = await new UserService().GetUser(_tokenManagerService.UserName, _tokenManagerService.UserHashKey);

                var notes = await _noteService.GetAllItemsFromFolder(id);
                var previusparent = new List<Object>() { new TimedModel() { PreviouseParent = id } };
                var result = new List<object>() { Json(notes).Value, previusparent};

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
                return BadRequest();
            }
        }
    }
}