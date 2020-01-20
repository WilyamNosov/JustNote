using JustNote.Serivces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JustNote.Tests
{
    public class ConfirmEmailServiceTests
    {
        [Fact]
        public async void TestConfirmMessage()
        {
            var token = "";
            Mock<ConfirmEmailService> mock = new Mock<ConfirmEmailService>();
            mock.Setup(confirmEmail => confirmEmail.AcceptConfirmMessage(token)).ReturnsAsync($"https://testawslambdas3bucket.s3.us-west-2.amazonaws.com/index.html");

            var target = mock.Object;

            var result = await target.AcceptConfirmMessage("");

            Assert.Equal($"https://testawslambdas3bucket.s3.us-west-2.amazonaws.com/index.html", result);
        }
    }
}
