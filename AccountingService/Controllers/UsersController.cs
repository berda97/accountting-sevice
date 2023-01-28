using Microsoft.AspNetCore.Mvc;
using AccountingService.Controllers.models;
using AccountingService.Data;
using AccountingService.Services;

namespace AccountingService.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private SalaryConversionContext salaryConversionContext;
        private ExchangeRateService exchangeRateService;

        public UsersController(SalaryConversionContext context) : base()
        {
            salaryConversionContext = context;
            exchangeRateService = new ExchangeRateService();
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = salaryConversionContext.User.ToArray(); // izvuci sve user iz tabele(user) 
            return Ok(new
            {
                Count = users.Length,
                Users = users,
            });

        }


        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = salaryConversionContext.User.Where(u => u.ID == id).SingleOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);

        }


        [HttpPost]
        public IActionResult CreateUser([FromBody] User request)
        {
            salaryConversionContext.User.Add(request);
            int changes = salaryConversionContext.SaveChanges();
            if (changes == 0)
            {
                return BadGateway();
            }
            return Ok();
        }

        [HttpPut]
        public IActionResult UpdateUser([FromBody] User request)
        {
            salaryConversionContext.User.Update(request);
            int changes = salaryConversionContext.SaveChanges();
            if (changes == 0)
            {
                return BadGateway();
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = salaryConversionContext.User.Where(u => u.ID == id).SingleOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            salaryConversionContext.User.Remove(user);
            int changes = salaryConversionContext.SaveChanges();

            if (changes == 0)
            {
                return BadGateway();
            }
            return Ok();
        }

        [HttpGet("{id}/salary")]
        public IActionResult GetUserSalary(int id, [FromQuery(Name ="currency")] string currency)
        {
            int salary = salaryConversionContext.User
                .Where(u => u.ID == id)
                .Select(u => u.Salary)
                .SingleOrDefault();

            if (salary == 0)
            {
                return NotFound();
            }

            double exchangeRate = exchangeRateService.GetCurrencyExchangeRate(currency);

            return Ok(new
            {
                Value = salary * exchangeRate,
                Currency = currency
            });
        }
        


        private IActionResult BadGateway()
        {
            return StatusCode(StatusCodes.Status502BadGateway);
        }
    }
}


