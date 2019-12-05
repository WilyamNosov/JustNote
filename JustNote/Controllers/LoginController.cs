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
        TokenManagerService _tokenManagerService;

        public LoginController(IDatabaseItemService<User> userService, IDatabaseItemService<Folder> folderService, IDatabaseItemService<Note> noteService, TokenManagerService tokenManagerService)
        {
            _userService = userService;
            _folderService = folderService;
            _noteService = noteService;
            _tokenManagerService = tokenManagerService;
        }

        [HttpPost]
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
                var notes = Json(await _noteService.GetAllItems(user.Id));
                var result = new List<Object>() { Json(token).Value, folders.Value, notes.Value };


                return Ok(result);
                //return Ok(token);
            } catch
            {
                return Unauthorized();
            }
        }
    }
}
