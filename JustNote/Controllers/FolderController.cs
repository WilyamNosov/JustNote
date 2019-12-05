﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Attributes;
using JustNote.Datas;
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace JustNotes.Controllers
{
    [Route("api/Folder")]
    [ApiController]
    public class FolderController : Controller
    {

        private TokenManagerService _tokenManagerService;
        private IDatabaseItemService<Folder> _folderService;
        private IDatabaseItemService<Note> _noteService;

        public FolderController(TokenManagerService tokenManagerService, IDatabaseItemService<Folder> folderService, IDatabaseItemService<Note> noteService)
        {
            _tokenManagerService = tokenManagerService;
            _folderService = folderService;
            _noteService = noteService;
        }

        [JustNotesAuthorize]
        [HttpGet]
        public async Task<IActionResult> Get(string token)
        {
            var folders = Json(await _folderService.GetAllItems(_tokenManagerService.User.Id));
            var notes = Json(await _noteService.GetAllItems(_tokenManagerService.User.Id));
            var result = new List<Object>() { folders.Value, notes.Value };

            return Ok(result);
        }

        [JustNotesAuthorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, string token)
        {
            var notes = Json(await _noteService.GetAllItemsFromFolder(id));
            //var result = new List<Object>() { notes.Value, new TimedModel() { PreviouseParent = id } };

            return Ok(notes);
        }

        [JustNotesAuthorize]
        [HttpPost]
        public async Task<IActionResult> Post(string token, [FromBody] Folder folder)
        {
            folder.FolderDate = DateTime.Now;
            folder.UserId = _tokenManagerService.User.Id;

            await _folderService.Create(folder);
            return Ok();
        }
        [JustNotesAuthorize]
        [HttpPost("Synchronize")]
        public async Task<IActionResult> Synchronize(string token, [FromBody] Object items)
        {
            var foldersList = new List<Folder>();
            var notesList = new List<Note>();
            var jsonArray = new JArray(items).ElementAt(0);
            var folder = new Folder();
            var note = new Note();

            for (int i = 0; i < jsonArray.Count(); i++)
            {
                var item = jsonArray.ElementAt(i);

                if (item.Count() == 3 && item.Count() == 6)
                {
                    folder = item.ToObject<Folder>();
                    folder.UserId = _tokenManagerService.User.Id;
                    foldersList.Add(folder);
                }
                else if (item.Count() == 5 && item.Count() == 8)
                {
                    note = item.ToObject<Note>();
                    note.UserId = _tokenManagerService.User.Id;
                    notesList.Add(note);
                }
            }


            // move to services
            var curentFolders = await _folderService.GetAllItems(_tokenManagerService.User.Id);
            var curentNotes = await _noteService.GetAllItems(_tokenManagerService.User.Id);

            await DatabaseData.Folders.DeleteManyAsync(new BsonDocument("UserId", new ObjectId(_tokenManagerService.User.Id)));
            await DatabaseData.Notes.DeleteManyAsync(new BsonDocument("UserId", new ObjectId(_tokenManagerService.User.Id)));

            if (foldersList.Count > 0)
            {
                await DatabaseData.Folders.InsertManyAsync(foldersList);
            }
            if (notesList.Count > 0)
            {
                await DatabaseData.Notes.InsertManyAsync(notesList);
            }

            return Ok();
        }

        [JustNotesAuthorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, string token, [FromBody] Folder folder)
        {
            await _folderService.Update(id, folder);
            return Ok();
        }

        [JustNotesAuthorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, string token)
        {
            await _folderService.Delete(id);
            return Ok();
        }
    }
}
