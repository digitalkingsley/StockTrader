using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Threading.Tasks;
using StockTrader.Service.DataService;
using StockTrader.StockTrader_Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using FluentAssertions;

namespace StockTrader.Test
{
    public class UserServiceTest
    {
        //Create and return a Context to work with the InMemory Db the Test Class
        //and ensure that the Db is recreated for each test run.
        private StockTraderContext GetContextWithSomeData()
        {
            var options = new DbContextOptionsBuilder<StockTraderContext>()
                              .UseInMemoryDatabase(Guid.NewGuid().ToString())
                              .Options;
            var context = new StockTraderContext(options);

            //Add some Seed data to the Db
            context.User.AddRange(new User
            {
                UserId=1,
                UserEmail="k1@gmail.com",
                UserName="kinso1",
                UserPassword="kinso1"
            },
            new User
            {
                UserId=2,
                UserEmail = "k2@gmail.com",
                UserName = "kinso2",
                UserPassword = "kinso2"
            }     
            );
            return context;
        }

        [Fact(DisplayName = "Method should return a registered user")]
        public void Register_User()
        {

            var context = GetContextWithSomeData();

            Mock<TransactionService> transactionServiceMock = new Mock<TransactionService>();
            UserService userService = new UserService(context, transactionServiceMock.Object);

            //Create a User Object to be authenticated
            var userToAuthenticate = new User
            {
                UserEmail = "k2@gmail.com",
                UserName = "kinso2",
                UserPassword = "kinso2"
            };

            var result = userService.Authenticate(userToAuthenticate);
            var okResult = result.Should().BeOfType<string>().Subject;
            okResult.Should().Be("success");

        }

    }
}
