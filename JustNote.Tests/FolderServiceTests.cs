using JustNote.Models;
using JustNote.Serivces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JustNote.Tests
{
    public class FolderServiceTests
    {
        [Fact]
        public async void TestGets()
        {
            var id = "";
            Mock<IDatabaseItemService<Folder>> mock = new Mock<IDatabaseItemService<Folder>>();

            mock.Setup(noteService => noteService.Get(id)).ReturnsAsync(new Folder());
            mock.Setup(noteService => noteService.GetAllItems(id)).ReturnsAsync(new List<Folder>());
            mock.Setup(noteService => noteService.GetAllItemsFromDatabase()).ReturnsAsync(new List<Folder>());
            mock.Setup(noteService => noteService.GetAllItemsFromFolder(id)).ReturnsAsync(new List<Folder>());

            var target = mock.Object;

            var resultSingle = await target.Get(id);
            var resultAllUserNotes = await target.GetAllItems(id);
            var resultAllNotesFromDb = await target.GetAllItemsFromDatabase();
            var resultAllNotesFromFolder = await target.GetAllItemsFromFolder(id);

            Assert.Equal(new Folder().GetType(), resultSingle.GetType());
            Assert.Equal(new List<Folder>(), resultAllUserNotes);
            Assert.Equal(new List<Folder>(), resultAllNotesFromDb);
            Assert.Equal(new List<Folder>(), resultAllNotesFromFolder);
        }
    }
}
