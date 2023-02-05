namespace AccountingService
{
    public class Error
    {
        public string Message { get; set; }
        public Code Code { get; set; }
    }
    public enum Code
    {
        Unknown,
        Validation,      
    }
}
