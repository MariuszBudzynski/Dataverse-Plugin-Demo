using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using PluginSample.FakeItTests.Helopers;
using PluginSample.Plugins;
using System;
using System.Security.Principal;
using Xunit;

namespace PluginSample.FakeItTests
{
    public class AccountUpdatePrimaryContactPluginTests
    {
        // FakeXrmEasy v1 is intentionally used here.
        // Newer versions require .NET Core and are not compatible with D365 plugins.

        // this test demonstrate second option to retrive data, instead of .CreateQuery() we
        // are using GetOrganizationService() for Service operations

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
            Entity account, contact;
            PrepeareTestData(out account, out contact);
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
        public void Plugin_Should_Should_Not_Update_Contact_When_Account_Updated_If_Message_Not_Update()
        {
            ////Arrange
            Entity account, contact;
            PrepeareTestData(out account, out contact);
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
        public void Plugin_Should_Should_Not_Update_Contact_When_Account_Updated_If_No_Target_Is_Available()
        {
            ////Arrange
            Entity account, contact;
            PrepeareTestData(out account, out contact);
            account["primarycontactid"] = new EntityReference("contact", contact.Id);

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                account,
                "Create",
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
        public void Plugin_Should_Should_Not_Update_Contact_When_Account_Updated_If_Entity_Not_account()
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
            mail["primarycontactid"] = new EntityReference("contact", contact.Id);

            var pluginContext = CreateFakeContext.CreateFakePluginContext(
                mail,
                "Create",
                _sut);
            pluginContext.InputParameters = new ParameterCollection();

            _sut.Initialize(new[] { mail, contact });

            ////Act
            _sut.ExecutePluginWith<AccountUpdatePrimaryContactPlugin>(pluginContext);

            var updatedContact = _organizationService
            .Retrieve("contact", contact.Id, new ColumnSet("description"));

            //Assert
            Assert.Equal("Test description", updatedContact["description"]);
        }

        private static void PrepeareTestData(out Entity account, out Entity contact)
        {
            account = new Entity("account")
            {
                Id = Guid.NewGuid(),
            };
            contact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                ["description"] = "Test description"
            };
        }
    }
}
