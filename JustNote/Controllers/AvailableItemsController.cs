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
        
        [HttpGet]
        public IActionResult GetItems(string token)
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                User user = new UserService().GetUser(userName, hashKey).GetAwaiter().GetResult();

                IEnumerable<Object> folders = folderData.GetAvailableFolders(user.Id).GetAwaiter().GetResult();
                IEnumerable<Object> notes = note.GetAvailableNotes(user.Id).GetAwaiter().GetResult();

                IEnumerable<Object> result = folders.Concat(notes);

                return Ok(result);
            }
            return Unauthorized();
        }
        [HttpPost("Folder/{id}")]
        public IActionResult GetFolderAccess(string token)
        {
            return null;
        }
        [HttpPost("Note/{id}")]
        public IActionResult GetNoteAccess(string id, string token, [FromBody]Object inputValue)
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                string userEmail = JObject.Parse(inputValue.ToString()).Value<String>("UserEmail");


            }

            return null;
        }
    }
}