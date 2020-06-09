using CompanyApi.Contracts.Entities;
using CompanyApi.Core.Auth;
using CompanyApi.Services.Repositories;
using Microsoft.Extensions.Options;

namespace CompanyApi.Services.Authorization
{
	public class UserService : IUserService
	{
		private readonly AuthSettings _authSettings;
		private readonly IUserRepository _userRepository;
        private readonly IJwtFactory _jwtFactory;

        public UserService(IOptions<AuthSettings> authSettings, IUserRepository userRepository, IJwtFactory jwtFactory)
		{
			_userRepository = userRepository;
            _jwtFactory = jwtFactory;
            _authSettings = authSettings.Value;
		}

		public User Authenticate(string username, string password)
		{
			var user = _userRepository.GetSingleAsync(x => x.Username == username && x.Password == password).Result;
			if (user == null)
			{
				return null;
			}

            user.Token = string.IsNullOrEmpty(_authSettings.SecretKey) ? null : _jwtFactory.EncodeToken(user.Username);

			// Remove password before returning!
			user.Password = null;

			return user;
		}
	}
}