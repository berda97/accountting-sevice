using Microsoft.AspNetCore.Mvc;
using AccountingService.Controllers.models;
using AccountingService.Data;
using AccountingService.Services;
using AccountingService.ErorHanding;

namespace AccountingService.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private SalaryConversionContext salaryConversionContext;

        private ExchangeRateService exchangeRateService;

        private NetSalaryService netSalaryService;



        public UsersController(SalaryConversionContext context) : base()
        {
            salaryConversionContext = context;
            exchangeRateService = new ExchangeRateService();
            netSalaryService = new NetSalaryService();


        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = salaryConversionContext.User.ToArray(); // izvuci sve user iz tabele(user) 
                return Ok(new
                {
                    Count = users.Length,
                    Users = users,
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return new StatusCodeResult(500);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                var user = salaryConversionContext.User.Where(u => u.ID == id).SingleOrDefault();


                return Ok(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return new StatusCodeResult(500);
            }
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] User request)
        {
            try
            {
                salaryConversionContext.User.Add(request);
                int changes = salaryConversionContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return new StatusCodeResult(500);

            }
        }

        [HttpPut]
        public IActionResult UpdateUser([FromBody] User request)
        {
            try
            {
                salaryConversionContext.User.Update(request);
                int changes = salaryConversionContext.SaveChanges();

                return Ok();
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            {
                try
                {
                    var user = salaryConversionContext.User.Where(u => u.ID == id).SingleOrDefault();
                    if (user == null)
                    {
                        return NotFound();
                    }
                    salaryConversionContext.User.Remove(user);
                    int changes = salaryConversionContext.SaveChanges();


                    return Ok();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return new StatusCodeResult(500);
                }
            }
        }

        [HttpGet("{id}/salary")]

        public IActionResult GetUserSalary(int id, [FromQuery(Name = "currency")] string currency, [FromQuery(Name = "isNetSalary")] bool isNetSalary)
        {
            try
            {
                double salary = salaryConversionContext.User
                    .Where(u => u.ID == id)
                    .Select(u => u.Salary)
                    .SingleOrDefault();



                double exchangeRate = exchangeRateService.GetCurrencyExchangeRate(currency);
                salary = salary * exchangeRate;

                if (isNetSalary)
                {
                    double netSalary = netSalaryService.Calculate(salary);
                    return Ok(new
                    {
                        Value = salary,
                        NetSalary = netSalary,
                        Currency = currency

                    });
                }

                return Ok(new
                {
                    Value = salary,
                    Currency = currency
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return new StatusCodeResult(500);
            }
        }
    }
}



