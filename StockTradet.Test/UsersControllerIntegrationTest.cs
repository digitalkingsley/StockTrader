using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using StockTrader;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using StockTrader.StockTrader_Model;
using FluentAssertions;


namespace StockTradet.Test
{
    public class UsersControllerIntegrationTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public UsersControllerIntegrationTest()
        {
            //Setup the Test Server and Client
            _server = new TestServer(new WebHostBuilder()
                                     .UseStartup<Startup>());
            _client = _server.CreateClient();
        }   

        [Fact(DisplayName ="It should return 'success'")]
        public async Task Register_User()
        {          
            var newUser = new User
            {
                UserName = "kinsola",
                UserEmail = "kinsointtester@gmail.com",
                UserPassword="kinsola"
            };
            var content = JsonConvert.SerializeObject(newUser);
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");
       
            //As this is an Integration test which talks to an actual Database, you'll need to ensure that this user
            //doesn't already exist in the database, otherwise the test will fail.
            var response = await _client.PostAsync("/api/v1/users/register", stringContent);
            
            var responseString = await response.Content.ReadAsStringAsync();

            var registeredUser = JsonConvert.DeserializeObject<User>(responseString);
            registeredUser.UserEmail.Should().Be("kinsointtester@gmail.com");
        }
    }
}
