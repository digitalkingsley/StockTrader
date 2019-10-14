using System;
using Xunit;
using StockTrader.StockTrader_Model;
using StockTrader.Service.DataService;
using Microsoft.EntityFrameworkCore;
using Moq;
using FluentAssertions;
using System.Threading.Tasks;

namespace StockTradet.Test
{
    public class UserServiceTest
    {
        

        private StockTraderContext GetContextWithData()
        {
            var options = new DbContextOptionsBuilder<StockTraderContext>()
                              .UseInMemoryDatabase(Guid.NewGuid().ToString())
                              .Options;
           StockTraderContext context = new StockTraderContext(options);

            context.AddRange(new User
            {
                UserId = 1,
                UserEmail = "kingso@gmail.com",
                UserPassword = "kinso"
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

        [Fact(DisplayName = "It should return success")]
        public void Authenticate_User()
        {
            StockTraderContext context = GetContextWithData();
            Mock<TransactionService> transactionMock = new Mock<TransactionService>(context);
            UserService userService = new UserService(transactionMock.Object);

            //Create a User Object to be authenticated
            var userToAuthenticate = new User
            {
                UserEmail = "kingso2@gmail.com",
                UserName = "kinso2",
                UserPassword = "kinso2"
            };

            var result = userService.Authenticate(userToAuthenticate, context);
            var okResult = result.Should().BeOfType<string>().Subject;
            okResult.Should().Be("success");
        }

        [Fact(DisplayName = "It should return null")]
        public void Authenticate_Unregistered_User()
        {
            StockTraderContext context = GetContextWithData();
            Mock<TransactionService> transactionMock = new Mock<TransactionService>(context);
            UserService userService = new UserService(transactionMock.Object);
                
            //Create a User Object to be authenticated
            var userToAuthenticate = new User
            {
                UserEmail = "kingso8@gmail.com",
                UserName = "kinso2",
                UserPassword = "kinso2"
            };

            var result = userService.Authenticate(userToAuthenticate, context);
            result.Should().Be(null);
        }

        [Fact(DisplayName = "It should return a success message")]
        public async Task Register_New_User()
        {
            StockTraderContext context = GetContextWithData();
            Mock<TransactionService> transactionMock = new Mock<TransactionService>(context);
            UserService userService = new UserService(transactionMock.Object);

            //Create a User Object to be authenticated
            var userToRegister = new User
            {
                UserId=3,
                UserEmail = "kingsothenew@gmail.com",
                UserName = "kinso2new",
                UserPassword = "kinso2newpass"
            };

            var result = await userService.Register(userToRegister, context);
            var okResult = result.Should().BeOfType<string>().Subject;
            okResult.Should().Be("success");
        }

        [Fact(DisplayName = "It should return a 'User Already Exists'")]
        public async void Register_New_User_Return_User_Exists()
        {
            StockTraderContext context = GetContextWithData();
            Mock<TransactionService> transactionMock = new Mock<TransactionService>(context);
            UserService userService = new UserService(transactionMock.Object);

            //Create a User Object to be authenticated
            var userToRegister = new User
            {
                UserId = 3,
                UserEmail = "kingso@gmail.com",
                UserName = "kinso2new",
                UserPassword = "kinso2newpass"
            };

            var result = await userService.Register(userToRegister, context);
            var okResult = result.Should().BeOfType<string>().Subject;
            okResult.Should().Be("User Already Exists");
        }
    }
}
