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
        private NoteService noteData = new NoteService();
        private TokenManagerService _tokenManagerService;

        public NoteController(TokenManagerService tokenManagerService)
        {
            _tokenManagerService = tokenManagerService;
        }

        [JustNotesAuthorize]
        [HttpGet("{id}")]
        public async Task<Note> Get(string id, string token)
        {
            return await noteData.GetNote(id);
        }

        [JustNotesAuthorize]
        [HttpPost]
        public async Task<IActionResult> Post(string token, [FromBody] Note note, string folderId = null)
        {
            var user = await new UserService().GetUser(_tokenManagerService.UserName, _tokenManagerService.UserHashKey);
            note.NoteDate = DateTime.Now;
            note.UserId = user.Id;
            note.FolderId = folderId;

            await noteData.CreateNote(note);
            return Ok();
        }

        [JustNotesAuthorize]
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(string id, string token, [FromBody] Note note)
        {
            var user = await new UserService().GetUser(_tokenManagerService.UserName, _tokenManagerService.UserHashKey);
            note.NoteDate = DateTime.Now;
            note.UserId = user.Id;
            note.FolderId = id;

            await noteData.CreateNote(note);
            return Ok();
        }

        [JustNotesAuthorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string token, string id, [FromBody] Note note)
        {
            await noteData.UpdateNote(id, note);
            return Ok();
        }

        [JustNotesAuthorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string token, string id)
        {
            await noteData.DeleteNote(id);
            return Ok();
        }
    }
}
