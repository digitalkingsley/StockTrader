using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockTrader.Service.InfrastructureService;
using StockTrader.StockTrader_Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;
using StockTrader.Service.DataService;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using NSwag.Generation.Processors.Contexts;
using System.IO;
using System.Xml.XPath;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StockTrader.Controllers
{

           
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        //These are needed for dependency injection
        private IUserService _users;
        private IStockHelperService _stockServiceHelper;

        public UsersController(IUserService users, IStockHelperService stockServiceHelper)
        {
           _users = users;
           _stockServiceHelper = stockServiceHelper;
        }


        #region EndPoint For User Registration
        /// <summary>
        /// Creates/Registers a new user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/users/register
        ///     {
        ///        "UserName": "kingsley",
        ///        "UserEmail": "kingsleycokei@gmail.com",
        ///        "UserPassword": "kinso"
        ///     }
        ///
        /// </remarks>
        /// <param name="user"></param>
        /// <returns>A newly registered user</returns>
        /// <response code="200">If a user was successfully registered/created
        /// <example>
        /// Sample response:
        /// {
        ///     "userId": 1012,
        ///     "userName": "kinsola",
        ///     "userPassword": "k1",
        ///     "userEmail": "k0@gmail.com",
        ///     "userAccountBalance": 500.0,
        ///     "stock": [],
        ///     "transaction": []
        /// }
        /// </example>
        /// </response>
        /// <response code="400">If a required field is missing in the request</response>       
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                   await _users.Register(user);
                   return Ok(user);

                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }
            else
            {
                return BadRequest(new { error = "Required fields are not supplied" });
            }               
        }
        #endregion

        #region EndPoint For User Authentication
        /// <summary>
        /// Allows a user to attempt a sign-in
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/users/signin
        ///     {
        ///
        ///        "UserEmail": "kingsleycokei@gmail.com",
        ///        "UserPassword": "kinso"
        ///     }
        ///
        /// </remarks>
        /// <param name="user"></param>
        /// <returns>A Token and its expiry date</returns>
        /// <response code="200">Returns the newly created item</response>
        /// <response code="400">If a required field is missing in the request</response>   
        [HttpPost("signin")]
        public IActionResult Authenticate([FromBody]User user)
        {
            try
            {

                var report = _users.Authenticate(user);

                if (report == "success")
                {
                    var authToken = _stockServiceHelper.GenerateAuthToken(user);
                    string serializedToken = _stockServiceHelper.SerializeToken(authToken);

                    return Ok(new
                    {
                        token = serializedToken,
                        expires = authToken.ValidTo                  
                    });
                }
                else
                {
                    return NotFound("Authentication Failed");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region EndPoint For Funding Using Account  
        [Authorize]
        [HttpPost("account/fund")]
        public async Task<IActionResult> FundAccount([FromBody] double amount)
        {

            string eMail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            string transactionReport = await _users.FundAccount(amount, eMail);
            if (transactionReport == "success")
            {
                return Ok("Success");
            }
            else
            {
                return Content("Transaction Failed");
            }
        }
        #endregion
    }
}

