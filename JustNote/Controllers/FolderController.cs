using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Attributes;
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

        private TokenManagerService _tokenManagerService;
        private FolderService folderData = new FolderService();

        public FolderController(TokenManagerService tokenManagerService)
        {
            _tokenManagerService = tokenManagerService;
        }

        [JustNotesAuthorize]
        [HttpGet]
        public async Task<IActionResult> Get(string token)
        {
            var notedata = new NoteService();
            var user = await new UserService().GetUser(_tokenManagerService.UserName, _tokenManagerService.UserHashKey);

            IEnumerable<Object> folders = await folderData.GetAllUserFolders(user.Id);
            IEnumerable<Object> notes = await notedata.GetAllUserNotes(user.Id);
            IEnumerable<Object> result = folders.Concat(notes);

            return Ok(result);
        }

        [JustNotesAuthorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, string token)
        {
            var noteData = new NoteService();
            var parentFolder = await folderData.GetFolder(id);

            IEnumerable<Object> folders = await folderData.GetAllChildFolder(id);
            IEnumerable<Object> notes = await noteData.GetAllNotesFromFolder(id);
            IEnumerable<Object> result = folders.Concat(notes);
            IEnumerable<Object> x = new List<Object>() { new TimedModel() { PreviouseParent = parentFolder.ParentFolderId } };

            return Ok(result.Concat(x));
        }

        [JustNotesAuthorize]
        [HttpPost]
        public async Task<IActionResult> Post(string token, [FromBody] Folder folder)
        {
            var user = await new UserService().GetUser(_tokenManagerService.UserName, _tokenManagerService.UserHashKey);
            folder.FolderDate = DateTime.Now;
            folder.UserId = user.Id;
            folder.ParentFolderId = null;

            await folderData.CreateFolder(folder);
            return Ok();
        }

        [JustNotesAuthorize]
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(string id, string token, [FromBody] Folder folder)
        {
            var user = await new UserService().GetUser(_tokenManagerService.UserName, _tokenManagerService.UserHashKey);
            folder.FolderDate = DateTime.Now;
            folder.UserId = user.Id;
            folder.ParentFolderId = id;

            await folderData.CreateFolder(folder);
            return Ok();
        }

        [JustNotesAuthorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, string token, [FromBody] Folder folder)
        {
            await folderData.UpdateFolder(id, folder);
            return Ok();
        }

        [JustNotesAuthorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, string token)
        {
            await folderData.DeleteFolder(id);
            return Ok();
        }
    }
}
