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

        private readonly XrmFakedContext _sut;

        public AccountCreateNotePluginTests()
        {
            _sut = new XrmFakedContext();
        }

        [Fact]
        public void Plugin_Should_Create_Annotation_On_Account_Create()
        {
            // Arrange
            var entity = PrepareTestData(_sut, "account");
            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                entity,
                "Create",
                _sut);

            // Act
            _sut.ExecutePluginWith<AccountCreateNotePlugin>(pluginContext);
            var createdNotes = _sut.CreateQuery("annotation");
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
            var entity = PrepareTestData(_sut, "account");
            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                entity,
                "Update",
                _sut);

            //Act
            _sut.ExecutePluginWith<AccountCreateNotePlugin>(pluginContext);
            var createdNotes = _sut.CreateQuery("annotation");

            //Assert
            Assert.Empty(createdNotes);
        }

        [Fact]
        public void Plugin_Should_Not_Create_Annotation_On_Account_Create_If_No_Target_Is_Available()
        {
            //Arrange
            var entity = PrepareTestData(_sut, "account");
            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                entity,
                "Create",
                _sut);
            pluginContext.InputParameters = new ParameterCollection();

            //Act
            _sut.ExecutePluginWith<AccountCreateNotePlugin>(pluginContext);
            var createdNotes = _sut.CreateQuery("annotation");

            //Assert
            Assert.Empty(createdNotes);
        }

        [Fact]
        public void Plugin_Should_Not_Create_Annotation_On_Account_Create_If_Entity_Not_account()
        {
            //Arrange
            var entity = PrepareTestData(_sut, "user");
            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                entity,
                "Create",
                _sut);

            //Act
            _sut.ExecutePluginWith<AccountCreateNotePlugin>(pluginContext);
            var createdNotes = _sut.CreateQuery("annotation");

            //Assert
            Assert.Empty(createdNotes);
        }

        private static Entity PrepareTestData(XrmFakedContext sut, string entityName)
        {
            var entity = new Entity(entityName)
            {
                Id = Guid.NewGuid()
            };

            sut.Initialize(new[] { entity });
            return entity;
        }

    }
}