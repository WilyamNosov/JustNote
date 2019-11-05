using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Mvc;

namespace JustNote.Controllers
{
    [Route("api/Folder")]
    [ApiController]
    public class FolderController : Controller
    {

        private string userName;
        private string hashKey;
        private FolderService folderData = new FolderService();
        [HttpGet]
        public IActionResult Get(string token)
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                NoteService note = new NoteService();
                User user = new UserService().GetUser(userName, hashKey).GetAwaiter().GetResult();

                IEnumerable<Object> folders = folderData.GetAllUserFolders(user.Id).GetAwaiter().GetResult();
                IEnumerable<Object> notes = note.GetAllUserNotes(user.Id).GetAwaiter().GetResult();

                IEnumerable<Object> result = folders.Concat(notes);

                return Ok(result);
            }
            return Unauthorized();
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id, string token)
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                NoteService note = new NoteService();
                Folder parentFolder = folderData.GetFolder(id).GetAwaiter().GetResult();

                IEnumerable<Object> folders = folderData.GetAllChildFolder(id).GetAwaiter().GetResult();
                IEnumerable<Object> notes = note.GetAllNotesFromFolder(id).GetAwaiter().GetResult();
                IEnumerable<Object> result = folders.Concat(notes);

                IEnumerable<Object> x = new List<Object>() { new TimedModel() { PreviouseParent = parentFolder.ParentFolderId } };

                return Ok(result.Concat(x));
            }
            return Unauthorized();
        }

        [HttpPost]
        public IActionResult Post(string token, [FromBody] Folder folder)
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                User user = new UserService().GetUser(userName, hashKey).GetAwaiter().GetResult();
                folder.FolderDate = DateTime.Now;
                folder.UserId = user.Id;
                folder.ParentFolderId = null;

                folderData.CreateFolder(folder).GetAwaiter().GetResult();
                return Ok();
            }

            return Unauthorized();
        }
        [HttpPost("{id}")]
        public IActionResult Post(string id, string token, [FromBody] Folder folder)
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                User user = new UserService().GetUser(userName, hashKey).GetAwaiter().GetResult();
                folder.FolderDate = DateTime.Now;
                folder.UserId = user.Id;
                folder.ParentFolderId = id;

                folderData.CreateFolder(folder).GetAwaiter().GetResult();
                return Ok();
            }

            return Unauthorized();
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, string token, [FromBody] Folder folder)
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                folderData.UpdateFolder(id, folder).GetAwaiter().GetResult();

                return Ok();
            }

            return Unauthorized();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id, string token)
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                folderData.DeleteFolder(id).GetAwaiter().GetResult();

                return Ok();
            }

            return Unauthorized();
        }
    }
}