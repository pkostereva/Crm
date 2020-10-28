using CRM.API.Models.InputModels;

namespace CRM.IntegrationTests
{
    public class TransferInputModelMock
    {
        private static AccountShortInputModel senderAccount = new AccountShortInputModel()
        {
            Id = 3,
            CurrencyId =3
        };

        private static AccountShortInputModel recipientAccount = new AccountShortInputModel()
        {
            Id = 2,
            CurrencyId = 4
        };
        
        public static TransferInputModel transactionInputModel = new TransferInputModel()
        {
            Amount = 2000,
            SenderAccount = senderAccount,
            RecipientAccount = recipientAccount
        };
    }
}
