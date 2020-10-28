using AutoMapper;
using CRM.API.Auth;
using CRM.API.Models.InputModels;
using CRM.API.Models.InputModels.AuthInputModels;
using CRM.DB.Models;
using CrmRepository.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DevEdu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILeadRepository _leadRepository;
        
        public TokenController(ILeadRepository leadRepository, IMapper mapper )
        {
            _mapper = mapper;
            _leadRepository = leadRepository;
        }

        [HttpPost("google-token")]
        public async ValueTask<ActionResult> GoogleToken(GoogleAutnInputModel model)
        {
            LeadSearchInputModel searchModel = new LeadSearchInputModel() { Email = model.Email };
            var lead = await _leadRepository.SearchLead(_mapper.Map<LeadSearchModel>(searchModel));
            if (lead.RequestData.Count == 0)
            {
                return NotFound("Client not found, contact administrator");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, model.Email),
            };

            var accountsInfo = lead.RequestData[0].Accounts;
           
            var identity = new ClaimsIdentity(claims, "Token");

            var now = DateTime.Now;
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var accountsAmount = new List<string>();

            var client = new RestClient("https://localhost:44349");
            foreach (Account acc in accountsInfo)
            {
                var request = new RestRequest($"api/transaction/balance/{acc.Id}", Method.GET, DataFormat.Json);
                var resp = await client.ExecuteAsync(request);
                JObject jsonObj = JObject.Parse(resp.Content);
                accountsAmount.Add(jsonObj.SelectToken("balance").ToString());
            }
            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name,
                accountsInfo,
                accountsAmount
            };

            return Json(response);
        }

        [HttpPost("token")]
        public async Task<IActionResult> Token(AuthInputModel currentUser)
        {
            var identity = await GetIdentity(currentUser);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.Now;
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return Json(response);
        }


        private async Task<ClaimsIdentity> GetIdentity(AuthInputModel currentUser)
        {
            LeadSearchInputModel searchModel = new LeadSearchInputModel() { Login = currentUser.Login };
            var searchUser = await _leadRepository.SearchLead(_mapper.Map<LeadSearchModel>(searchModel));
            var userInfo = await _leadRepository.GetLeadById((long)searchUser.RequestData[0].Id);
            if (searchUser != null && Hashing.ValidateUserPassword(currentUser.Password, userInfo.RequestData.Password))
            {
                if (searchUser != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, currentUser.Login),
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");
                    return claimsIdentity;
                }
            }
            return null;
        }
    }
}