using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyApi.Contracts.Converters;
using CompanyApi.Contracts.Dto;
using CompanyApi.Contracts.Entities;
using CompanyApi.Controllers.Base;
using CompanyApi.Core.Errors;
using CompanyApi.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CompanyApi.Controllers
{
	//[Authorize]
	[ApiController]
	[ApiVersion("1.0", Deprecated = true)]
	[ApiVersion("1.1")]
	[EnableCors("EnableCORS")]
    [Route("api/[controller]")]
    public class CompaniesController : BaseController<CompaniesController>
	{
        public ICompanyRepository CompanyRepository;
        private readonly IConverter<Company, CompanyDto> _companyToDtoConverter;
        private readonly IConverter<IList<Company>, IList<CompanyDto>> _companyToDtoListConverter;

        public CompaniesController(ICompanyRepository companyRepository,
			IConverter<Company,CompanyDto> companyToDtoConverter,
			IConverter<IList<Company>, IList<CompanyDto>> companyToDtoListConverter)
        {
	        CompanyRepository = companyRepository;
            _companyToDtoConverter = companyToDtoConverter;
	        _companyToDtoListConverter = companyToDtoListConverter;
        }

		/// <summary>
		/// Create Company
		/// </summary>
		/// <remarks>This API will create new Company</remarks>
		/// POST /api/companies/create/{company}
		/// <param name="company">Company model</param>
		[MapToApiVersion("1.1")]
		[HttpPost("create", Name = "CreateCompany")]
		[ProducesResponseType(201, Type = typeof(Company))]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public async Task<IActionResult> CreateAsync([FromBody] Company company)
		{
			Logger.LogDebug("CreateAsync");
			if (company == null)
			{
				return BadRequest(new BadRequestError("The company is null"));
			}
			await CompanyRepository.AddAsync(company);
			return CreatedAtRoute("GetCompanyById", new
			{
				Controller = "Companies",
				id = company.CompanyId
			}, company);
		}

		/// <summary>
		/// Delete Company
		/// </summary>
		/// <remarks>This API will delete Company with Id</remarks>
		/// DELETE /api/companies/{id}
		/// <param name="id"></param>
		/// <returns></returns>
	    [MapToApiVersion("1.1")]
		[HttpDelete("{id}", Name = "DeleteCompany")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			Logger.LogDebug("DeleteAsync");
			var company = await CompanyRepository.GetSingleAsync(cmp => cmp.CompanyId == id);
			if (company == null)
			{
				return NotFound(new NotFoundError("The company was not found"));
			}
			await CompanyRepository.DeleteAsync(company);
			return NoContent();
		}

		/// <summary>
		/// Get all Companies
		/// </summary>
		/// <remarks>This API return list of all Companies</remarks>
		/// GET api/companies/getall
		/// <returns>List of Companies</returns>
		[MapToApiVersion("1.1")]
		[HttpGet("getall", Name = "GetAllCompanies")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<CompanyDto>))]
		[ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAllAsync()
        {
	        Logger.LogDebug("GetAllAsync");
			var companies = await CompanyRepository.GetAllAsync().ConfigureAwait(false);
	        if (!companies.Any())
	        {
		        return NotFound(new NotFoundError("The companies list is empty"));
	        }
	        var companiesDto = _companyToDtoListConverter.Convert(companies);
            return Ok(companiesDto);
        }

		/// <summary>
		/// Get Company
		/// </summary>
		/// <remarks>This API return Company with Id</remarks>
		/// GET /api/companies/{id}
		/// <param name="id"></param>
		/// <returns>Return Company</returns>
		[AllowAnonymous]
		[MapToApiVersion("1.1")]
		[HttpGet("{id}", Name = "GetCompanyById")]
		[ProducesResponseType(200, Type = typeof(CompanyDto))]
		[ProducesResponseType(404)]
		public async Task<ActionResult<CompanyDto>> GetAsync(int id)
		{
			Logger.LogDebug("GetAsync");
			var company = await CompanyRepository.GetSingleAsync(cmp => cmp.CompanyId == id);
			if (company == null)
			{
				return NotFound(new NotFoundError("The company was not found"));
			}
			var companyDto = _companyToDtoConverter.Convert(company);
			return Ok(companyDto);
		}

		/// <summary>
		/// Update Company
		/// </summary>
		/// POST /api/companies/update/{company}
		/// <param name="company"></param>
		/// <returns>Returns updated Company</returns>
		[MapToApiVersion("1.1")]
		[HttpPost("update", Name = "UpdateCompany")]
		[ProducesResponseType(201, Type = typeof(CompanyDto))]
		[ProducesResponseType(400)]
		public async Task<IActionResult> UpdateAsync([FromBody] Company company)
		{
			Logger.LogDebug("UpdateAsync");
			if (company == null)
			{
				return BadRequest(new BadRequestError("The retrieved company is null"));
			}
			var updatedCompany = await CompanyRepository.UpdateAsync(company);
			if (updatedCompany == null)
			{
				return BadRequest(new BadRequestError("The updated company is null"));
			}

			return CreatedAtRoute("GetCompanyById", new
			{
				Controller = "Companies",
				id = updatedCompany.CompanyId
			}, company);
		}
	}
}
