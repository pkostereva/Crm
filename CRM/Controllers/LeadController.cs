using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CRM.API.Common;
using CRM.API.Models.InputModels;
using CRM.API.Models.OutputModels;
using CRM.Core;
using CRM.DB.Models;
using CrmRepository.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp;

namespace CRM.API.Controllers
{
   // [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class LeadController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILeadRepository _leadRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IOptions<UrlOptions> _urlOptions;

        public LeadController(ILeadRepository leadRepository, IAccountRepository accountRepository, IMapper mapper, IOptions<UrlOptions> urlOptions)
        {
            _leadRepository = leadRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _urlOptions = urlOptions;
        }

        /// <summary>
        /// Adds lead by inputModel
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {
        ///        "FirstName": "Екатерина",
        ///        "LastName": "Зверь",
        ///        "Patronymic": "Елисеевна",
        ///        "BirthDate": "01.11.1995",
        ///        "Email": "ZZzver@gmail.com",
        ///        "Password": "notDifficultSuperSuperPassword",
        ///        "Phone": "+79942228872",
        ///        "Login": "ZZzzver1995",
        ///        "CityId": 3
        ///     }
        /// </remarks>
        /// <param name="leadInputModel">Use inputModel without Id to add lead</param>
        /// <returns>Created lead</returns>
        /// <response code="201">Returns the newly created lead</response>           
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async ValueTask<ActionResult<LeadOutputModel>> AddLead(LeadInputModel leadInputModel)
        {
            var result = await _leadRepository.AddOrUpdateLead(_mapper.Map<Lead>(leadInputModel));
            if (result.IsOkay)
            {
                if (result.RequestData == null) { return Problem($"Added lead not found", statusCode: 520); }
                return Ok(_mapper.Map<LeadOutputModel>(result.RequestData));
            }
            return Problem($"Transaction failed {result.ExMessage}", statusCode: 520);
        }

        /// <summary>
        /// Updates lead by inputModel
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {
        ///        "Id": 113,
        ///        "FirstName": "Екатерина",
        ///        "LastName": "Зверь",
        ///        "Patronymic": "Елисеевна",
        ///        "BirthDate": "01.11.1995",
        ///        "Email": "ZZzver@gmail.com",
        ///        "Password": "notDifficultSuperSuperPassword",
        ///        "Phone": "+79942228872",
        ///        "Login": "ZZzzver1995",
        ///        "CityId": 3
        ///     }
        /// </remarks>
        /// <param name="leadInputModel">Use inputModel with Id to update lead</param>
        /// <returns>Updated lead</returns>
        /// <response code="201">Returns updated lead</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async ValueTask<ActionResult<LeadOutputModel>> UpdateLead(LeadInputModel leadInputModel)
        {
            if (leadInputModel.Id < 1) return BadRequest("LeadId can not be less than one.");
            var result = await _leadRepository.AddOrUpdateLead(_mapper.Map<Lead>(leadInputModel));
            if (result.IsOkay)
            {
                if (result.RequestData == null) { return Problem($"Updated lead not found", statusCode: 520); }
                return Ok(_mapper.Map<LeadOutputModel>(result.RequestData));
            }
            return Problem($"Transaction failed {result.ExMessage}", statusCode: 520);
        }

        /// <summary>
        /// Gets specific lead by Id
        /// </summary>
        /// <param name="leadId"></param>
        /// <returns>Lead with specific Id</returns>
        /// <response code="201">Returns lead with specific Id</response>
        /// <response code="400">Lead deleted or doesn't exist</response>     
        [HttpGet("{leadId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async ValueTask<ActionResult<LeadOutputModel>> GetLeadById(long leadId)
        {
            if (leadId < 1) return BadRequest("LeadId can not be less than one.");
            var result = await _leadRepository.GetLeadById(leadId);
            if (result.IsOkay)
            {
                if (result.RequestData == null) { return NotFound("Lead not found"); }
                return Ok(_mapper.Map<LeadOutputModel>(result.RequestData));
            }
            return Problem($"Transaction failed {result.ExMessage}", statusCode: 520);
        }

        /// <summary>
        /// Gets all not deleted leads
        /// </summary>
        /// <returns>All not deleted leads</returns>
        /// <response code="201">Returns all not deleted leads</response>  
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async ValueTask<ActionResult<List<LeadOutputModel>>> GetAllLeads()
        {
            var result = await _leadRepository.GetAllLeads();
            if (result.IsOkay)
            {
                if (result.RequestData == null) { return NotFound("Leads not found"); }
                return Ok(_mapper.Map<List<LeadOutputModel>>(result.RequestData));
            }
            return Problem($"Transaction failed {result.ExMessage}", statusCode: 520);
        }

        /// <summary>
        /// Searches single lead or list of leads by specific parameters
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     for search:
        ///     {
        ///        FirstName = "%лия",
        ///        FirstNameOperator = 3,
        ///        LastName = "%орот%",
        ///        LastNameOperator = 3,
        ///        Patronymic = "%ьевна",
        ///        PatronymicOperator = 3,
        ///        BirthDate = "01.01.1990",
        ///        BirthDateDateEnd = "03.01.1992",
        ///        Email = "%orot%",
        ///        EmailOperator = 3,
        ///        Phone = "%4102887%",
        ///        PhoneOperator = 3,
        ///        Login = "%orot%",
        ///        LoginOperator = 3,
        ///        RegistrationDate = "11.04.2020",
        ///        RegistrationDateEnd = DateTime.Now.ToString(),
        ///        CitiesValues = "3, 2, 1",
        ///        IsDeletedInclude = true,
        ///        AccountId = 32
        ///     }
        /// </remarks>
        /// <param name="inputModel">To execute search of lead or list of leads by specific parameters</param>
        /// <returns>Found lead/list of leads</returns>
        /// <response code="201">Returns found lead or list of leads</response>
        /// <response code="400">Requested lead doesn't exist</response>         
        [HttpPost("search")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async ValueTask<ActionResult<List<LeadOutputModel>>> SearchLead(LeadSearchInputModel inputModel)
        {
            var result = await _leadRepository.SearchLead(_mapper.Map<LeadSearchModel>(inputModel));
            if (result.IsOkay)
            {
                if (result.RequestData == null) { return NotFound("Lead not found"); }
                return Ok(_mapper.Map<List<LeadOutputModel>>(result.RequestData));
            }
            return Problem($"Transaction failed {result.ExMessage}", statusCode: 520);
        }

        /// <summary>
        /// Soft delete of specific lead
        /// </summary>
        /// <param name="leadId"></param>
        /// <response code="201">Will be returned after succsessful delete</response>
        /// <response code="400">Lead requested by Id doesn't exist</response>
        [HttpDelete("{leadId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async ValueTask<ActionResult> LeadSoftDelete(long leadId)
        {
            if (leadId < 1) return BadRequest("LeadId can not be less than one.");
            var result = await _leadRepository.DeleteLeadById(leadId);
            if (result.IsOkay) { return Ok(); }
            return Problem($"Transaction failed {result.ExMessage}", statusCode: 520);
        }

        /// <summary>
        /// Adds lead's account by inputModel
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     for adding:
        ///     {
        ///        "LeadId": 1234567,
        ///        "CurrencyId": 3
        ///     }
        /// </remarks>
        /// <param name="accountInputModel"></param>
        /// <returns>Created lead's account</returns>
        /// <response code="201">Returns the newly created lead's account</response>      
        [HttpPost("account")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async ValueTask<ActionResult<AccountOutputModel>> AddAccount(AccountInputModel accountInputModel)
        {
            string URIdeposit = $"api/transaction/initialize";
            if (accountInputModel.CurrencyId < 1) return BadRequest("Account cannot be added");
            var result = await _accountRepository.AddAccount(_mapper.Map<Account>(accountInputModel));
            TransactionEntityInputModel transactionEntityInputModel = new TransactionEntityInputModel { Amount = 19000  };

            if (result.IsOkay)
            {
                if (result.RequestData == null) { return Problem($"Added account not found", statusCode: 520); }

                transactionEntityInputModel.Account = new AccountShortInputModel 
                {
                    Id = (long)result.RequestData.Id, 
                    CurrencyId = result.RequestData.Currency.Id 
                };
                var requestResult = await RequestSender.SendRequest<TransactionEntityOutputModel>(_urlOptions.Value.TransactionStoreApiUrl, URIdeposit, Method.POST, transactionEntityInputModel);

                if (requestResult.IsSuccessful)
                {
                    return Ok(_mapper.Map<AccountOutputModel>(result.RequestData));
                }
            }
            return Problem($"Transaction failed {result.ExMessage}", statusCode: 520);
        }

        /// <summary>
        /// Soft deletes a specific lead's account
        /// </summary>
        /// <param name="accountId"></param>
        /// <response code="201">Will be returned after succsessful delete</response>
        /// <response code="400">Account for delete doesn't exist</response>   
        [HttpDelete("account/{accountId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async ValueTask<ActionResult> AccountSoftDelete(long accountId)
        {
            if (accountId < 1) return BadRequest("No account was found for this lead.");
            var result = await _accountRepository.DeleteAccountLeadById(accountId);
            if (result.IsOkay) { return Ok(); }
            return Problem($"Transaction failed {result.ExMessage}", statusCode: 520);
        }

        /// <summary>
        /// Gets specific lead's account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>Account with specific Id</returns>
        /// <response code="201">Returns lead's account with specific Id</response>
        /// <response code="400">Account deleted or doesn't exist</response>  
        [HttpGet("account/{accountId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async ValueTask<ActionResult<AccountOutputModel>> GetAccountById(long accountId)
        {
            if (accountId < 1) return BadRequest("accountId can not be less than one.");
            var result = await _accountRepository.GetAccountById(accountId);
            if (result.IsOkay)
            {
                if (result.RequestData == null) { return NotFound("account not found"); }
                return Ok(_mapper.Map<AccountOutputModel>(result.RequestData));
            }
            return Problem($"Transaction failed {result.ExMessage}", statusCode: 520);
        }

        /// <summary>
        /// Gets all not deleted lead's accounts
        /// </summary>
        /// <returns>All not deleted lead's accounts</returns>
        /// <response code="201">Returns all not deleted lead's accounts</response>
        [AllowAnonymous]
        [HttpGet("account")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async ValueTask<ActionResult<List<AccountOutputModel>>> GetAllAccounts()
        {
            var result = await _accountRepository.GetAllAccounts();
            if (result.IsOkay)
            {
                if (result.RequestData == null) { return NotFound("There isn't any accounts to show"); }
                return Ok(_mapper.Map<List<AccountOutputModel>>(result.RequestData));
            }
            return Problem($"Transaction failed {result.ExMessage}", statusCode: 520);
        }

        [AllowAnonymous]
        [HttpGet("process-action")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async ValueTask<ActionResult<List<AccountOutputModel>>> ProcessAction(int operation, long account, decimal amount)
        {
            var result = await _accountRepository.GetAllAccounts();
            if (result.IsOkay)
            {
                if (result.RequestData == null) { return NotFound("There isn't any accounts to show"); }
                return Ok(_mapper.Map<List<AccountOutputModel>>(result.RequestData));
            }
            return Problem($"Transaction failed {result.ExMessage}", statusCode: 520);
        }

    }
}