using CRM.API;
using CRM.API.Models.InputModels;
using CRM.API.Models.OutputModels;
using CRM.Core;
using CRM.DB.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CRM.IntegrationTests
{
    public class TestHelper : IoCSupport<AutofacModule>
    {
        protected async ValueTask DropOrRestoreTestDbs(string name)
        {
            string connectionString = Resolve<IOptions<StorageOptions>>().Value.DBMasterConnectionString;
            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                conn.ChangeDatabase("master");
            await conn.QueryAsync(
                    name,
                    null,
                    transaction: null,
                    commandTimeout:3600,
                    commandType: CommandType.StoredProcedure); 
            }
        }

        protected void DeepEqualForLeadModels(LeadInputModel inputModel, LeadOutputModel outputModel)
        {
            Assert.NotNull(outputModel.CityName);
            Assert.NotNull(outputModel.RegistrationDate);
            Assert.NotNull(outputModel.Accounts);
            var model = mapper.Map<Lead>(inputModel);
            var model1 = mapper.Map<LeadOutputModel>(model);
            Assert.IsTrue(model1.EqualsForLeadTest(outputModel));
        }

        protected void DeepEqualForAccountModels(AccountInputModel inputModel, AccountOutputModel outputModel)
        {
            Assert.NotNull(outputModel.CurrencyCode);
            var model = mapper.Map<Account>(inputModel);
            var model1 = mapper.Map<AccountOutputModel>(model);
            Assert.IsTrue(model1.EqualsForAccountTest(outputModel));
        }

        protected T AssertAndConvert<T>(ActionResult<T> actionResult)
        {
            Assert.NotNull(actionResult);
            OkObjectResult result = actionResult.Result as OkObjectResult;
            Assert.NotNull(result);
            return (T)result.Value;
        }

        protected T AssertAndConvertSpecificResult<T>(ActionResult<T> actionResult)
        {
            Assert.NotNull(actionResult);
            Assert.NotNull(actionResult.Value);
            return (T)actionResult.Value;
        }

        protected void AssertAndConvertNotFoundActionResult<T>(ActionResult<T> actionResult)
        {
            Assert.NotNull(actionResult);
            Assert.NotNull(actionResult.Result);
            var result = actionResult.Result as NotFoundObjectResult;
            Assert.AreEqual(result.StatusCode, 404);
        }

        protected void ChangeLead(LeadInputModel model)
        {
            model.Email = $"reginaUberAlles@gmail.com{LeadInputModelMock.RandomNumber(1, 1000000000)}";
            model.Phone = $"+79995196034{LeadInputModelMock.RandomNumber(1, 1000000000)}";
            model.Login = $"regina{LeadInputModelMock.RandomNumber(1, 1000000000)}";
        }
    }
}
