using AccountingService.Controllers.models;
using AccountingService.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using AccountingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;

namespace AccountingService.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private SalaryConversionContext salaryConversionContext;
        private JwtTokenService jwtTokenSevice;
        private ClaimsService claimsSevice;
        private IConfiguration configuration;
        public AuthenticationController(SalaryConversionContext context, IConfiguration config) : base()
        {
            configuration = config;
            salaryConversionContext = context;
            jwtTokenSevice = new JwtTokenService(configuration);
            claimsSevice = new ClaimsService();
        }
        [HttpPost("register")]
        public async Task<ActionResult<SystemUser>> Register(Authentication request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = new SystemUser();
            try
            {
                const string PasswordRegexPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";
                var useremail = salaryConversionContext.SystemUser.
                    Where(u => u.Email == request.Email).SingleOrDefault();
                if (useremail != null)
                {
                    return BadRequest("Email already exists.");
                }

                if (!Regex.IsMatch(request.Password, PasswordRegexPattern))
                {
                    return BadRequest("Password must contain at least 8 characters, lowercase and uppercase letters, numbers, and special characters.");
                }
                user.Email = request.Email;
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                salaryConversionContext.SystemUser.Add(user);
                await salaryConversionContext.SaveChangesAsync();
                return Ok(user);
            }
            catch (Exception)
            {
                var error = new Error
                {
                    Message = "An error occurred during the registration process",
                    Code = Code.Connection
                };
                return BadRequest(error);
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<SystemUser>> Login(Authentication req)
        {
            var userlogin = new SystemUser();
            try
            {
                var userdata = await salaryConversionContext.SystemUser.
                    FirstOrDefaultAsync(u => u.Email == req.Email);

                if (userdata == null)
                {
                    var emailFailed = new Error
                    {
                        Message = "Wrong email",
                        Code = Code.Unknown
                    };
                    return BadRequest(emailFailed);
                }
                var verifypassword = VerifyPasswordHash(req.Password, userdata.PasswordHash, userdata.PasswordSalt);
                if (verifypassword)
                {
                    var claims = claimsSevice.GetUserClaims(userdata);
                    var generateJwt = jwtTokenSevice.GetToken(claims);
                    return Ok(generateJwt);
                }
                var error = new Error
                {
                    Message = "Your password is incorrect ",
                    Code = Code.PasswordIncorrect
                };
                return BadRequest(error);
            }
            catch (Exception)
            {
                var error = new Error
                {
                    Message = "An error occurred during the login process",
                    Code = Code.Connection
                };
                return BadRequest(error);
            }
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key.ToArray();
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)).ToArray();
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        private IActionResult BadGateway(Error error)
        {
            return StatusCode(StatusCodes.Status502BadGateway, error);
        }
    }
}
