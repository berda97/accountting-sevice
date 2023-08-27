using Microsoft.EntityFrameworkCore;
using AccountingService.Controllers.models;

namespace AccountingService.Data
{
    public class SalaryConversionContext : DbContext  //context - baza
    {
       
        public DbSet<User> User { get; set; }
        public DbSet<SystemUser> SystemUser { get; set; }
        public SalaryConversionContext(DbContextOptions<SalaryConversionContext> options ) : base(options)
        {
            
        }
    }
}
