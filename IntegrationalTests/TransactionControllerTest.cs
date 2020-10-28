using CRM.API.Controllers;
using CRM.API.Models.InputModels;
using CRM.API.Models.OutputModels;
using CRM.Core;
using CRM.Core.ConfigurationOptions;
using CrmRepository.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.IntegrationTests
{
    public class TransactionControllerTest : TestHelper
    {
        private IOptions<UrlOptions> _urlOptions;
        private IOptions<PayPalOptions> _paypalOptions;
        private IAccountRepository _accountRepository;
        private TransactionController _systemUnderTest;
        private TransactionEntityInputModel _transactionToInsert;
        private TransferInputModel _transferToInsert;

        [OneTimeSetUp]
        public async ValueTask Setup()
        {
            _urlOptions = Resolve<IOptions<UrlOptions>>();
            _paypalOptions = Resolve<IOptions<PayPalOptions>>();
            _accountRepository = Resolve<IAccountRepository>();
            _systemUnderTest = new TransactionController(_urlOptions, _paypalOptions, _accountRepository);
            await DropOrRestoreTestDbs("Dbs_Restore");
            _transactionToInsert = (TransactionEntityInputModel)TransactionInputModelMock.transactionInputModel.Clone();
            _transferToInsert = (TransferInputModel)TransferInputModelMock.transactionInputModel.Clone();
        }

        [OneTimeTearDown]
        public async ValueTask TearDown()
        {
            _systemUnderTest = null;
            await DropOrRestoreTestDbs("Dbs_Drop");
            ShutdownIoC();
        }

        [Test]
        public async ValueTask ShouldGetAllTransactions()
        {
            ActionResult<List<TransactionEntityOutputModel>> actionResult = await _systemUnderTest.GetAllTransaction();
            var list = AssertAndConvertSpecificResult(actionResult);
            Assert.IsNotEmpty(list);
        }

        [Test]
        public async ValueTask ShouldWithdraw()
        {
            ActionResult<TransactionEntityOutputModel> actionResultWithdraw = await _systemUnderTest.WithdrawTransactionInsert(_transactionToInsert);
            var transactionWithdraw = AssertAndConvertSpecificResult(actionResultWithdraw);
            var actionResultGetById = await _systemUnderTest.TransactionGetById(transactionWithdraw.Id);
            var transactionGetById = AssertAndConvertSpecificResult(actionResultGetById);
            Assert.AreEqual(transactionWithdraw, transactionGetById);
        }

        [Test]
        public async ValueTask ShouldDeposit()
        {
            ActionResult<TransactionEntityOutputModel> actionResultDeposit = await _systemUnderTest.DepositTransactionInsert(_transactionToInsert);
            var transactionDeposit = AssertAndConvertSpecificResult(actionResultDeposit);
            var actionResultGetById = await _systemUnderTest.TransactionGetById(transactionDeposit.Id);
            var transactionGetById = AssertAndConvertSpecificResult(actionResultGetById);
            Assert.AreEqual(transactionDeposit, transactionGetById);
        }

        [Test]
        public async ValueTask ShouldTransfer()
        {
            ActionResult <List<TransactionEntityOutputModel>> actionResultTransfer = await _systemUnderTest.TransferTransactionInsert(_transferToInsert);
            var transferTransactions = AssertAndConvertSpecificResult(actionResultTransfer);
            var actionResultFirstPartOfTransfer = await _systemUnderTest.TransactionGetById(transferTransactions[0].Id);
            var firstTransaction = AssertAndConvertSpecificResult(actionResultFirstPartOfTransfer);
            var actionResultSecontPartOfTransfer = await _systemUnderTest.TransactionGetById(transferTransactions[1].Id);
            var secondTransaction = AssertAndConvertSpecificResult(actionResultSecontPartOfTransfer);
            Assert.That(transferTransactions.Contains(firstTransaction));
            Assert.That(transferTransactions.Contains(secondTransaction));
        }
    }
}
