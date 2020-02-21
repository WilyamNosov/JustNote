using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace JustNotes.Controllers
{
    [Route("api/Login")]
    [ApiController]
    public class LoginController : Controller
    {
        IDatabaseItemService<User> _userService;
        IDatabaseItemService<Folder> _folderService;
        IDatabaseItemService<Note> _noteService;
        IDatabaseItemService<Picture> _pictureService;
        TokenManagerService _tokenManagerService;

        public LoginController(IDatabaseItemService<User> userService, IDatabaseItemService<Folder> folderService, IDatabaseItemService<Note> noteService,
            IDatabaseItemService<Picture> pictureService, TokenManagerService tokenManagerService)
        {
            _userService = userService;
            _folderService = folderService;
            _noteService = noteService;
            _pictureService = pictureService;
            _tokenManagerService = tokenManagerService;
        }

        [HttpPost]
<<<<<<< HEAD
        public async Task<IActionResult> LoginUser(Login loginObject)
        {
            var userName = loginObject.UserName;
            var password = loginObject.UserPassword;
            
            var user = await new UserService().GetUser(userName, new HashKeyService().GetHashKey(password));
            
            if (user != null && user.ConfirmedEmail == true)
            {
                var token = new TokenManagerService().GenerateToken(userName, user.HashKey);
                return Ok(token);
            }
             
            return Unauthorized();
=======
        public async Task<IActionResult> LoginUser(Object inputValue)
        {
            var userName = JObject.Parse(inputValue.ToString()).Value<String>("UserName");
            var password = JObject.Parse(inputValue.ToString()).Value<String>("UserPassword");
            var hashKey = new HashKeyService().GetHashKey(password);

            try
            {
                var user = await new UserService().GetUser(userName, hashKey);
                var token = _tokenManagerService.GenerateToken(userName, user.HashKey);

                var folders = Json(await _folderService.GetAllItems(user.Id));
                var notes = await _noteService.GetAllItems(user.Id);
                var pcitures = new List<PictureLoginReturn>();
                
                var x = await _noteService.GetAllItems(user.Id);

                foreach(var note in notes)
                {
                    note.ImageArray = note.LocalId;
                    var picturesInNote = await _pictureService.GetAllItemsFromFolder(note.LocalId);
                    var base64Array = new List<string>();
                    foreach (var pictureInNote in picturesInNote)
                    {
                        base64Array.Add(pictureInNote.ImageCode);
                    }
                    
                    pcitures.Add(new PictureLoginReturn() { Id = note.LocalId, ImageArray = base64Array});
                }

                var result = new List<Object>() { Json(token).Value, folders.Value, Json(notes).Value, Json(pcitures).Value };


                return Ok(result);
                //return Ok(token);
            } catch
            {
                return Unauthorized();
            }
>>>>>>> DatabaseData
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> ValidateUser(string id)
        {
            var userName = "";
            var userHashKey = "";
            var tokenManagerService = new TokenManagerService();

            if (tokenManagerService.ValidateToken(id, out userName, out userHashKey))
            {
                return Ok(id);
            }

            return Unauthorized();
        }
    }
}
