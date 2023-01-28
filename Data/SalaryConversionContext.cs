using Microsoft.EntityFrameworkCore;
using UsersAPI.Controllers.models;

namespace UsersAPI.Data
{
    public class SalaryConversionContext : DbContext  //context - baza
    {
        public DbSet<User> User { get; set; } // User(2) ime tabele u bazim,
        public SalaryConversionContext(DbContextOptions<SalaryConversionContext> options) : base(options)
        {

        }

    }
}
