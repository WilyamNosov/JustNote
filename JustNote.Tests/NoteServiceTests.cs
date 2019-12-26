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
        public async void TestGet() 
        {
            var id = "1576154839832";
            Mock<IDatabaseItemService<Note>> mock = new Mock<IDatabaseItemService<Note>>();

            mock.Setup(noteService => noteService.Get(id)).ReturnsAsync(new Note());
            var target = mock.Object;

            var result = await target.Get(id);

            Assert.Equal(new Note(), result);

        }
    }
}
