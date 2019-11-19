using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace JustNotes.Controllers
{
    [Route("api/Access")]
    [ApiController]
    public class AvailableItemsController : Controller
    {
        private string userName;
        private string hashKey;
        private FolderService folderData = new FolderService();
        private NoteService note = new NoteService();
        private SharedService access = new SharedService();

        [HttpGet]
        public async Task<IActionResult> GetItems(string token)
        {
            try
            {
                var tokenManagerService = new TokenManagerService();
                if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
                {
                    var user = await new UserService().GetUser(userName, hashKey);
                    var result = await access.GetAvailableItems(user.Id);
                    return Ok(result);
                }
                return Unauthorized();

            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemsFromFolder(string id, string token)
        {
            try
            {
                var tokenManagerService = new TokenManagerService();
                if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
                {
                    var user = await new UserService().GetUser(userName, hashKey);

                    var result = await access.GetAvailableItemsFromFolder(id, user.Id);
                    var parentFolder = await folderData.GetFolder(id);
                    var previusparent = new List<Object>() { new TimedModel() { PreviouseParent = parentFolder.ParentFolderId } };

                    return Ok(result.Concat(previusparent));
                }
                return Unauthorized();
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost("Create/Folder/{id}")]
        public async Task<IActionResult> CreateNewFoler(string id, string token, [FromBody]Folder folder)
        {
            try
            {
                var tokenManagerService = new TokenManagerService();
                if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
                {
                    User user = await new UserService().GetUser(userName, hashKey);

                    var parentFolder = await new FolderService().GetFolder(id);

                    folder.FolderDate = DateTime.Now;
                    folder.UserId = parentFolder.UserId;
                    folder.ParentFolderId = id;

                    await new FolderService().CreateFolder(folder);
                    return Ok();
                }

                return Unauthorized();
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost("Folder/{id}")]
        public async Task<IActionResult> GetFolderAccess(string id, string token, [FromBody]Object inputValue)
        {
            try
            {
                var tokenManagerService = new TokenManagerService();
                if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
                {
                    string userEmail = JObject.Parse(inputValue.ToString()).Value<String>("UserEmail");
                    string role = JObject.Parse(inputValue.ToString()).Value<String>("Role");
                    string userId = new UserService().GetUserByEmail(userEmail).GetAwaiter().GetResult().Id;

                    await access.CreateNewFolderAccess(userId, id, role);
                    return Ok();
                }

                return Unauthorized();

            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost("Create/Note/{id}")]
        public async Task<IActionResult> CreateNewNote(string id, string token, [FromBody]Note note)
        {
            try
            {
                var tokenManagerService = new TokenManagerService();
                if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
                {
                    User user = new UserService().GetUser(userName, hashKey).GetAwaiter().GetResult();

                    var parentFolder = await new FolderService().GetFolder(id);

                    note.NoteDate = DateTime.Now;
                    note.UserId = parentFolder.UserId;
                    note.FolderId = id;

                    await new NoteService().CreateNote(note);
                    return Ok();
                }

                return Unauthorized();

            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost("Note/{id}")]
        public async Task<IActionResult> CreateNewNoteAccess(string id, string token, [FromBody]Object inputValue)
        {
            try
            {
                var tokenManagerService = new TokenManagerService();
                if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
                {
                    var userEmail = JObject.Parse(inputValue.ToString()).Value<String>("UserEmail");
                    var role = JObject.Parse(inputValue.ToString()).Value<String>("Role");
                    var user = await new UserService().GetUserByEmail(userEmail);

                    await access.CreateNewNoteAccess(user.Id, id, role);
                    return Ok();
                }

                return Unauthorized();

            }
            catch
            {
                return BadRequest();
            }
        }
    }
}