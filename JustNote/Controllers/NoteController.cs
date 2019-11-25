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
    [Route("api/Note")]
    [ApiController]
    public class NoteController : Controller
    {
        private IDatabaseItemService<Note> _noteService;
        private TokenManagerService _tokenManagerService;

        public NoteController(TokenManagerService tokenManagerService, IDatabaseItemService<Note> noteService)
        {
            _tokenManagerService = tokenManagerService;
            _noteService = noteService;
        }

        [JustNotesAuthorize]
        [HttpGet("{id}")]
        public async Task<Note> Get(string id, string token)
        {
            return await _noteService.Get(id);
        }

        [JustNotesAuthorize]
        [HttpPost]
        public async Task<IActionResult> Post(string token, [FromBody] Note note, string folderId = null)
        {
            note.NoteDate = DateTime.Now;
            note.UserId = _tokenManagerService.User.Id;
            note.FolderId = folderId;

            await _noteService.Create(note);
            return Ok();
        }

        [JustNotesAuthorize]
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(string id, string token, [FromBody] Note note)
        {
            note.NoteDate = DateTime.Now;
            note.UserId = _tokenManagerService.User.Id;
            note.FolderId = id;

            await _noteService.Create(note);
            return Ok();
        }

        [JustNotesAuthorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string token, string id, [FromBody] Note note)
        {
            await _noteService.Update(id, note);
            return Ok();
        }

        [JustNotesAuthorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string token, string id)
        {
            await _noteService.Delete(id);
            return Ok();
        }
    }
}
