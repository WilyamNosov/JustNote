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
    [Route("api/Note")]
    [ApiController]
    public class NoteController : Controller
    {
        private string userName;
        private string hashKey;
        private NoteService noteData = new NoteService();

        //[JustNotesAuthorize]
        [HttpGet("{id}")]
        public async Task<Note> Get(string id)
        {
            return await noteData.GetNote(id);
        }

        [HttpPost]
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
            }

            return Unauthorized();
        }
        [HttpPost("{id}")]
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
            }

            return Unauthorized();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string token, string id, [FromBody] Note note)
        {
            var tokenManagerService = new TokenManagerService();
            if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
            {
                await noteData.UpdateNote(id, note);
                return Ok();
            }

            return Unauthorized();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string token, string id)
        {
            var tokenManagerService = new TokenManagerService();
            if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
            {
                await noteData.DeleteNote(id);
                return Ok();
            }

            return Unauthorized();
        }
    }
}
