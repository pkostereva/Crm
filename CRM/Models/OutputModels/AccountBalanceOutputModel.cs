using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.API.Models.OutputModels
{
    public class AccountBalanceOutputModel
    {
        public long AccountId { get; set; }
        public decimal Balance { get; set; }
        public string TimeStamp { get; set; }
    }
}
