using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockTrader.Service.InfrastructureService;
using StockTrader.Service.DataService;
using Microsoft.Extensions.Configuration;
using StockTrader.StockTrader_Model;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StockTrader.Controllers
{
    [Route("api/v1/[controller]")]
    public class TransactionsController : ControllerBase
    {

        private IStockHelperService _stockHelperService;
        private StockTraderContext _context;
        private ITransactionService _transactionService;

        public TransactionsController(IStockHelperService stockHelperService, StockTraderContext context, ITransactionService transactionService)
        {
            _context = context;
            _stockHelperService = stockHelperService;
            _transactionService = transactionService;
        }

        /// <summary>
        /// EndPoint fetches all transactions associated with a user
        /// </summary>
        /// <returns>EndPoint returns a collection of Transactions if found</returns>
        #region EndPoint To Fetch All User Transactions 
        [Authorize]
        [HttpGet("all")]
        public IActionResult GetAllTransactions()
        {
            //Get the current user
            string eMail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var currentUser = _stockHelperService.GetCurrentUser(eMail, _context);

            if (currentUser != null)
            {
                //Fetch all transactions for user
                var allTransactions = _transactionService.GetAllTransactionsByUser(currentUser);
                if (allTransactions != null)
                {
                    return Ok(allTransactions);
                }
                else
                {
                    return Content("No Transactions Found For User");
                }
            }
            else
            {
                return NotFound("No User Found");
            }
        }
        #endregion

        /// <summary>
        /// EndPoint Fetches a summary of transactions associated with user over a given period
        /// When sending-in these date values with JavaScript from the client, the instruction below should be used
        /// {
        /// var date=new Date();
        /// date.toJason(). 
        /// }
        /// This date produced represents a C# DateTime. These values should be sent as query-string paramaters
        /// </summary>
        /// <returns>EndPoint returns a list of all transactions associated with a user within a given period</returns>
        #region EndPoint To Fetch User Transactions For A Given Period
        //[Authorize]
        //[HttpGet("range")]
        //public IActionResult GetTransactionsByPeriod([FromQuery]DateTime startDate, [FromQuery]DateTime endDate)
        //{
        //    var currentUser = _stockHelperService.GetCurrentUser();
        //    if(currentUser!=null)
        //    {
        //        var userTransactions = _transactionService.GetAllTransactionsByUser(currentUser, startDate, endDate);
        //        if(userTransactions!=null)
        //        {
        //            return Ok(userTransactions);
        //        }
        //        else
        //        {
        //            return Content("No Transactions Found For User");
        //        }
        //    }
        //    else
        //    {
        //        return NotFound("User Not Found");
        //    }

        //}
        #endregion

    }
}
