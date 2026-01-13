using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using PluginSample.FakeItTests.Helopers;
using PluginSample.Plugins;
using System;
using Xunit;

namespace PluginSample.Tests
{
    public class AccountUpdatePluginTests
    {
        // FakeXrmEasy v1 is intentionally used here.
        // Newer versions require .NET Core and are not compatible with D365 plugins.
        private readonly XrmFakedContext _sut;

        public AccountUpdatePluginTests()
        {
            _sut = new XrmFakedContext();
        }

        [Fact]
        public void Plugin_Should_Set_Description_On_Update()
        {
            ////Arrange
            var entity = PrepareTestData("account");
            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                entity,
                "Update",
                _sut);

            ////Act
            _sut.ExecutePluginWith<AccountUpdatePlugin>(pluginContext);

            //Assert
            Assert.Equal("Updated by plugin", entity["description"]);
        }


        [Fact]
        public void Plugin_Should_Not_Set_Description_If_Not_Update()
        {
            //Arrange
            var entity = PrepareTestData("account");
            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                entity,
                "Create",
                _sut);

            //Act
            _sut.ExecutePluginWith<AccountUpdatePlugin>(pluginContext);

            //Assert
            Assert.Equal("Test description", entity["description"]);
        }

        [Fact]
        public void Plugin_Should_Not_Set_Description_If_No_Target_Is_Available()
        {
            //Arrange
            var entity = PrepareTestData("account");
            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                entity,
                "Update",
                _sut);
            pluginContext.InputParameters = new ParameterCollection();

            //Act
            _sut.ExecutePluginWith<AccountUpdatePlugin>(pluginContext);

            //Assert
            Assert.Equal("Test description", entity["description"]);
        }

        [Fact]
        public void Plugin_Should_Not_Set_Description_If_Entity_Not_account()
        {
            //Arrange
            var entity = PrepareTestData("mail");
            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                entity,
                "Update",
                _sut);

            //Act
            _sut.ExecutePluginWith<AccountUpdatePlugin>(pluginContext);

            //Assert
            Assert.Equal("Test description", entity["description"]);
        }
        private static Entity PrepareTestData(string entityName)
        {
            return new Entity(entityName)
            {
                Id = Guid.NewGuid(),
                ["description"] = "Test description"
            };
        }

    }
}