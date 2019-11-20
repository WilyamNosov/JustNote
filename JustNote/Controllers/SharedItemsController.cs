using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Attributes;
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace JustNotes.Controllers
{
    [Route("api/Access")]
    [ApiController]
    public class SharedItemsController : Controller
    {
        private TokenManagerService _tokenManagerService;
        private FolderService folderData = new FolderService();
        private NoteService noteData = new NoteService();
        private SharedService access = new SharedService();

        public SharedItemsController(TokenManagerService tokenManagerService)
        {
            _tokenManagerService = tokenManagerService;
        }

        [JustNotesAuthorize]
        [HttpGet]
        public async Task<IActionResult> GetItems(string token)
        {
            try
            {
                var user = await new UserService().GetUser(_tokenManagerService.UserName, _tokenManagerService.UserHashKey);
                var result = await access.GetAvailableItems(user.Id);
                return Ok(result);
            }
            catch
            {
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

                var result = await access.GetAvailableItemsFromFolder(id, user.Id);
                var parentFolder = await folderData.Get(id);
                var previusparent = new List<Object>() { new TimedModel() { PreviouseParent = parentFolder.ParentFolderId} };

                return Ok(result.Concat(previusparent));
            }
            catch
            {
                return BadRequest();
            }
        }

        [JustNotesAuthorize]
        [HttpPost("Create/Folder/{id}")]
        public async Task<IActionResult> CreateNewFoler(string id, string token, [FromBody]Folder folder)
        {
            try
            {
                User user = await new UserService().GetUser(_tokenManagerService.UserName, _tokenManagerService.UserHashKey);

                var parentFolder = await folderData.Get(id);

                folder.FolderDate = DateTime.Now;
                folder.UserId = parentFolder.UserId;
                folder.ParentFolderId = id;

                await folderData.Create(folder);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [JustNotesAuthorize]
        [HttpPost("Folder/{id}")]
        public async Task<IActionResult> GetFolderAccess(string id, string token, [FromBody]Object inputValue)
        {
            try
            {
                string userEmail = JObject.Parse(inputValue.ToString()).Value<String>("UserEmail");
                string role = JObject.Parse(inputValue.ToString()).Value<String>("Role");
                string userId = new UserService().GetUserByEmail(userEmail).GetAwaiter().GetResult().Id;

                await access.CreateNewFolderAccess(userId, id, role);
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
                User user = new UserService().GetUser(_tokenManagerService.UserName, _tokenManagerService.UserHashKey).GetAwaiter().GetResult();

                var parentFolder = await folderData.Get(id);

                note.NoteDate = DateTime.Now;
                note.UserId = parentFolder.UserId;
                note.FolderId = id;

                await noteData.Create(note);
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
                var role = JObject.Parse(inputValue.ToString()).Value<String>("Role");
                var user = await new UserService().GetUserByEmail(userEmail);

                await access.CreateNewNoteAccess(user.Id, id, role);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}