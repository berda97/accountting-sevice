using AccountingService.Interface;

namespace AccountingService.Services
{
    public class TimeService : ITimeService
    {
        
        public DateTime Now => DateTime.UtcNow;
       
    }
}
