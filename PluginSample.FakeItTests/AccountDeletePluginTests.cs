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
        private readonly XrmFakedContext _sut;

        public AccountDeletePluginTests()
        {
            _sut = new XrmFakedContext();
        }

        [Fact]
        public void Plugin_Should_Delete_ChildEntity_On_Account_Delete()
        {
            // Arrange
            var stage = 20;
            var accountId = Guid.NewGuid();
            var accountEntityReference = PrepareTestData(_sut, accountId, "account");
            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                accountEntityReference,
                "Delete",
                _sut,
                stage);

            // Act
            _sut.ExecutePluginWith<AccountDeletePlugin>(pluginContext);
            var results = _sut.CreateQuery("new_childentity").ToList();

            // Assert
            Assert.Empty(results);
        }


        [Fact]
        public void Plugin_Should_Not_Delete_ChildEntity_On_Account_Delete_If_Message_Not_Delete()
        {
            //Arrange
            var stage = 20;
            var accountId = Guid.NewGuid();
            var accountEntityReference = PrepareTestData(_sut, accountId, "account");
            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                accountEntityReference,
                "Create",
                _sut,
                stage);

            //Act
            _sut.ExecutePluginWith<AccountDeletePlugin>(pluginContext);
            var results = _sut.CreateQuery("new_childentity").ToList();

            //Assert
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Plugin_Should_Not_Delete_ChildEntity_On_Account_Delete_If_Stage_Not_20()
        {
            //Arrange
            var stage = 10;
            var accountId = Guid.NewGuid();
            var accountEntityReference = PrepareTestData(_sut, accountId, "account");
            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                accountEntityReference,
                "Delete",
                _sut,
                stage);

            //Act
            _sut.ExecutePluginWith<AccountDeletePlugin>(pluginContext);
            var results = _sut.CreateQuery("new_childentity").ToList();

            //Assert
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Plugin_Should_Not_Delete_ChildEntity_On_Account_Delete_If_No_Target_Is_Available()
        {
            //Arrange
            var stage = 20;
            var accountId = Guid.NewGuid();
            var accountEntityReference = PrepareTestData(_sut, accountId, "account");
            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                accountEntityReference,
                "Delete",
                _sut,
                stage);
            pluginContext.InputParameters = new ParameterCollection();

            //Act
            _sut.ExecutePluginWith<AccountDeletePlugin>(pluginContext);
            var results = _sut.CreateQuery("new_childentity").ToList();

            //Assert
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Plugin_Should_Not_Delete_ChildEntity_On_Account_Delete_If_Target_Entity_Not_Account()
        {
            //Arrange
            var stage = 20;
            var accountId = Guid.NewGuid();
            PrepareTestData(_sut, accountId, "account");
            var wrongTarget = new EntityReference("mail", Guid.NewGuid());
            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                wrongTarget,
                "Delete",
                _sut,
                stage);

            //Act
            _sut.ExecutePluginWith<AccountDeletePlugin>(pluginContext);
            var results = _sut.CreateQuery("new_childentity").ToList();

            //Assert
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Plugin_Should_Not_Delete_When_Target_Is_Entity_Instead_Of_EntityReference()
        {
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

            _sut.Initialize(new[] { entity, child1, child2 });

            var pluginContext = _sut.GetDefaultPluginContext();
            pluginContext.MessageName = "Delete";
            pluginContext.Stage = stage;
            pluginContext.InputParameters = new ParameterCollection
            {
                {"Target", entity }
            };

            _sut.ExecutePluginWith<AccountDeletePlugin>(pluginContext);

            var results = _sut.CreateQuery("new_childentity").ToList();

            Assert.NotEmpty(results);
        }

        [Fact]
        public void Plugin_Should_Delete_Only_Children_Of_Deleted_Account()
        {
            // Arrange
            var stage = 20;
            var accountId = Guid.NewGuid();
            var otherAccountId = Guid.NewGuid();
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
            var foreignChild = new Entity("new_childentity")
            {
                Id = Guid.NewGuid(),
                ["new_parentaccountid"] = new EntityReference("account", otherAccountId)
            };

            _sut.Initialize(new[] { entity, child1, child2, foreignChild });

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                new EntityReference("account", accountId),
                "Delete",
                _sut,
                stage);

            // Act
            _sut.ExecutePluginWith<AccountDeletePlugin>(pluginContext);
            var results = _sut.CreateQuery("new_childentity").ToList();

            // Assert
            Assert.Single(results);
        }

        [Fact]
        public void Plugin_Should_Not_Throw_When_Account_Has_No_Children()
        {
            // Arrange
            var stage = 20;
            var accountId = Guid.NewGuid();
            var entity = new Entity("account") { Id = accountId };

            _sut.Initialize(new[]{entity});

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                new EntityReference("account", accountId),
                "Delete",
                _sut,
                stage);

            // Act
            var exception = Record.Exception(() =>
                _sut.ExecutePluginWith<AccountDeletePlugin>(pluginContext));

            // Assert
            Assert.Null(exception);
        }

        private static EntityReference PrepareTestData(XrmFakedContext sut, Guid accountId, string entityName)
        {
            var entity = new Entity(entityName)
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

            sut.Initialize(new[] { entity, child1, child2 });
            return accountEntityReference;
        }

    }
}