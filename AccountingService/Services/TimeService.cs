using AccountingService.Interface;

namespace AccountingService.Services
{
    public class TimeService : ITimeService
    {
        public int Minutes { get; }
        public DateTime Now => DateTime.UtcNow;
        public TimeService(int minutes)
        {
            Minutes = minutes;
        }
    }
}
