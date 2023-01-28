using Newtonsoft.Json.Linq;
using RestSharp;

namespace UsersAPI.Services
{
    public class ExchangeRateService
    {
        private static JToken _exchangeRates;

        public double GetCurrencyExchangeRate(string currency)
        {
            if (currency == null)
            {
                return 1;
            }

            ReloadExchangeRates();

            return _exchangeRates[currency].Value<double>();
        }

        private static void ReloadExchangeRates()
        {
            if(_exchangeRates != null )
            {
                return;
            }

            var client = new RestClient("https://api.apilayer.com/fixer/latest");

            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddHeader("apikey", "[API-KEY]");

            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);

            var obj = JObject.Parse(response.Content);
            _exchangeRates = obj["rates"];
           
        }
    }
}
