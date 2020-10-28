using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CRM.API.Common;
using CRM.API.Models.InputModels;
using CRM.API.Models.OutputModels;
using CRM.Core;
using CRM.Core.ConfigurationOptions;
using CrmRepository.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;

namespace CRM.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IOptions<UrlOptions> _urlOptions;
        private readonly IOptions<PayPalOptions> _paypalOptions;
        private readonly IAccountRepository _accountRepository;
        public TransactionController(IOptions<UrlOptions> urlOptions, IOptions<PayPalOptions> paypalOptions, IAccountRepository accountRepository)
        {
            _urlOptions = urlOptions;
            _paypalOptions = paypalOptions;
            _accountRepository = accountRepository;
        }

        /// <summary>
        /// Gets specific transaction
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Transactions with specific Id</returns>
        /// <response code="201">Returns transaction with specific Id</response>
        /// <response code="400">Transaction doesn't exist</response>  
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async ValueTask<ActionResult<TransactionEntityOutputModel>> TransactionGetById(long id)
        {
            string URI = $"api/transaction/{id}";
            var result = await RequestSender.SendRequest<TransactionEntityOutputModel>(_urlOptions.Value.TransactionStoreApiUrl, URI);
            return result.Data;
        }

        /// <summary>
        /// Gets list of all transactions
        /// </summary>
        /// <returns>List of all transactions</returns>
        /// <response code="201">Returns list of all existing transactions</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async ValueTask<ActionResult<List<TransactionEntityOutputModel>>> GetAllTransaction()
        {
            string URI = $"api/transaction/";
            var result = await RequestSender.SendRequest<List<TransactionEntityOutputModel>>(_urlOptions.Value.TransactionStoreApiUrl, URI);
            return result.Data;
        }

        /// <summary>
        /// Adds deposit transaction by inputModel
        /// </summary>
        /// <param name="transactionEntityInputModel"></param>
        /// <remarks>
        /// Sample request:
        ///     {
        ///        "Amount": 3000,
        ///        "Account": 
        ///        {
        ///				"Id": 111,
        ///				"CurrencyId": 1
        ///        }
        ///     }
        /// </remarks>
        /// <returns>Created transaction</returns>
        /// <response code="201">Returns the newly created transaction</response>
        /// <response code="400">Transaction wasn't created</response>  
        [HttpPost("deposit")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async ValueTask<ActionResult<TransactionEntityOutputModel>> DepositTransactionInsert(TransactionEntityInputModel transactionEntityInputModel)
        {
            string URIdeposit = $"api/transaction/deposit";
            string URIgetBalance = $"api/transaction/balance/{transactionEntityInputModel.Account.Id}";
            var account = await _accountRepository.GetAccountById(transactionEntityInputModel.Account.Id);
            if (!account.IsOkay)
                return Problem($"Request failed {account.ExMessage}", statusCode: 520);
            if (account.RequestData == null)
                return NotFound($"AccountId: {transactionEntityInputModel.Account.Id} not found");
            var balance = await RequestSender.SendRequest<AccountBalanceOutputModel>(_urlOptions.Value.TransactionStoreApiUrl, URIgetBalance, Method.GET);
            if (balance == null)
                return Problem($"Request failed", statusCode: 520);
            transactionEntityInputModel.TimeStamp = balance.Data.TimeStamp;

            var result = await RequestSender.SendRequest<TransactionEntityOutputModel>(_urlOptions.Value.TransactionStoreApiUrl, URIdeposit, Method.POST, transactionEntityInputModel);
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return Problem($"Transaction failed: {result.Content}", statusCode: (int)result.StatusCode);
            return result.Data;
        }

        /// <summary>
        /// Adds withdraw transaction by inputModel
        /// </summary>
        /// <param name="transactionEntityInputModel"></param>
        /// <remarks>
        /// Sample request:
        ///     {
        ///        "Amount": 3000,
        ///        "Account": 
        ///        {
        ///				"Id": 111,
        ///				"CurrencyId": 1
        ///        }
        ///     }
        /// </remarks>
        /// <returns>Created transaction</returns>
        /// <response code="201">Returns the newly created transaction</response>
        /// <response code="400">Transaction wasn't created</response>  
        [HttpPost("withdraw")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async ValueTask<ActionResult<TransactionEntityOutputModel>> WithdrawTransactionInsert(TransactionEntityInputModel transactionEntityInputModel)
        {
            string URIwithdraw = $"api/transaction/withdraw";
            string URIgetBalance = $"api/transaction/balance/{transactionEntityInputModel.Account.Id}";
            var account = await _accountRepository.GetAccountById(transactionEntityInputModel.Account.Id);
            if (!account.IsOkay)
                return Problem($"Request failed {account.ExMessage}", statusCode: 520);
            if (account.RequestData == null)
                return NotFound($"AccountId: {transactionEntityInputModel.Account.Id} not found");
            var balance = await RequestSender.SendRequest<AccountBalanceOutputModel>(_urlOptions.Value.TransactionStoreApiUrl, URIgetBalance, Method.GET);
            if (balance == null)
                return Problem($"Request failed:", statusCode: 520);
            if (balance.Data.Balance < transactionEntityInputModel.Amount)
                return BadRequest($"Request failed: insufficient funds");
            transactionEntityInputModel.TimeStamp = balance.Data.TimeStamp;

            var result = await RequestSender.SendRequest<TransactionEntityOutputModel>(_urlOptions.Value.TransactionStoreApiUrl, URIwithdraw, Method.POST, transactionEntityInputModel);
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return Problem($"Transaction failed: {result.Content}", statusCode: (int)result.StatusCode);
            return result.Data;
        }

        /// <summary>
        /// Adds transfer transaction by inputModel
        /// </summary>
        /// <param name="transferInputModel"></param>
        /// <remarks>
        /// Sample request:
        ///     {
        ///        "Amount": 3000,
        ///        "SenderAccount": 
        ///        {
        ///				"Id": 111,
        ///				"CurrencyId": 1
        ///        },
        ///        "RecipientAccount": 
        ///        {
        ///				"Id": 112,
        ///				"CurrencyId": 3
        ///        }
        ///     }
        /// </remarks>
        /// <returns>List of created transactions (deposit and withdraw)</returns>
        /// <response code="201">Returns list of the newly created transactions</response>
        /// <response code="400">Transactions weren't created</response>  
        [HttpPost("transfer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async ValueTask<ActionResult<List<TransactionEntityOutputModel>>> TransferTransactionInsert(TransferInputModel transferInputModel)
        {
            string URItransfer = $"api/transaction/transfer";
            string URIgetSenderBalance = $"api/transaction/balance/{transferInputModel.SenderAccount.Id}";
            string URIgetRecipientAccount = $"api/transaction/balance/{transferInputModel.RecipientAccount.Id}";
            var senderAccount = await _accountRepository.GetAccountById(transferInputModel.SenderAccount.Id);
            var recipientAccount = await _accountRepository.GetAccountById(transferInputModel.RecipientAccount.Id);
            if (!senderAccount.IsOkay && recipientAccount.IsOkay)
            {
                var message = "";
                if (!senderAccount.IsOkay)
                    message = $"Transaction failed {senderAccount.ExMessage}";
                if (!recipientAccount.IsOkay)
                    message += $"Transaction failed {recipientAccount.ExMessage}";
                return Problem(message, statusCode: 520);
            }
            if (senderAccount.RequestData == null)
                return NotFound($"SenderAccountId: {transferInputModel.SenderAccount.Id} not found");
            if (recipientAccount.RequestData == null)
                return NotFound($"RecipientAccountId: {transferInputModel.RecipientAccount.Id} not found");

            var senderBalance = await RequestSender.SendRequest<AccountBalanceOutputModel>(_urlOptions.Value.TransactionStoreApiUrl, URIgetSenderBalance, Method.GET);
            if (senderBalance == null)
                return Problem($"Request failed:", statusCode: 520);
            transferInputModel.SenderTimeStamp = senderBalance.Data.TimeStamp;
            if (senderBalance.Data.Balance < transferInputModel.Amount) { return BadRequest($"Not enough funds to complete the transaction"); }

            var recipientBalance = await RequestSender.SendRequest<AccountBalanceOutputModel>(_urlOptions.Value.TransactionStoreApiUrl, URIgetRecipientAccount, Method.GET);
            if (recipientBalance == null)
                return Problem($"Request failed:", statusCode: 520);
            transferInputModel.RecipientTimeStamp = recipientBalance.Data.TimeStamp;

            var result = await RequestSender.SendRequest<List<TransactionEntityOutputModel>>(_urlOptions.Value.TransactionStoreApiUrl, URItransfer, Method.POST, transferInputModel);
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return Problem($"Transaction failed: {result.Content}", statusCode: (int)result.StatusCode);
            return result.Data;
        }

        [HttpGet("get-token")]
        public async ValueTask<string> GetAccessToken()
        {
            var client = new RestClient("https://api.sandbox.paypal.com/v1/oauth2/token");

            client.Authenticator = new HttpBasicAuthenticator($"{_paypalOptions.Value.ClientId}", $"{_paypalOptions.Value.Secret}");

            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");
            var tResponse = await client.ExecuteAsync(request);
            var responseJson = tResponse.Content;
            var token = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson)["access_token"].ToString();
            return token;
        }


        [HttpPost("create-payment")]
        public async ValueTask<string> CreatePayment()
        {
            JsonData jsonData = new JsonData();
            var token = await GetAccessToken();
            var client = new RestClient("https://api.sandbox.paypal.com");
            client.AddDefaultHeader("Authorization", $"Bearer {token}");
            var request = new RestRequest("v1/payments/payment", Method.POST, DataFormat.Json);

            jsonData.intent = "sale";
            jsonData.payer = new Payer() { payment_method = "paypal" };
            jsonData.transactions = new Transaction[]
            {
                new Transaction()
                {
                    amount = new Amount()
                    {
                            total = "30.11",
                            currency = "USD"
                    }
                }
            };
            jsonData.redirect_urls = new RedirectUrls()
            {
                return_url = "https://example.com/return",
                cancel_url = "https://example.com/cancel"
            };
            request.AddJsonBody(jsonData);
            var response = await client.ExecuteAsync(request);
            return response.Content;
        }


        [HttpGet("refund-payment/{paymentId}")]
        public async ValueTask<bool> RefundPayment(string paymentId)
        {

            var token = await GetAccessToken();
           
            var client = new RestClient("https://api.sandbox.paypal.com");
            client.AddDefaultHeader("Authorization", $"Bearer {token}");
            var request = new RestRequest($"v2/payments/sale/{paymentId}/refund", Method.GET, DataFormat.Json);

            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");

            var response = await client.ExecuteAsync(request);
            return response.IsSuccessful;
        }
    }
}

