using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace JustNote.Controllers
{
    [Route("api/Access")]
    public class AvailableItemsController : Controller
    {
        private string userName;
        private string hashKey;
        private FolderService folderData = new FolderService();
        private NoteService note = new NoteService();
        private SharedService access = new SharedService();
        
        [HttpGet]
        public IActionResult GetItems(string token)
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                User user = new UserService().GetUser(userName, hashKey).GetAwaiter().GetResult();

                IEnumerable<Object> result = access.GetAvailableItems(user.Id).GetAwaiter().GetResult();

                return Ok(result);
            }
            return Unauthorized();
        }
        [HttpGet("{id}")]
        public IActionResult GetItemsFromFolder(string id, string token)
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                User user = new UserService().GetUser(userName, hashKey).GetAwaiter().GetResult();

                IEnumerable<Object> result = access.GetAvailableItemsFromFolder(id, user.Id).GetAwaiter().GetResult();

                return Ok(result);
            }
            return Unauthorized();
        }
        [HttpPost("Create/Folder/{id}")]
        public IActionResult CreateNewNote(string id, string token, [FromBody]Folder folder)
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                User user = new UserService().GetUser(userName, hashKey).GetAwaiter().GetResult();

                folder.FolderDate = DateTime.Now;
                folder.UserId = new FolderService().GetFolder(id).GetAwaiter().GetResult().UserId;
                folder.ParentFolderId = id;

                new FolderService().CreateFolder(folder).GetAwaiter().GetResult();
                return Ok();
            }

            return Unauthorized();
        }
        [HttpPost("Folder/{id}")]
        public IActionResult GetFolderAccess(string id, string token, [FromBody]Object inputValue)
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                string userEmail = JObject.Parse(inputValue.ToString()).Value<String>("UserEmail");
                string role = JObject.Parse(inputValue.ToString()).Value<String>("Role");
                string userId = new UserService().GetUserByEmail(userEmail).GetAwaiter().GetResult().Id;

                access.CreateNewFolderAccess(userId, id, role).GetAwaiter().GetResult();
                return Ok();
            }

            return Unauthorized();
        }
        [HttpPost("Create/Note/{id}")]
        public IActionResult CreateNewNote(string id, string token, [FromBody]Note note)
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                User user = new UserService().GetUser(userName, hashKey).GetAwaiter().GetResult();

                note.NoteDate = DateTime.Now;
                note.UserId = new FolderService().GetFolder(id).GetAwaiter().GetResult().UserId;
                note.FolderId = id;

                new NoteService().CreateNote(note).GetAwaiter().GetResult();
                return Ok();
            }

            return Unauthorized();
        }
        [HttpPost("Note/{id}")]
        public IActionResult GetNoteAccess(string id, string token, [FromBody]Object inputValue)
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                string userEmail = JObject.Parse(inputValue.ToString()).Value<String>("UserEmail");
                string role = JObject.Parse(inputValue.ToString()).Value<String>("Role");
                string userId = new UserService().GetUserByEmail(userEmail).GetAwaiter().GetResult().Id;
                
                access.CreateNewNoteAccess(userId, id, role).GetAwaiter().GetResult();
                return Ok();
            }

            return Unauthorized();
        }
    }
}