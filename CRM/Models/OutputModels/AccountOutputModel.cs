using System;

namespace CRM.API.Models.OutputModels
{
	public class AccountOutputModel
	{
		public long Id { get; set; }
		public long LeadId { get; set; }
		public int CurrencyId { get; set; }
		public string CurrencyCode { get; set; }

        public bool EqualsForAccountTest(object obj)
        {
            return obj is AccountOutputModel accountOutputModel &&
                   LeadId == accountOutputModel.LeadId &&
                   CurrencyId == accountOutputModel.CurrencyId;
        }

        public override bool Equals(object obj)
        {
            return obj is AccountOutputModel model &&
                Id == model.Id &&
                LeadId == model.LeadId &&
                CurrencyId == model.CurrencyId &&
                CurrencyCode == model.CurrencyCode;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, LeadId, CurrencyId);
        }
    }
}
