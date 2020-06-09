using System;
using System.Collections.Generic;

namespace CompanyApi.Contracts.Dto
{
	[Serializable]
	public class CompanyDto
	{
		public int CompanyId { get; set; }

		public string Name { get; set; }

		public ICollection<string> Employees { get; set; } = new HashSet<string>();
	}
}