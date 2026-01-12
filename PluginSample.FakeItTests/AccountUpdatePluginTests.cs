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

        [Fact]
        public void Plugin_Should_Set_Description_On_Update()
        {
            ////Arrange
            var fakeContext = new XrmFakedContext();
            var entity = new Entity("account")
            {
                Id = Guid.NewGuid(),
                ["description"] = "Test description"
            };
            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                entity,
                "Update",
                fakeContext);

            ////Act
            fakeContext.ExecutePluginWith<AccountUpdatePlugin>(pluginContext);

            //Assert
            Assert.Equal("Updated by plugin", entity["description"]);
        }

        [Fact]
        public void Plugin_Should_Not_Set_Description_If_Not_Update()
        {
            //Arrange
            var fakeContext = new XrmFakedContext();
            var entity = new Entity("account")
            {
                Id = Guid.NewGuid(),
                ["description"] = "Test description"
            };
            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                entity,
                "Create",
                fakeContext);

            //Act
            fakeContext.ExecutePluginWith<AccountUpdatePlugin>(pluginContext);

            //Assert
            Assert.Equal("Test description", entity["description"]);
        }

        [Fact]
        public void Plugin_Should_Not_Set_Description_If_No_Target_Is_Available()
        {
            //Arrange
            var fakeContext = new XrmFakedContext();
            var entity = new Entity("account")
            {
                Id = Guid.NewGuid(),
                ["description"] = "Test description"
            };
            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                entity,
                "Update",
                fakeContext);
            pluginContext.InputParameters = new ParameterCollection();

            //Act
            fakeContext.ExecutePluginWith<AccountUpdatePlugin>(pluginContext);

            //Assert
            Assert.Equal("Test description", entity["description"]);
        }

        [Fact]
        public void Plugin_Should_Not_Set_Description_If_Entity_Not_account()
        {
            //Arrange
            var fakeContext = new XrmFakedContext();
            var entity = new Entity("mail")
            {
                Id = Guid.NewGuid(),
                ["description"] = "Test description"
            };
            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                entity,
                "Update",
                fakeContext);

            //Act
            fakeContext.ExecutePluginWith<AccountUpdatePlugin>(pluginContext);

            //Assert
            Assert.Equal("Test description", entity["description"]);
        }
    }
}