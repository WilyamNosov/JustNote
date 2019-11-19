using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
<<<<<<< Updated upstream
=======
using JustNote.Attributes;
//using JustNote.Attributes.Authorization;
>>>>>>> Stashed changes
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Mvc;

namespace JustNote.Controllers
{
    [Route("api/Note")]
    [ApiController]
    public class NoteController : Controller
    {
        private string userName;
        private string hashKey;
        private NoteService noteData = new NoteService();

        [HttpGet("{id}")]
<<<<<<< Updated upstream
        public Note Get(string id)
=======
        [JustNotesAuthorize]
        public async Task<Note> GetNote(string id, string token)
>>>>>>> Stashed changes
        {
            return noteData.GetNote(id).GetAwaiter().GetResult();
        }

        [HttpPost]
<<<<<<< Updated upstream
        public IActionResult Post(string token, [FromBody] Note note, string folderId = null)
=======
        public async Task<IActionResult> CreateNote(string token, [FromBody] Note note, string folderId = null)
>>>>>>> Stashed changes
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                User user = new UserService().GetUser(userName, hashKey).GetAwaiter().GetResult();
                note.NoteDate = DateTime.Now;
                note.UserId = user.Id;
                note.FolderId = folderId;

                noteData.CreateNote(note).GetAwaiter().GetResult();
                return Ok();
            }

            return Unauthorized();
        }
        [HttpPost("Many")]
        public async Task<IActionResult> CreateNotes(string token, [FromBody] List<Note> notelist, string folderId = null)
        {
            var tokenManagerService = new TokenManagerService();
            if (tokenManagerService.ValidateToken(token, out userName, out hashKey))
            {
                foreach (Note note in notelist)
                {
                    var user = await new UserService().GetUser(userName, hashKey);
                    note.Id = null;
                    note.NoteDate = DateTime.Now;
                    note.UserId = user.Id;
                    note.FolderId = folderId;

                    await noteData.CreateNote(note);
                }
                return Ok();
            }

            return Unauthorized();
        }
        [HttpPost("{id}")]
<<<<<<< Updated upstream
        public IActionResult Post(string id, string token, [FromBody] Note note)
=======
        public async Task<IActionResult> CreateNoteInFolder(string id, string token, [FromBody] Note note)
>>>>>>> Stashed changes
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                User user = new UserService().GetUser(userName, hashKey).GetAwaiter().GetResult();
                note.NoteDate = DateTime.Now;
                note.UserId = user.Id;
                note.FolderId = id;

                noteData.CreateNote(note).GetAwaiter().GetResult();
                return Ok();
            }

            return Unauthorized();
        }

        [HttpPut("{id}")]
<<<<<<< Updated upstream
        public IActionResult Put(string token, string id, [FromBody] Note note)
=======
        public async Task<IActionResult> UpdateNote(string token, string id, [FromBody] Note note)
>>>>>>> Stashed changes
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                User user = new UserService().GetUser(userName, hashKey).GetAwaiter().GetResult();
                noteData.UpdateNote(id, user.Id, note).GetAwaiter().GetResult();

                return Ok();
            }

            return Unauthorized();
        }

        [HttpDelete("{id}")]
<<<<<<< Updated upstream
        public IActionResult Delete(string token, string id)
=======
        public async Task<IActionResult> DeleteNote(string token, string id)
>>>>>>> Stashed changes
        {
            if (new TokenManagerService().ValidateToken(token, out userName, out hashKey))
            {
                noteData.DeleteNote(id).GetAwaiter().GetResult();
                return Ok();
            }

            return Unauthorized();
        }
    }
}