using Newtonsoft.Json.Linq;
using RestSharp;

namespace AccountingService.Services
{
    public class ExchangeRateService
    {
        private static JToken _exchangeRates;
        private static DateTime _lastUpdate;
        public virtual TimeSpan UpdateInterval => TimeSpan.FromDays(1);
        private readonly IConfiguration _configuration;
        public ExchangeRateService(IConfiguration config)
        {
            _configuration = config;
        }
        public double GetCurrencyExchangeRate(string currency)
        {
            if (currency == null || _exchangeRates == null || _exchangeRates.SelectToken(currency) == null) 
            {
                return 1;
            }
            ReloadExchangeRates();
            return _exchangeRates[currency].Value<double>();
        }
        public   void ReloadExchangeRates()
        {
            if (_exchangeRates != null && (DateTime.Now - _lastUpdate) < UpdateInterval)
            {
                return;
            }
            var apikey = _configuration["AppSettings:ApiKey"];
            if (string.IsNullOrWhiteSpace(apikey))
            {
                throw new ArgumentException("Invalid API key", nameof(apikey));
            }
            var client = new RestClient("https://api.apilayer.com/fixer/latest");
            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddHeader("apikey", apikey);

            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            var obj = JObject.Parse(response.Content);
            _exchangeRates = obj["rates"];
            _lastUpdate= DateTime.Now;  
        }
    }
}
