using Microsoft.AspNetCore.Mvc;

namespace AccountingService.ErorHanding
{
    public class Error : Exception
    {
       public readonly Dictionary<int, string> _errorCodes;

        public Error()
        {
            _errorCodes = new Dictionary<int, string>
        {
            { 100, "Invalid argument" },
            { 101, "File not found" },
            { 102, "Permission denied" },
            {404,"Not Found" },
            {500, "Internal Server Error"},
        };

        }

        public void HandleError(int errorCode)
        {
            if (_errorCodes.ContainsKey(errorCode))
            {
                Console.WriteLine(_errorCodes[errorCode]);
            }
            else
            {
                Console.WriteLine("Unknown error");
            }
        }

       
    }
}
