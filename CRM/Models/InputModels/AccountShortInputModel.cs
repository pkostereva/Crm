using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.API.Models.InputModels
{
	public class AccountShortInputModel
	{
		public long Id { get; set; }
		[Required]
		public int CurrencyId { get; set; }

		public object Clone()
		{
			return new AccountShortInputModel()
			{
				CurrencyId = CurrencyId,
			};
		}

		public override bool Equals(object obj)
		{
			return obj is AccountInputModel accountInputModel &&
				Id == accountInputModel.Id &&
				CurrencyId == accountInputModel.CurrencyId;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Id, CurrencyId);
		}
	}
}
