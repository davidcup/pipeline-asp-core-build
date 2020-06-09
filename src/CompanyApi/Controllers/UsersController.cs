using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyApi.Contracts.Dto;
using CompanyApi.Contracts.Entities;
using CompanyApi.Controllers.Base;
using CompanyApi.Core.Errors;
using CompanyApi.Services.Authorization;
using CompanyApi.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CompanyApi.Controllers
{
	[Authorize]
	[ApiController]
    [ApiVersion("1.1")]
    [EnableCors("EnableCORS")]
	[Route("api/[controller]")]
	public class UsersController : BaseController<UsersController>
	{
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        public UsersController(IUserService userService, IUserRepository userRepository)
		{
			_userService = userService;
			_userRepository = userRepository;
		}

        /// <summary>
        /// Authenticate User
        /// </summary>
        /// <remarks>This API will create new User</remarks>
        /// POST /api/users/authenticate/{user}
        /// <param name="user"></param>
        /// <returns>User with token</returns>
		[AllowAnonymous]
		[HttpPost("authenticate")]
		public IActionResult Authenticate([FromBody] UserDto user)
		{
			var existingUser = _userService.Authenticate(user.Username, user.Password);
			if (existingUser == null)
			{
				return BadRequest(new {message = "Username or password is incorrect"});
			}
			return Ok(existingUser);
		}

        /// <summary>
        /// Create User
        /// </summary>
        /// <remarks>This API will create new User</remarks>
        /// POST /api/users/create/{user}
        /// <param name="user">User model</param>
        [MapToApiVersion("1.1")]
        [HttpPost("create", Name = "CreateUser")]
        [ProducesResponseType(201, Type = typeof(User))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateAsync([FromBody] User user)
        {
            Logger.LogDebug("CreateAsync");
            if (user == null)
            {
                return BadRequest(new BadRequestError("The user is null"));
            }
            await _userRepository.AddAsync(user);
            return CreatedAtRoute("GetUserByUserName", new
            {
                Controller = "Users",
                userName = user.Username
            }, user);
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <remarks>This API will delete User with userName</remarks>
        /// GET /api/users/{userName}
        /// <param name="userName"></param>
        /// <returns>Return User</returns>
        [MapToApiVersion("1.1")]
        [HttpDelete("{userName}", Name = "DeleteUser")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteAsync(string userName)
        {
            Logger.LogDebug("DeleteAsync");
            var user = await _userRepository.GetSingleAsync(usr => usr.Username == userName);
            if (user == null)
            {
                return NotFound(new NotFoundError("The User was not found"));
            }
            await _userRepository.DeleteAsync(user);
            return NoContent();
        }

        /// <summary>
        /// Get all Users
        /// </summary>
        /// <remarks>This API return list of all Users</remarks>
        /// GET /api/users/getall
        /// <returns>List of Users</returns>
        [MapToApiVersion("1.1")]
        [HttpGet("getall")]
        public async Task<ActionResult<IList<User>>> GetAllAsync()
        {
            Logger.LogDebug("GetAllAsync");
            var users = await _userRepository.GetAllAsync().ConfigureAwait(false);
            if (!users.Any())
            {
                return NotFound(new NotFoundError("The Users list is empty"));
            }
            var result = users.Select(i => new
            {
                i.Username,
                i.Password,
                EmployeeFirstName = i.Employee?.FirstName,
                EmployeeLastName = i.Employee?.LastName
            }).ToList();
            return Ok(result);
        }

        /// <summary>
        /// Get User
        /// </summary>
        /// <remarks>This API return User with Username</remarks>
        /// GET /api/users/{userName}
        /// <param name="userName"></param>
        /// <returns>Return User</returns>
        [MapToApiVersion("1.1")]
        [HttpGet("{userName}", Name = "GetUserByUserName")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<User>> GetAsync(string userName)
        {
            Logger.LogDebug("GetAsync");
            var user = await _userRepository.GetSingleAsync(usr => usr.Username == userName);
            if (user == null)
            {
                return NotFound(new NotFoundError("The User was not found"));
            }
            return Ok(user);
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// POST /api/users/update/{user}
        /// <param name="user"></param>
        /// <returns>Returns updated User</returns>
        [MapToApiVersion("1.1")]
        [HttpPost("update", Name = "UpdateUser")]
        [ProducesResponseType(201, Type = typeof(User))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateAsync([FromBody] User user)
        {
            Logger.LogDebug("UpdateAsync");
            if (user == null)
            {
                return BadRequest(new BadRequestError("The retrieved user is null"));
            }
            var updatedUser = await _userRepository.UpdateAsync(user);
            if (updatedUser == null)
            {
                return BadRequest(new BadRequestError("The updated user is null"));
            }
            return CreatedAtRoute("GetUserByUserName", new
            {
                Controller = "Users",
                userName = updatedUser.Username
            }, user);
        }
	}
}