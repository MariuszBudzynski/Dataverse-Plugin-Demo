using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using PluginSample.FakeItTests.Helopers;
using PluginSample.Plugins;
using System;
using System.Linq;
using Xunit;

namespace PluginSample.FakeItTests
{
    public class AccountDeletePluginTests
    {
        // FakeXrmEasy v1 is intentionally used here.
        // Newer versions require .NET Core and are not compatible with D365 plugins.

        [Fact]
        public void Plugin_Should_Delete_ChildEntity_On_Account_Delete()
        {
            // Arrange
            var fakeContext = new XrmFakedContext();
            var stage = 20;
            var accountId = Guid.NewGuid();
            var entity = new Entity("account")
            {
                Id = accountId
            };
            var child1 = new Entity("new_childentity")
            {
                Id = Guid.NewGuid(),
                ["new_parentaccountid"] = new EntityReference("account", accountId)
            };
            var child2 = new Entity("new_childentity")
            {
                Id = Guid.NewGuid(),
                ["new_parentaccountid"] = new EntityReference("account", accountId)
            };
            var accountEntityReference = new EntityReference("account", accountId);

            fakeContext.Initialize(new[] { entity, child1, child2 });

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                accountEntityReference,
                "Delete",
                fakeContext,
                stage);

            // Act
            fakeContext.ExecutePluginWith<AccountDeletePlugin>(pluginContext);
            var results = fakeContext.CreateQuery("new_childentity").ToList();

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void Plugin_Should_Not_Delete_ChildEntity_On_Account_Delete_If_Message_Not_Delete()
        {
            //Arrange
            var fakeContext = new XrmFakedContext();
            var stage = 20;
            var accountId = Guid.NewGuid();
            var entity = new Entity("account")
            {
                Id = accountId
            };
            var child1 = new Entity("new_childentity")
            {
                Id = Guid.NewGuid(),
                ["new_parentaccountid"] = new EntityReference("account", accountId)
            };
            var child2 = new Entity("new_childentity")
            {
                Id = Guid.NewGuid(),
                ["new_parentaccountid"] = new EntityReference("account", accountId)
            };
            var accountEntityReference = new EntityReference("account", accountId);

            fakeContext.Initialize(new[] { entity, child1, child2 });

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                accountEntityReference,
                "Create",
                fakeContext,
                stage);

            //Act
            fakeContext.ExecutePluginWith<AccountCreateNotePlugin>(pluginContext);
            var results = fakeContext.CreateQuery("new_childentity").ToList();

            //Assert
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Plugin_Should_Not_Delete_ChildEntity_On_Account_Delete_If_Stage_Not_20()
        {
            //Arrange
            var fakeContext = new XrmFakedContext();
            var stage = 10;
            var accountId = Guid.NewGuid();
            var entity = new Entity("account")
            {
                Id = accountId
            };
            var child1 = new Entity("new_childentity")
            {
                Id = Guid.NewGuid(),
                ["new_parentaccountid"] = new EntityReference("account", accountId)
            };
            var child2 = new Entity("new_childentity")
            {
                Id = Guid.NewGuid(),
                ["new_parentaccountid"] = new EntityReference("account", accountId)
            };
            var accountEntityReference = new EntityReference("account", accountId);

            fakeContext.Initialize(new[] { entity, child1, child2 });

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                accountEntityReference,
                "Create",
                fakeContext,
                stage);

            //Act
            fakeContext.ExecutePluginWith<AccountCreateNotePlugin>(pluginContext);
            var results = fakeContext.CreateQuery("new_childentity").ToList();

            //Assert
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Plugin_Should_Not_Delete_ChildEntity_On_Account_Delete_If_No_Target_Is_Available()
        {
            //Arrange
            var fakeContext = new XrmFakedContext();
            var stage = 20;
            var accountId = Guid.NewGuid();
            var entity = new Entity("account")
            {
                Id = accountId
            };
            var child1 = new Entity("new_childentity")
            {
                Id = Guid.NewGuid(),
                ["new_parentaccountid"] = new EntityReference("account", accountId)
            };
            var child2 = new Entity("new_childentity")
            {
                Id = Guid.NewGuid(),
                ["new_parentaccountid"] = new EntityReference("account", accountId)
            };
            var accountEntityReference = new EntityReference("account", accountId);

            fakeContext.Initialize(new[] { entity, child1, child2 });

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                accountEntityReference,
                "Create",
                fakeContext,
                stage);
            pluginContext.InputParameters = new ParameterCollection();

            //Act
            fakeContext.ExecutePluginWith<AccountCreateNotePlugin>(pluginContext);
            var results = fakeContext.CreateQuery("new_childentity").ToList();

            //Assert
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Plugin_Should_Not_Delete_ChildEntity_On_Account_Delete_If_Target_Entity_Not_Account()
        {
            //Arrange
            var fakeContext = new XrmFakedContext();
            var stage = 20;
            var accountId = Guid.NewGuid();
            var entity = new Entity("mail")
            {
                Id = accountId
            };
            var child1 = new Entity("new_childentity")
            {
                Id = Guid.NewGuid(),
                ["new_parentaccountid"] = new EntityReference("account", accountId)
            };
            var child2 = new Entity("new_childentity")
            {
                Id = Guid.NewGuid(),
                ["new_parentaccountid"] = new EntityReference("account", accountId)
            };
            var accountEntityReference = new EntityReference("account", accountId);

            fakeContext.Initialize(new[] { entity, child1, child2 });

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                accountEntityReference,
                "Create",
                fakeContext,
                stage);
            pluginContext.InputParameters = new ParameterCollection();

            //Act
            fakeContext.ExecutePluginWith<AccountCreateNotePlugin>(pluginContext);
            var results = fakeContext.CreateQuery("new_childentity").ToList();

            //Assert
            Assert.NotEmpty(results);
        }
    }
}
