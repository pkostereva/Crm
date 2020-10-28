using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.API.Models.InputModels
{
	public class AccountInputModel : ICloneable
	{
		public long? Id { get; set; }
		[Required]
		public long LeadId { get; set; }
		[Required]
		public int CurrencyId { get; set; }

		public object Clone()
		{
			return new AccountInputModel()
			{
				LeadId = LeadId,
				CurrencyId = CurrencyId
			};
		}

		public override bool Equals(object obj)
		{
			return obj is AccountInputModel accountInputModel &&
				Id == accountInputModel.Id &&
				LeadId == accountInputModel.LeadId &&
				CurrencyId == accountInputModel.CurrencyId;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Id, LeadId, CurrencyId);
		}
	}
}
