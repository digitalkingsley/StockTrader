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
        //Seed the InMemory Db with data
        StockTraderContext context;
        private StockTraderContext GetContextWithData()
        {
            var options = new DbContextOptionsBuilder<StockTraderContext>()
                              .UseInMemoryDatabase(Guid.NewGuid().ToString())
                              .Options;
             context = new StockTraderContext(options);

            context.AddRange(new User
            {
                UserId=1,
                UserEmail="kingso@gmail.com",
                UserPassword="kinso"
            },
            new User
            {
                UserId = 2,
                UserEmail = "kingso2@gmail.com",
                UserPassword = "kinso2"
            }
            );

            context.SaveChanges();

            return context;
        }

        [Fact(DisplayName = "Method should return a registered user")]
        public void Authenticate_User()
        {
            StockTraderContext context = GetContextWithData();
            Mock<TransactionService> transactionServiceMock = new Mock<TransactionService>(context);
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
