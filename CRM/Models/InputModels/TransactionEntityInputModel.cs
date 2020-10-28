using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.API.Models.InputModels
{
    public class TransactionEntityInputModel : ICloneable
    {
        public decimal Amount { get; set; }
        [Required]
        public AccountShortInputModel Account { get; set; }

        public string TimeStamp { get; set; }

        public virtual object Clone()
        {
            return new TransactionEntityInputModel()
            {
                Amount = this.Amount,
                Account = this.Account
            };
        }
    }
}
