using JustNote.Serivces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JustNote.Tests
{
    public class EmailServiceTests
    {
        [Fact]
        public async void ShareItemMessageBuildTests()
        {
            string senderEmail = "";
            string getterEmail = "";
            string itemId = "";

            Mock<EmailService> mock = new Mock<EmailService>();

            mock.Setup(emailService => emailService.ShareItemMessageBuild(senderEmail, getterEmail, itemId)).ReturnsAsync(true);

            var target = mock.Object;

            var result = await target.ShareItemMessageBuild(senderEmail, getterEmail, itemId);

            Assert.True(result);
        }
    }
}
