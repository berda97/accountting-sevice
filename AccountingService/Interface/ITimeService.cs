namespace AccountingService.Interface
{
    public interface ITimeService
    {
        int Minutes { get; }
        DateTime Now { get; }
    }
}
