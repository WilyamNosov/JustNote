using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public Note Get(string id)
        {
            return noteData.GetNote(id).GetAwaiter().GetResult();
        }

        [HttpPost]
        public IActionResult Post(string token, [FromBody] Note note, string folderId = null)
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
        [HttpPost("{id}")]
        public IActionResult Post(string id, string token, [FromBody] Note note)
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
        public IActionResult Put(string token, string id, [FromBody] Note note)
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
        public IActionResult Delete(string token, string id)
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