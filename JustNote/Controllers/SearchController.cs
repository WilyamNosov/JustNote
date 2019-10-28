using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Mvc;

namespace JustNote.Controllers
{
    [Route("api/Search")]
    [ApiController]
    public class SearchController : Controller
    {
        [HttpGet]
        public IEnumerable<object> Search(string searchRequest)
        {
            IEnumerable<Object> folders = new FolderService().GetFolderBySearchString(searchRequest).GetAwaiter().GetResult();
            IEnumerable<Object> notes = new NoteService().GetNoteBySearchString(searchRequest).GetAwaiter().GetResult();
            
            return folders.Concat(notes);
        }
    }
}