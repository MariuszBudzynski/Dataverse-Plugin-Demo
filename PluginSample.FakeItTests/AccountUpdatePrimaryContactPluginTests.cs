using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using PluginSample.FakeItTests.Helopers;
using PluginSample.Plugins;
using System;
using Xunit;

namespace PluginSample.FakeItTests
{
    public class AccountUpdatePrimaryContactPluginTests
    {
        // NOTE:
        // FakeXrmEasy v1 is intentionally used.
        // Newer versions require .NET Core and are not compatible with Dynamics 365 plugins.
        //
        // These tests demonstrate an alternative approach to data verification:
        // instead of using CreateQuery(), they rely on IOrganizationService operations
        // (Retrieve / Update) to reflect real CRM behavior.

        private readonly XrmFakedContext _sut;
        private readonly IOrganizationService _organizationService;

        public AccountUpdatePrimaryContactPluginTests()
        {
            _sut = new XrmFakedContext();
            _organizationService = _sut.GetOrganizationService();
        }

        [Fact]
        public void Plugin_Should_Update_Contact_When_Account_Updated()
        {
            ////Arrange
            var InitialDescription = "Test description";
            Entity account, contact;
            PrepareTestData(out account, out contact, InitialDescription);
            account["primarycontactid"] = new EntityReference("contact", contact.Id);

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                account,
                "Update",
                _sut);

            _sut.Initialize(new[] { account, contact });

            ////Act
            _sut.ExecutePluginWith<AccountUpdatePrimaryContactPlugin>(pluginContext);

            var updatedContact = _organizationService
            .Retrieve("contact", contact.Id, new ColumnSet("description"));

            //Assert
            Assert.Equal("Updated because parent account was updated", updatedContact["description"]);
        }

        [Fact]
        public void Plugin_Should_Not_Update_Contact_When_Message_Is_Not_Update()
        {
            ////Arrange
            var InitialDescription = "Test description";
            Entity account, contact;
            PrepareTestData(out account, out contact, InitialDescription);
            account["primarycontactid"] = new EntityReference("contact", contact.Id);

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                account,
                "Create",
                _sut);

            _sut.Initialize(new[] { account, contact });

            ////Act
            _sut.ExecutePluginWith<AccountUpdatePrimaryContactPlugin>(pluginContext);

            var updatedContact = _organizationService
            .Retrieve("contact", contact.Id, new ColumnSet("description"));

            //Assert
            Assert.Equal("Test description", updatedContact["description"]);
        }

        [Fact]
        public void Plugin_Should_Not_Update_Contact_When_No_Target_Is_Available()
        {
            ////Arrange
            var InitialDescription = "Test description";
            Entity account, contact;
            PrepareTestData(out account, out contact, InitialDescription);
            account["primarycontactid"] = new EntityReference("contact", contact.Id);

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                account,
                "Update",
                _sut);
            pluginContext.InputParameters = new ParameterCollection();

            _sut.Initialize(new[] { account, contact });

            ////Act
            _sut.ExecutePluginWith<AccountUpdatePrimaryContactPlugin>(pluginContext);

            var updatedContact = _organizationService
            .Retrieve("contact", contact.Id, new ColumnSet("description"));

            //Assert
            Assert.Equal("Test description", updatedContact["description"]);
        }

        [Fact]
        public void Plugin_Should_Not_Update_Contact_When_Account_Has_No_PrimaryContact()
        {
            ////Arrange
            var InitialDescription = "Test description";
            Entity account, contact;
            PrepareTestData(out account, out contact, InitialDescription);

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                account,
                "Update",
                _sut);

            _sut.Initialize(new[] { account, contact });

            ////Act
            _sut.ExecutePluginWith<AccountUpdatePrimaryContactPlugin>(pluginContext);

            var updatedContact = _organizationService
            .Retrieve("contact", contact.Id, new ColumnSet("description"));

            //Assert
            Assert.Equal("Test description", updatedContact["description"]);
        }

        [Fact]
        public void Plugin_Should_Not_Update_Contact_When_Entity_Is_Not_Account()
        {
            ////Arrange
            var mail = new Entity("mail")
            {
                Id = Guid.NewGuid(),
            };
            var contact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["description"] = "Test description"
            };

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                mail,
                "Update",
                _sut);

            _sut.Initialize(new[] { mail, contact });

            ////Act
            _sut.ExecutePluginWith<AccountUpdatePrimaryContactPlugin>(pluginContext);

            var updatedContact = _organizationService
            .Retrieve("contact", contact.Id, new ColumnSet("description"));

            //Assert
            Assert.Equal("Test description", updatedContact["description"]);
        }

        private static void PrepareTestData(out Entity account, out Entity contact, string description)
        {
            account = new Entity("account")
            {
                Id = Guid.NewGuid(),
            };
            contact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["description"] = description
            };
        }
    }
}
