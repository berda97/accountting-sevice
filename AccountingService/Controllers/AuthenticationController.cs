using AccountingService.Controllers.models;
using AccountingService.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using BC = BCrypt.Net.BCrypt;
using System.Text.RegularExpressions;

namespace AccountingService.Controllers
{
    [EnableCors("CorsOriginPolicy")]
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private SalaryConversionContext salaryConversionContext;
        public AuthenticationController(SalaryConversionContext context) : base()
        {
            salaryConversionContext = context;
        }
            [HttpPost("register")]
        public async Task<ActionResult<SystemUser>> Register(Authentication requst)
        {
            CreatePasswordHash(requst.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = new SystemUser();
            try
            {
                
                var useremail= salaryConversionContext.SystemUser.
                    Where(u => u.Email == requst.Email).SingleOrDefault();
                if (useremail != null)
                {
                    return BadRequest("Email already exists.");
                }

                if (!Regex.IsMatch(requst.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$"))
                {
                    return BadRequest("Password mora da sadrži najmanje 8 karaktera, mala i velika slova, brojeve i specijalne znakove.");
                }
                user.Email = requst.Email;
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                
                    salaryConversionContext.SystemUser.Add(user);
                    await salaryConversionContext.SaveChangesAsync();

                return Ok(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Došlo je do greške pri registraciji: " + ex.Message);
                return StatusCode(500, "Došlo je do greške pri registraciji.");
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
                if (userdata == null )
                {
                    return BadRequest("Pogrešan email.");
                }         
                var verifypassword = VerifyPasswordHash(req.Password, userdata.PasswordHash, userdata.PasswordSalt);
                if (verifypassword)
                {
                    return Ok("uspesno ste se logovali");
                }

                return BadRequest("Pogresan password");
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Došlo je do greške pri prijavljivanju: " + ex.Message);
                return BadRequest("Došlo je do greške pri prijavljivanju.");
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
    }
}
