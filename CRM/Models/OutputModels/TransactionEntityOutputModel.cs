using System;

namespace CRM.API.Models.OutputModels
{
    public class TransactionEntityOutputModel
    {
        public long Id { get; set; }
        public decimal Amount { get; set; }
        public string TimeStamp { get; set; }
        public long AccountId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is TransactionEntityOutputModel transactionEntityOutputModel &&
                   Id == transactionEntityOutputModel.Id &&
                   Amount == transactionEntityOutputModel.Amount &&
                   TimeStamp == transactionEntityOutputModel.TimeStamp &&
                   AccountId == transactionEntityOutputModel.AccountId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id,
                Amount, TimeStamp, AccountId);
        }
    }
}
