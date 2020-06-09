using System;

namespace CompanyApi.Contracts.Entities.Base
{
	public abstract class BaseAuditEntity : IBaseAuditEntity    
	{
		public DateTime Created { get; set; } = DateTime.UtcNow;
		
		public DateTime Modified { get; set; } = DateTime.UtcNow;
	}
}
