using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RestApi_Main;
using RestApi_Main.Models;

namespace RestApi_Main.Controllers
{
    /// <summary>
    /// Контроллер сервиса
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : Controller
    {
        private readonly DataContext _context;
        private readonly ILogger<ApplicationController> _logger;

        public ApplicationController(DataContext context, ILogger<ApplicationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Принимает заявку в формате json. 
        /// Возвращает уникальный номер и Id принятой заявки в формате json
        /// </summary>
        /// <param name="obj">Данные по заявке в формате Json</param> 
        /// <returns>Возвращает уникальный номер и Id принятой заявки в формате json</returns>
        [HttpPost("create")]
        //public JsonResult Create(Object obj)
        public async Task<JsonResult> Create(Object obj)
        {
            JObject json = JObject.Parse(obj.ToString());

            string applicationNum = (string) json["ApplicationNum"];
            int applicationId = AddJsonDB(json);


            //The data that needs to be sent. Any object works.
            var pocoObject = new
            {
                Name = "John Doe",
                Occupation = "gardener"
            };

            //Converting the object to a json string. NOTE: Make sure the object doesn't contain circular references.
            //string jsonSerialized = JsonConvert.SerializeObject(json);

            //Needed to setup the body of the request
            //StringContent data = new StringContent(jsonSerialized, Encoding.UTF8, "application/json");
            StringContent data = new StringContent(obj.ToString(), Encoding.UTF8, "application/json");

            //The url to post to.
            var url = "http://localhost:3000/api/scoring/evaluate";
            var client = new HttpClient();

            //Pass in the full URL and the json string content
            var response = await client.PostAsync(url, data);

            //It would be better to make sure this request actually made it through
            string result = await response.Content.ReadAsStringAsync();

            json = JObject.Parse(result);
            _logger.LogInformation(">>>> ScoringStatus : " + json["scoringStatus"]);

            //close out the client
            client.Dispose();            

            return Json( new
            { 
                Id = applicationId,
                ApplicationNum = applicationNum,
                ScoringStatus = json["scoringStatus"]
            });
        }

        /// <summary>
        /// Возвращает результат рассмотрения заявки
        /// </summary>
        /// <param name="numApplication">Номер заявки</param>   
        /// <returns>Результат рассмотрения заявки в формате json</returns>
        [HttpGet("status")]
        public JsonResult Status(string numApplication)
        {
            if (_context.CreditApplications.Any(o => o.ApplicationNum == numApplication))
            {
                var clientId = _context.CreditApplications.Where(o => o.ApplicationNum == numApplication).First().ApplicantId;
                var creditId = _context.CreditApplications.Where(o => o.ApplicationNum == numApplication).First().RequestedCreditId;

                return new JsonResult(new 
                {
                    Id = _context.CreditApplications.Where(o => o.ApplicationNum == numApplication).First().Id,
                    ApplicationNum = numApplication,
                    ApplicationDate = _context.CreditApplications.Where(o => o.ApplicationNum == numApplication).First().ApplicationDate,
                    BranchBank = _context.CreditApplications.Where(o => o.ApplicationNum == numApplication).First().BranchBank,
                    BranchBankAddr = _context.CreditApplications.Where(o => o.ApplicationNum == numApplication).First().BranchBankAddr,
                    CreditManagerId = _context.CreditApplications.Where(o => o.ApplicationNum == numApplication).First().CreditManagerId,

                    Applicant = new
                    {
                        FirstName = _context.Clients.Where(o => o.UserId == clientId).First().FirstName,
                        MiddleName = _context.Clients.Where(o => o.UserId == clientId).First().MiddleName,
                        LastName = _context.Clients.Where(o => o.UserId == clientId).First().LastName
                    },

                    RequestedCredit = new 
                    {
                        CreditType = creditId,
                        RequestedAmount = _context.CreditsType.Where(o => o.CreditType == creditId).First().RequestedAmount,
                        RequestedCurrency = _context.CreditsType.Where(o => o.CreditType == creditId).First().RequestedCurrency
                    },

                    ScoringStatus = _context.CreditApplications.Where(o => o.ApplicationNum == numApplication).First().ScoringStatus,
                    ScoringDate = _context.CreditApplications.Where(o => o.ApplicationNum == numApplication).First().ScoringDate
                });
            }
            else
            {
                return Json($"Application {numApplication} not found");
            }
        }

        /// <summary>
        /// Вносит заявку в БД
        /// </summary>
        /// <param name="json"> JSon файл с данными заявки </param>   
        /// <returns>Уникальный Id заявления </returns>
        public int AddJsonDB(JObject json)
        {
            #region Проверка наличия заявителя в таблице Clients

            JObject jsonApplicant = (JObject)json["Applicant"];
            Clients applicant = new Clients 
            {
                FirstName       = (string) jsonApplicant["FirstName"],
                MiddleName      = (string) jsonApplicant["MiddleName"],
                LastName        = (string) jsonApplicant["LastName"],
                DateBirth       = (DateTime) jsonApplicant["DateBirth"],
                CityBirth       = (string) jsonApplicant["CityBirth"],
                AddressBirth    = (string) jsonApplicant["AddressBirth"],
                AddressCurrent  = (string) jsonApplicant["AddressCurrent"],
                INN             = (int) jsonApplicant["INN"],
                SNILS           = (int) jsonApplicant["SNILS"],
                PassportNum     = (string) jsonApplicant["PassportNum"]
            };

            if (_context.Clients.Any(o => o.INN == applicant.INN))
            {
                // такой клиент уже есть
                applicant.UserId = _context.Clients.Where(o => o.INN == applicant.INN).First().UserId;
            }
            else
            {
                // внести нового клиента
                _context.Clients.Add(applicant);
                _context.SaveChanges();
            }
            #endregion 

            #region Проверка наличия кредитного менеджера в таблице CreditManagers
            CreditManagers creditManager = new CreditManagers
            {
                CreditManagerId = (int)json["CreditManagerId"],
                FirstName       = "",
                MiddleName      = "",
                LastName        = ""
            };

            if (!_context.CreditManagers.Any(o => o.CreditManagerId == creditManager.CreditManagerId))
            {
                // внести нового кредитного менеджера
                _context.CreditManagers.Add(creditManager);
                _context.SaveChanges();
            }
            #endregion 

            #region Проверка наличия типа кредита в таблице CreditsType
            JObject jsonCreditsType = (JObject)json["RequestedCredit"];
            CreditsType сreditType = new CreditsType
            {
                CreditType          = (int) jsonCreditsType["CreditType"],
                RequestedAmount     = (int)jsonCreditsType["RequestedAmount"],
                RequestedCurrency   = (string)jsonCreditsType["RequestedCurrency"],
                AnnualSalary        = (int)jsonCreditsType["AnnualSalary"],
                MonthlySalary       = (int)jsonCreditsType["MonthlySalary"],
                CompanyName         = (string)jsonCreditsType["CompanyName"],
                Comment             = (string)jsonCreditsType["Comment"]
            };

            if (!_context.CreditsType.Any(o => o.CreditType == сreditType.CreditType))
            {
                // внести новый тип кредита
                _context.CreditsType.Add(сreditType);
                _context.SaveChanges();
            }
            #endregion 

            CreditApplications сreditApplication = new CreditApplications
            {
                ApplicationNum      = (string)json["ApplicationNum"],
                ApplicationDate     = (DateTime)json["ApplicationDate"],
                BranchBank          = (string)json["BranchBank"],
                BranchBankAddr      = (string)json["BranchBankAddr"],
                CreditManagerId     = creditManager.CreditManagerId,
                ApplicantId         = applicant.UserId,
                RequestedCreditId   = сreditType.CreditType,
                ScoringStatus       = false,
                ScoringDate         = DateTime.Now
            };

            if (_context.CreditApplications.Any(o => o.ApplicationNum == сreditApplication.ApplicationNum))
            {
                // заявка уже существует
                сreditApplication.Id = _context.CreditApplications.Where(o => o.ApplicationNum == сreditApplication.ApplicationNum).First().Id;
            }
            else
            {
                // создать новую заявку
                _context.CreditApplications.Add(сreditApplication);
                _context.SaveChanges();
            }

            return сreditApplication.Id;
        }        
    }
}
