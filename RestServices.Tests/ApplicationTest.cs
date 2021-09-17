using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

using RestApi_Main;
using RestApi_Main.Controllers;

namespace RestServices.Tests
{
    public class ApplicationTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        private const string url = "http://localhost:2000/api/application/create";

        public ApplicationTest()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder()
               .UseStartup<Startup>());
            _client = _server.CreateClient();
        }        
        
        [Fact]
        public async Task CabSendRequest()
        {
            var testData = new
            {
                ApplicationNum = "КД2135555",
                ApplicationDate = DateTime.Now,
                BranchBank = "Арбат",
                BranchBankAddr = "ул. Новая, 21",
                CreditManagerId = 678543,
                Applicant = new {
                    FirstName = "Иванов",
                    MiddleName = "Иван",
                    LastName = "Диванович",
                    DateBirth = DateTime.Parse("1974-03-03T00:00:00"),
                    CityBirth = "Москва",
                    AddressBirth = "Место рождения",
                    AddressCurrent = "Прописка",
                    INN = 7747456,
                    SNILS = 12345567,
                    PassportNum = "4508 8345567"
                },
                RequestedCredit = new
                {
                    CreditType = 1,
                    RequestedAmount = 10000000.0,
                    RequestedCurrency = "rur",
                    AnnualSalary = 14400000.0,
                    MonthlySalary = 120000.0,
                    CompanyName = "IBM LLC",
                    Comment = "Комментарий к кредитной заявке"
                }
            };

            var jsSer = JsonConvert.SerializeObject(testData);
            StringContent data = new StringContent(jsSer.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync(url, data);
            response.EnsureSuccessStatusCode();
            
            var responseString = await response.Content.ReadAsStringAsync();
            string result = await response.Content.ReadAsStringAsync();

            var jsonResponse = JObject.Parse(result);

            // Assert            
            Assert.Equal(testData.ApplicationNum, jsonResponse["applicationNum"]);
        }
    }
}
