using Newtonsoft.Json.Linq;
using System.Data;

namespace hw03.Services
{
    public class Hw03Service
    {
        private const string WEATHER_URL = "https://api.open-meteo.com/v1/forecast";
        private const string AIRPORT_URL = "http://www.airport-data.com/api/ap_info.json?iata=";
        private const string STOCK_URL = "https://yahoo-finance15.p.rapidapi.com/api/yahoo/qu/quote/";

        private const string RAPIDAPI_KEY = "46f91ba65bmsh857b85549b30ee5p175598jsn6ce0cb199f7b";
        private const string RAPIDAPI_HOST = "yahoo-finance15.p.rapidapi.com";


        private HttpClient _httpClient;

        public Hw03Service()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string?> Get(string? queryAirportTemp, string? queryStockPrice, string? queryEval)
        {
            if (!ContainsExactlyOneQueryParameter(queryAirportTemp, queryStockPrice, queryEval))
            {
                return null;
            }

            if (queryAirportTemp != null)
            {
                return await GetAirportTemperatureAsync(queryAirportTemp);
            }
            else if (queryStockPrice != null)
            {
                return await GetStockPriceAsync(queryStockPrice);
            }

            return GetEvaluatedQuery(queryEval);
        }

        private string? GetEvaluatedQuery(string? queryEval)
        {
            try
            {
                var result = new DataTable().Compute(queryEval, "");
                return Convert.ToString(result);
            }
            catch
            {
                return null;
            }
        }

        private async Task<string?> GetStockPriceAsync(string queryStockPrice)
        {
            try
            {
                var requestUrl = $"{STOCK_URL}{queryStockPrice}";
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Add("x-rapidapi-key", RAPIDAPI_KEY);
                request.Headers.Add("x-rapidapi-host", RAPIDAPI_HOST);

                var response = await _httpClient.SendAsync(request);

                var json = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(json);
                var price = data["body"]?[0]?["regularMarketPrice"]?.Value<string>();

                return price;
            }
            catch
            {
                return null;
            }
        }

        private async Task<string?> GetAirportTemperatureAsync(string queryAirportTemp)
        {
            try
            {
                var airportResponse = await _httpClient.GetStringAsync($"{AIRPORT_URL}{queryAirportTemp}");
                var airport = JObject.Parse(airportResponse);
                var lat = airport["latitude"]?.Value<string>();
                var lon = airport["longitude"]?.Value<string>();

                if (lat == null || lon == null)
                    return null;

                var weatherUrl = $"{WEATHER_URL}?latitude={lat}&longitude={lon}&current_weather=true";
                var weatherResponse = await _httpClient.GetStringAsync(weatherUrl);
                var weather = JObject.Parse(weatherResponse);

                return weather["current_weather"]?["temperature"]?.Value<string>();
            }
            catch
            {
                return null;
            }
        }

        private bool ContainsExactlyOneQueryParameter(string? queryAirportTemp, string? queryStockPrice, string? queryEval)
        {
            int count = 0;
            if (queryAirportTemp != null) count++;
            if (queryStockPrice != null) count++;
            if (queryEval != null) count++;
            return count == 1;
        }
    }
}
