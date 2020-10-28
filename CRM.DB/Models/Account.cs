using System;

namespace CRM.DB.Models
{
	public class Account
	{
		public long? Id { get; set; }
		public long LeadId { get; set; }
		public Currency Currency { get; set; }
		public DateTime Timestamp { get; set; }
		public bool IsDeleted { get; set; }
	}
}
