using AutoMapper;
using CRM.API.Controllers;
using CRM.API.Models.InputModels;
using CRM.API.Models.OutputModels;
using CRM.Core;
using CRM.IntegrationTests.Mocks;
using CrmRepository.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NUnit.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.IntegrationTests
{
    [TestFixture]
    public class LeadControllerTest : TestHelper
    {
        private IMapper _mapper;
        private ILeadRepository _leadRepository;
        private IAccountRepository _accountRepository;
        private IOptions<UrlOptions> _urlOptions;
        private LeadController _systemUnderTest;
        private AccountInputModel _accountIMForTest;
        private AccountOutputModel _accountOMForTest;
        private List<AccountOutputModel> _accountsOMForTest = new List<AccountOutputModel>();
        private LeadInputModel _leadForTest;
        private List<LeadOutputModel> _leadsForCompare = new List<LeadOutputModel>();
        private List<LeadInputModel> _leadsToInsertForSearch = new List<LeadInputModel>();

        [OneTimeSetUp]
        public async ValueTask Setup()
        {
            _mapper = Resolve<IMapper>();
            _leadRepository = Resolve<ILeadRepository>();
            _accountRepository = Resolve<IAccountRepository>();
            _systemUnderTest = new LeadController(_leadRepository, _accountRepository, _mapper, _urlOptions);
            await DropOrRestoreTestDbs("Dbs_Restore");
            _leadForTest = (LeadInputModel)LeadInputModelMock.leadInputModel.Clone();
            _accountIMForTest = (AccountInputModel)AccountInputModelMock.accountInputModel.Clone();

            foreach (var item in LeadInputModelMock.leadsToInsertForSearchTest)
            {
                _leadsToInsertForSearch.Add((LeadInputModel)item.Clone());
            }

            foreach (var leadModel in _leadsToInsertForSearch)
            {
                ActionResult<LeadOutputModel> leadsInsertActionResult 
                    = await _systemUnderTest.AddLead(leadModel);
                LeadOutputModel outputLead = AssertAndConvert(leadsInsertActionResult);
                leadModel.Id = outputLead.Id;
                _leadsForCompare.Add(outputLead);
                _accountIMForTest.LeadId = (long)outputLead.Id;
                ActionResult<AccountOutputModel> accountForSearchActionResult = await _systemUnderTest.AddAccount(_accountIMForTest);
                var accountForSearch = AssertAndConvert(accountForSearchActionResult);
                _accountsOMForTest.Add(accountForSearch);
            }
        }

        [OneTimeTearDown]
        public async ValueTask TearDown()
        {
            _systemUnderTest = null;
            await DropOrRestoreTestDbs("Dbs_Drop");
            ShutdownIoC();
        }

        [Test]
        public async ValueTask TestEntity()
        {
            await TestAddLead();
            await TestAddAccount();
            await TestSelectLead();
            await TestSelectAccounts();
            await TestUpdateLead();
            await TestDeleteAccount();
            await TestDeleteLead();
            await TestSearchLead();
        }

        private async ValueTask TestAddLead()
        {
            ActionResult<LeadOutputModel> actionResult = await _systemUnderTest.AddLead(_leadForTest);
            LeadOutputModel model = AssertAndConvert(actionResult);
            _leadForTest.Id = model.Id;
            _accountIMForTest.LeadId = (long)model.Id;
            DeepEqualForLeadModels(_leadForTest, model);
        }

        private async ValueTask TestAddAccount()
        {
            ActionResult<AccountOutputModel> actionResult = await _systemUnderTest.AddAccount(_accountIMForTest);
            AccountOutputModel model = AssertAndConvert(actionResult);
            _accountOMForTest = model;
            _accountIMForTest.Id = model.Id;
            DeepEqualForAccountModels(_accountIMForTest, model);
        }

        private async ValueTask TestSelectLead()
        {
            ActionResult<LeadOutputModel> returnedLeadActionResult = await _systemUnderTest.GetLeadById(Convert.ToInt64(_leadForTest.Id));
            LeadOutputModel returnedLead = AssertAndConvert(returnedLeadActionResult);
            DeepEqualForLeadModels(_leadForTest, returnedLead);
            ActionResult<List<LeadOutputModel>> returnedLeadsActionResult = await _systemUnderTest.GetAllLeads();
            List<LeadOutputModel> listActual = AssertAndConvert(returnedLeadsActionResult);
            CollectionAssert.IsNotEmpty(listActual);
            CollectionAssert.Contains(listActual, returnedLead);
        }

        private async ValueTask TestSelectAccounts()
        {
            ActionResult<AccountOutputModel> getAccountActionResult = await _systemUnderTest.GetAccountById(Convert.ToInt64(_accountIMForTest.Id));
            AccountOutputModel returnedAccount = AssertAndConvert(getAccountActionResult);
            DeepEqualForAccountModels(_accountIMForTest, returnedAccount);
            ActionResult<List<AccountOutputModel>> getAccountsActionResult = await _systemUnderTest.GetAllAccounts();
            List<AccountOutputModel> listActual = AssertAndConvert(getAccountsActionResult);
            CollectionAssert.IsNotEmpty(listActual);
            CollectionAssert.Contains(listActual, returnedAccount);
        }

        private async ValueTask TestUpdateLead()
        {
            ChangeLead(_leadForTest);
            ActionResult<LeadOutputModel> actionResult = await _systemUnderTest.UpdateLead(_leadForTest);
            LeadOutputModel model = AssertAndConvert(actionResult);
            DeepEqualForLeadModels(_leadForTest, model);
            Assert.NotNull(model.LastUpdateDate);
        }

        private async ValueTask TestDeleteLead()
        {
            ActionResult<LeadOutputModel> getLeadActionResult = await _systemUnderTest.GetLeadById(Convert.ToInt64(_leadForTest.Id));
            Assert.IsNotNull(getLeadActionResult);
            var leadByGetById = AssertAndConvert(getLeadActionResult);
            ActionResult<LeadOutputModel> deleteLeadActionResult = await _systemUnderTest.LeadSoftDelete(Convert.ToInt64(_leadForTest.Id));
            Assert.NotNull(deleteLeadActionResult);
            Assert.IsNull(deleteLeadActionResult.Result as OkObjectResult);
            ActionResult<List<LeadOutputModel>> deletedLeadsActionResult = await _systemUnderTest.SearchLead(new LeadSearchInputModel { IsDeletedInclude = true });
            var deletedLeadsModels = AssertAndConvert(deletedLeadsActionResult);
            deletedLeadsModels.Contains(leadByGetById);
        }

        private async ValueTask TestDeleteAccount()
        {
            ActionResult<AccountOutputModel> getAccountActionResult = 
                await _systemUnderTest.GetAccountById(Convert.ToInt64(_accountIMForTest.Id));
            Assert.IsNotNull(getAccountActionResult);
            var returnedAccount = AssertAndConvert(getAccountActionResult);

            ActionResult<AccountOutputModel> softDeleteAccountActionResult = 
                await _systemUnderTest.AccountSoftDelete((long)_accountIMForTest.Id);
            Assert.NotNull(softDeleteAccountActionResult);
            Assert.IsNull(softDeleteAccountActionResult.Result as OkObjectResult);
            
            ActionResult<List<LeadOutputModel>> searchLeadActionResult = 
                await _systemUnderTest.SearchLead(new LeadSearchInputModel { AccountId = (long)_accountIMForTest.Id });
            var leadsByAccountId = AssertAndConvert(searchLeadActionResult);
            foreach (var lead in leadsByAccountId)
            {
                lead.Accounts.Contains(_accountOMForTest);
            };
        }

        private async ValueTask TestSearchLead()
        {
            await TestSearchNothing();
            await TestSearchById();
            await TestSearchByIdOperator();
            await TestSearchByFirstName();
            await TestSearchByFirstNameOperator();
            await TestSearchByLastName();
            await TestSearchByLastNameOperator();
            await TestSearchByPatronymic();
            await TestSearchByPatronymicOperator();
            await TestSearchByBirthDate();
            await TestSearchByBirthDateOperator();
            await TestSearchByBirthDateBetween();
            await TestSearchByPhone();
            await TestSearchByPhoneOperator();
            await TestSearchByEmail();
            await TestSearchByEmailOperator();
            await TestSearchByLogin();
            await TestSearchByLoginOperator();
            await TestSearchByCityId();
            await TestSearchByCityValues();
            await TestSearchByRegistrationDate();
            await TestSearchByRegistrationDateEnd();
            await TestSearchByAccountId();
            await TestSearchByIsDeletedInclude();
            await TestSearchByCombination();
        }

        #region Search
        private async ValueTask TestSearchNothing()
        {
            ActionResult<List<LeadOutputModel>> actionResult =
                await _systemUnderTest.SearchLead(LeadSearchInputModelMock.searchNothing);
            List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
            Assert.IsTrue(outputLeads.Count is 0);
        }

        private async ValueTask TestSearchById()
        {
            foreach (var item in _leadsToInsertForSearch)
            {
                ActionResult<List<LeadOutputModel>> actionResult =
                    await _systemUnderTest.SearchLead(new LeadSearchInputModel { Id = item.Id });
                List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
                DeepEqualForLeadModels(item, outputLeads[0]);
            }
        }

        private async ValueTask TestSearchByIdOperator()
        {

            ActionResult<List<LeadOutputModel>> actionResultMore =
                await _systemUnderTest.SearchLead(new LeadSearchInputModel { IdOperator = 2, IdEnd = (int?)_leadsToInsertForSearch[_leadsToInsertForSearch.Count - 1].Id });
            List<LeadOutputModel> outputLeadsMore = AssertAndConvert(actionResultMore);
            for (int i = 1; i < outputLeadsMore.Count; i++)
            {
                _leadsForCompare.Contains(outputLeadsMore[i]);
            }


            ActionResult<List<LeadOutputModel>> actionResultLess =
                await _systemUnderTest.SearchLead(new LeadSearchInputModel { IdOperator = 1, IdEnd = (int?)_leadsToInsertForSearch[0].Id });
            List<LeadOutputModel> outputLeadsLess = AssertAndConvert(actionResultLess);
            for (int i = 1; i < outputLeadsLess.Count; i++)
            {
                _leadsForCompare.Contains(outputLeadsLess[i]);
            }
        }

        private async ValueTask TestSearchByFirstName()
        {
            foreach (var item in _leadsToInsertForSearch)
            {
                ActionResult<List<LeadOutputModel>> actionResult =
                await _systemUnderTest.SearchLead(new LeadSearchInputModel { FirstName = item.FirstName });
                List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
                DeepEqualForLeadModels(item, outputLeads[0]);
            }
        }

        private async ValueTask TestSearchByFirstNameOperator()
        {
            ActionResult<List<LeadOutputModel>> actionResult =
            await _systemUnderTest.SearchLead(LeadSearchInputModelMock.searchByFirstNameOperation);
            List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
            for (int i = 0; i < outputLeads.Count; i++)
            {
                _leadsForCompare.Contains(outputLeads[i]);
            }
        }

        private async ValueTask TestSearchByLastName()
        {
            foreach (var item in _leadsToInsertForSearch)
            {
                ActionResult<List<LeadOutputModel>> actionResult =
                await _systemUnderTest.SearchLead(new LeadSearchInputModel { LastName = item.LastName });
                List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
                DeepEqualForLeadModels(item, outputLeads[0]);
            }
        }

        private async ValueTask TestSearchByLastNameOperator()
        {
            ActionResult<List<LeadOutputModel>> actionResult =
            await _systemUnderTest.SearchLead(LeadSearchInputModelMock.searchByLastNameOperation);
            List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
            for (int i = 0; i < outputLeads.Count; i++)
            {
                _leadsForCompare.Contains(outputLeads[i]);
            }
        }

        private async ValueTask TestSearchByPatronymic()
        {
            foreach (var item in _leadsToInsertForSearch)
            {
                ActionResult<List<LeadOutputModel>> actionResult =
                await _systemUnderTest.SearchLead(new LeadSearchInputModel { Patronymic = item.Patronymic });
                List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
                DeepEqualForLeadModels(item, outputLeads[0]);
            }
        }

        private async ValueTask TestSearchByPatronymicOperator()
        {
            ActionResult<List<LeadOutputModel>> actionResult =
            await _systemUnderTest.SearchLead(LeadSearchInputModelMock.searchByPatronymicOperation);
            List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
            for (int i = 1; i < outputLeads.Count; i++)
            {
                _leadsForCompare.Contains(outputLeads[i]);
            }
        }

        private async ValueTask TestSearchByBirthDate()
        {
            foreach (var item in _leadsToInsertForSearch)
            {
                ActionResult<List<LeadOutputModel>> actionResult =
                await _systemUnderTest.SearchLead(new LeadSearchInputModel { BirthDate = item.BirthDate });
                List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
                foreach (var lead in _leadsForCompare)
                {
                    outputLeads.Contains(lead);
                }
            }
        }

        private async ValueTask TestSearchByBirthDateOperator()
        {
            ActionResult<List<LeadOutputModel>> actionResultMore =
            await _systemUnderTest.SearchLead(LeadSearchInputModelMock.searchByBirthDateMore);
            List<LeadOutputModel> outputLeadsMore = AssertAndConvert(actionResultMore);
            for (int i = 1; i < outputLeadsMore.Count; i++)
            {
                _leadsForCompare.Contains(outputLeadsMore[i]);
            }

            ActionResult<List<LeadOutputModel>> actionResultLess =
            await _systemUnderTest.SearchLead(LeadSearchInputModelMock.searchByBirthDateLess);
            List<LeadOutputModel> outputLeadsLess = AssertAndConvert(actionResultLess);
            for (int i = 1; i < outputLeadsLess.Count; i++)
            {
                _leadsForCompare.Contains(outputLeadsLess[i]);
            }
        }

        private async ValueTask TestSearchByBirthDateBetween()
        {
            ActionResult<List<LeadOutputModel>> actionResultBetween =
            await _systemUnderTest.SearchLead(LeadSearchInputModelMock.searchByBirthDateBetween);
            List<LeadOutputModel> outputLeadsBetween = AssertAndConvert(actionResultBetween);
            for (int i = 1; i < outputLeadsBetween.Count; i++)
            {
                _leadsForCompare.Contains(outputLeadsBetween[i]);
            }
        }

        private async ValueTask TestSearchByPhone()
        {
            foreach (var item in _leadsToInsertForSearch)
            {
                ActionResult<List<LeadOutputModel>> actionResult =
                await _systemUnderTest.SearchLead(new LeadSearchInputModel { Phone = item.Phone });
                List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
                DeepEqualForLeadModels(item, outputLeads[0]);
            }
        }

        private async ValueTask TestSearchByPhoneOperator()
        {
            ActionResult<List<LeadOutputModel>> actionResult =
            await _systemUnderTest.SearchLead(LeadSearchInputModelMock.searchByPhoneOperation);
            List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
            for (int i = 0; i < outputLeads.Count; i++)
            {
                _leadsForCompare.Contains(outputLeads[i]);
            }
        }

        private async ValueTask TestSearchByEmail()
        {
            foreach (var item in _leadsToInsertForSearch)
            {
                ActionResult<List<LeadOutputModel>> actionResult =
                await _systemUnderTest.SearchLead(new LeadSearchInputModel { Email = item.Email });
                List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
                DeepEqualForLeadModels(item, outputLeads[0]);
            }
        }

        private async ValueTask TestSearchByEmailOperator()
        {
            ActionResult<List<LeadOutputModel>> actionResult =
            await _systemUnderTest.SearchLead(LeadSearchInputModelMock.searchByEmailOperation);
            List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
            for (int i = 0; i < outputLeads.Count; i++)
            {
                _leadsForCompare.Contains(outputLeads[i]);
            }
        }

        private async ValueTask TestSearchByLogin()
        {
            foreach (var item in _leadsToInsertForSearch)
            {
                ActionResult<List<LeadOutputModel>> actionResult =
                await _systemUnderTest.SearchLead(new LeadSearchInputModel { Login = item.Login });
                List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
                DeepEqualForLeadModels(item, outputLeads[0]);
            }
        }

        private async ValueTask TestSearchByLoginOperator()
        {
            ActionResult<List<LeadOutputModel>> actionResult =
            await _systemUnderTest.SearchLead(LeadSearchInputModelMock.searchByLoginOperation);
            List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
            for (int i = 0; i < outputLeads.Count; i++)
            {
                _leadsForCompare.Contains(outputLeads[i]);
            }
        }

        private async ValueTask TestSearchByRegistrationDate()
        {
            foreach (var item in _leadsForCompare)
            {
                ActionResult<List<LeadOutputModel>> actionResult =
                await _systemUnderTest.SearchLead(new LeadSearchInputModel { RegistrationDate = item.RegistrationDate, RegistrationDateOperator = 2 });
                List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
                outputLeads.Contains(item);
            }
        }

        private async ValueTask TestSearchByRegistrationDateEnd()
        {
            foreach (var item in _leadsForCompare)
            {
                ActionResult<List<LeadOutputModel>> actionResult =
                await _systemUnderTest.SearchLead(LeadSearchInputModelMock.searchByRegistrationDateEnd);
                List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
                outputLeads.Contains(item);
            }
        }

        private async ValueTask TestSearchByCityId()
        {
            foreach (var item in _leadsToInsertForSearch)
            {
                ActionResult<List<LeadOutputModel>> actionResult =
                await _systemUnderTest.SearchLead(new LeadSearchInputModel { CityId = item.CityId });
                List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
                foreach (var lead in _leadsForCompare)
                {
                    outputLeads.Contains(lead);
                }
            }
        }

        private async ValueTask TestSearchByCityValues()
        {
            foreach (var item in _leadsToInsertForSearch)
            {
                ActionResult<List<LeadOutputModel>> actionResult =
                await _systemUnderTest.SearchLead(LeadSearchInputModelMock.searchByCityValues);
                List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
                foreach (var lead in _leadsForCompare)
                {
                    outputLeads.Contains(lead);
                }
            }
        }

        private async ValueTask TestSearchByAccountId()
        {
            foreach (var account in _accountsOMForTest)
            {
                ActionResult<List<LeadOutputModel>> returnedLeadsActionResult =
                    await _systemUnderTest.SearchLead(new LeadSearchInputModel { AccountId = account.Id });
                List<LeadOutputModel> outputLeads = AssertAndConvert(returnedLeadsActionResult);
                foreach (var lead in _leadsForCompare)
                {
                    outputLeads.Contains(lead);
                }
            }
        }

        private async ValueTask TestSearchByIsDeletedInclude()
        {
            foreach (var item in _leadsToInsertForSearch)
            {
                ActionResult<LeadOutputModel> deletedLeadActionResult =
                    await _systemUnderTest.LeadSoftDelete((long)item.Id);
                ActionResult <List<LeadOutputModel>> actionResult =
                await _systemUnderTest.SearchLead(LeadSearchInputModelMock.searchByIsDeletedInclude);
                List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
                foreach (var lead in _leadsForCompare)
                {
                    outputLeads.Contains(lead);
                }
            }
        }

        private async ValueTask TestSearchByCombination()
        {
            for (int i = 0; i < _accountsOMForTest.Count; i++)
            {
                var leadMockWithAccount = LeadSearchInputModelMock.searchByCombination;
                leadMockWithAccount.AccountId = _accountsOMForTest[i].Id;
                ActionResult<List<LeadOutputModel>> actionResult =
                await _systemUnderTest.SearchLead(leadMockWithAccount);
                List<LeadOutputModel> outputLeads = AssertAndConvert(actionResult);
                foreach (var lead in _leadsForCompare)
                {
                    outputLeads.Contains(lead);
                }
            }
        }
        #endregion
    }
}