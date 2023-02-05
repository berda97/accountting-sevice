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
            catch (Exception )
            {
                var error = new Error
                {
                    Message = "Failed to get users",
                    Code = Code.Unknown
                };
                return BadGateway(error);
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                var user = salaryConversionContext.User.Where(u => u.ID == id).SingleOrDefault();
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception )
            {

                var error = new Error
                {
                    Message = "Failed to get user",
                    Code = Code.Unknown
                };
                return BadGateway(error);
            }
        }
        [HttpPost]
        public IActionResult CreateUser([FromBody] User request)
        {
            try
            {
                salaryConversionContext.User.Add(request);
                int changes = salaryConversionContext.SaveChanges();
                if (changes == 0)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch (Exception )
            {
                var error = new Error
                {
                    Message = "Failed to create" ,
                    Code = Code.Unknown
                };
                return BadGateway(error);
            }
        }

        [HttpPut]
        public IActionResult UpdateUser([FromBody] User request)
        {
            try
            {
                salaryConversionContext.User.Update(request);
                int changes = salaryConversionContext.SaveChanges();
                if (changes == 0)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch (Exception )
            {
                var error = new Error
                {
                    Message = "Failed to update",
                    Code = Code.Unknown
                };
                return BadGateway(error);
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
                catch (Exception )
                {
                    var error = new Error
                    {
                        Message = "Failed to delete",
                        Code = Code.Unknown
                    };
                    return BadGateway(error);
                }
            }
            
        }
        [HttpGet("{id}/salary")]

        public IActionResult GetUserSalary(int id, [FromQuery(Name = "currency")] string currency, [FromQuery(Name = "isNetSalary")] bool isNetSalary)
        {
            double salary , exchangeRate;
            try
            {
                 salary = salaryConversionContext.User
                    .Where(u => u.ID == id)
                    .Select(u => u.Salary)
                    .SingleOrDefault();
                if(salary == 0)
                {
                    return NotFound();
                }
                 exchangeRate = exchangeRateService.GetCurrencyExchangeRate(currency);
                
            }
            catch (Exception )
            {
                var error = new Error
                {
                    Message = "Failed to convert",
                    Code = Code.Unknown
                };               
                return BadGateway(error);
            }
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
        private IActionResult BadGateway(Error error)
        {
            return StatusCode(StatusCodes.Status502BadGateway,error);
        }

    }
}



