using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.API.Models.InputModels
{
    public class TransferInputModel : ICloneable
    {
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public AccountShortInputModel SenderAccount { get; set; }
        [Required]
        public AccountShortInputModel RecipientAccount { get; set; }
        public string SenderTimeStamp { get; set; }
        public string RecipientTimeStamp { get; set; }

        public virtual object Clone()
        {
            return new TransferInputModel()
            {
                Amount = this.Amount,
                SenderAccount = this.SenderAccount,
                RecipientAccount = this.RecipientAccount
            };
        }
    }
}
