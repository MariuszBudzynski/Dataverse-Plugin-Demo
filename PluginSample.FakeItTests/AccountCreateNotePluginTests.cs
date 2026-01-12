using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using PluginSample.FakeItTests.Helopers;
using PluginSample.Plugins;
using System;
using System.Linq;
using Xunit;

namespace PluginSample.FakeItTests
{
    public class AccountCreateNotePluginTests
    {
        // FakeXrmEasy v1 is intentionally used here.
        // Newer versions require .NET Core and are not compatible with D365 plugins.

        [Fact]
        public void Plugin_Should_Create_Annotation_On_Account_Create()
        {
            // Arrange
            var fakeContext = new XrmFakedContext();
            var account = new Entity("account")
            {
                Id = Guid.NewGuid()
            };

            fakeContext.Initialize(new[] { account });

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                account,
                "Create",
                fakeContext);

            // Act
            fakeContext.ExecutePluginWith<AccountCreateNotePlugin>(pluginContext);
            var createdNotes = fakeContext.CreateQuery("annotation");
            var note = createdNotes.FirstOrDefault();
            var subject = note.GetAttributeValue<string>("subject");
            var notetext = note.GetAttributeValue<string>("notetext");

            // Assert
            Assert.Single(createdNotes);
            Assert.Equal("Account created", subject);
            Assert.Equal("This note was created automatically by a plugin.", notetext);
        }

        [Fact]
        public void Plugin_Should_Not_Create_Annotation_On_Account_Create_If_Message_Not_Create()
        {
            //Arrange
            var fakeContext = new XrmFakedContext();
            var account = new Entity("account")
            {
                Id = Guid.NewGuid()
            };

            fakeContext.Initialize(new[] { account });

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                account,
                "Update",
                fakeContext);

            //Act
            fakeContext.ExecutePluginWith<AccountCreateNotePlugin>(pluginContext);
            var createdNotes = fakeContext.CreateQuery("annotation");

            //Assert
            Assert.Empty(createdNotes);
        }

        [Fact]
        public void Plugin_Should_Not_Create_Annotation_On_Account_Create_If_No_Target_Is_Available()
        {
            //Arrange
            var fakeContext = new XrmFakedContext();
            var account = new Entity("account")
            {
                Id = Guid.NewGuid()
            };

            fakeContext.Initialize(new[] { account });

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                account,
                "Create",
                fakeContext);
            pluginContext.InputParameters = new ParameterCollection();

            //Act
            fakeContext.ExecutePluginWith<AccountCreateNotePlugin>(pluginContext);
            var createdNotes = fakeContext.CreateQuery("annotation");

            //Assert
            Assert.Empty(createdNotes);
        }

        [Fact]
        public void Plugin_Should_Not_Create_Annotation_On_Account_Create_If_Entity_Not_account()
        {
            //Arrange
            var fakeContext = new XrmFakedContext();
            var account = new Entity("users")
            {
                Id = Guid.NewGuid()
            };

            fakeContext.Initialize(new[] { account });

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                account,
                "Create",
                fakeContext);

            //Act
            fakeContext.ExecutePluginWith<AccountCreateNotePlugin>(pluginContext);
            var createdNotes = fakeContext.CreateQuery("annotation");

            //Assert
            Assert.Empty(createdNotes);
        }

    }
}