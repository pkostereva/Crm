using CRM.API.Models.InputModels;

namespace CRM.IntegrationTests
{
    public class TransactionInputModelMock
    {
        private static AccountShortInputModel account = new AccountShortInputModel()
        {
            Id = 3
        };

        public static TransactionEntityInputModel transactionInputModel = new TransactionEntityInputModel()
        {
            Amount = 2000,
            Account = account
        };
    }
}
