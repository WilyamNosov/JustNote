using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JustNotes.Controllers
{
    [Route("api/Folder")]
    [ApiController]
    public class FolderController : Controller
    {

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

                IEnumerable<Object> result = folders.Concat(notes);

                return Ok(result);
            }
            return Unauthorized();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, string token)
        {
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
        }

        [HttpPost]
        public async Task<IActionResult> Post(string token, [FromBody] Folder folder)
        {
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
            }

            return Unauthorized();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, string token, [FromBody] Folder folder)
        {
            var tokenManagerService = new TokenManagerService();
            if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
            {
                await folderData.UpdateFolder(id, folder);

                return Ok();
            }

            return Unauthorized();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, string token)
        {
            var tokenManagerService = new TokenManagerService();
            if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
            {
                await folderData.DeleteFolder(id);

                return Ok();
            }

            return Unauthorized();
        }
    }
}
