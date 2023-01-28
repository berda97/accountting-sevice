using Microsoft.EntityFrameworkCore;
using AccountingService.Controllers.models;

namespace AccountingService.Data
{
    public class SalaryConversionContext : DbContext  //context - baza
    {
        public DbSet<User> User { get; set; } // User(2) ime tabele u bazim,
        public SalaryConversionContext(DbContextOptions<SalaryConversionContext> options) : base(options)
        {

        }

    }
}
