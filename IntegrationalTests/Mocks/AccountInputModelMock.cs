using CRM.API.Models.InputModels;

namespace CRM.IntegrationTests.Mocks
{
    public class AccountInputModelMock
    {
        public static AccountInputModel accountInputModel = new AccountInputModel()
        {
            LeadId = 1234567,
            CurrencyId = 3
        };
    }
}
