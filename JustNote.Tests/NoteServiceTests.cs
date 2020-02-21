using JustNote.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using JustNote.Serivces;
using Moq;

namespace JustNote.Tests
{
    public class NoteServiceTests
    {
        [Fact]
        public async void TestGets() 
        {
            var id = "";
            Mock<IDatabaseItemService<Note>> mock = new Mock<IDatabaseItemService<Note>>();

            mock.Setup(noteService => noteService.Get(id)).ReturnsAsync(new Note());
            mock.Setup(noteService => noteService.GetAllItems(id)).ReturnsAsync(new List<Note>());
            mock.Setup(noteService => noteService.GetAllItemsFromDatabase()).ReturnsAsync(new List<Note>());
            mock.Setup(noteService => noteService.GetAllItemsFromFolder(id)).ReturnsAsync(new List<Note>());
            
            var target = mock.Object;

            var resultSingle = await target.Get(id);
            var resultAllUserNotes = await target.GetAllItems(id);
            var resultAllNotesFromDb = await target.GetAllItemsFromDatabase();
            var resultAllNotesFromFolder = await target.GetAllItemsFromFolder(id);

            Assert.Equal(new Note().GetType(), resultSingle.GetType());
            Assert.Equal(new List<Note>(), resultAllUserNotes);
            Assert.Equal(new List<Note>(), resultAllNotesFromDb);
            Assert.Equal(new List<Note>(), resultAllNotesFromFolder);
        }
    }
}
