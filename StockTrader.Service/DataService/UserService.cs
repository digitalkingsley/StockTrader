using System;
using System.Collections.Generic;
using System.Text;
using StockTrader.StockTrader_Model;
using StockTrader.Service.InfrastructureService;
using System.Linq;
using System.Threading.Tasks;



namespace StockTrader.Service.DataService
{
   public class UserService: IUserService
    {
       
        private ITransactionService _transactionService;

        public UserService(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// This method allows a new user to register on the system
        /// </summary>
        /// <param name="user"></param>
        #region Method To Register A New User
        public async Task<string> Register(User user, StockTraderContext context)
        {
            var existingUser = context.User.FirstOrDefault(u => u.UserEmail == user.UserEmail);
            if (existingUser == null)
            {
              await context.User.AddAsync(user);
              await  context.SaveChangesAsync();
              return "success";
            }
            else
            {
                return "User Already Exists";
            }
        }
        #endregion

        /// <summary>
        /// Method authenticates a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Method returns a string</returns>
        #region Method To Authenticate A User
        public string Authenticate(User user, StockTraderContext context)
        {
            if(string.IsNullOrEmpty(user.UserEmail) || string.IsNullOrEmpty(user.UserPassword))
            {
                throw new Exception("No Email or Password supplied");
            }
            var _user = context.User.FirstOrDefault(u => u.UserEmail == user.UserEmail && u.UserPassword==user.UserPassword);

           if(_user==null)
            {
                return null;
            }
            else
            {
                return "success";
            }        
        }
        #endregion

        #region Method To Fund User Account
        public async Task<string> FundAccount(double amount, string email, StockTraderContext context)
        {
            if (string.IsNullOrEmpty(amount.ToString()))
            {
                throw new Exception("Invalid Amount");
            }
            else
            {
                User _user = context.User.FirstOrDefault(u => u.UserEmail == email);
                if (_user == null)
                {
                    throw new Exception("User Not Found");
                }
                else
                {
                    _user.UserAccountBalance += amount;
                    //Register this transaction
                    Transaction transactionReport = await _transactionService.AddTransactionForFunding(amount, _user, context, "funding");

                    if (transactionReport != null)
                    {
                        await context.SaveChangesAsync();
                        return "success";
                    }
                    else
                    {
                        throw new Exception("Transaction Failed");
                    }                  
                }
            }
        }
        #endregion

    }
}
