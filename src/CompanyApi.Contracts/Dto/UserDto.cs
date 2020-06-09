using System;

namespace CompanyApi.Contracts.Dto
{
	[Serializable]
	public class UserDto
	{
		public string Username { get; set; }
	
		public string Password { get; set; }
	}
}
