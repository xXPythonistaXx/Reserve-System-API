using Microsoft.AspNetCore.Mvc;

namespace Reserve_System_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AvailbilityReserveController : Controller
    {
        private readonly HttpClient _httpClient;

        public AvailbilityReserveController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ship>>> Get([FromQuery] string origin, [FromQuery] string destiny, [FromQuery] DateTime date)
        {
            try
            {
                var golFlights = await GetGolFlights(origin, destiny, date);
                var latamFlights = await GetLatamFlights(origin, destiny, date);

                var result = CombineTranslateFlights(golFlights, latamFlights);

                result = result.OrderBy(f => f.FarePrice).ThenBy(f => f.DepartureDate).ToList();
                return Ok(result);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<List<Ship>> GetGolFlights(string origin, string destiny, DateTime date)
        {
            var golApiUrl = $"https://dev.reserve.com.br/airapi/gol/getavailability?origin={origin}&destination={destiny}&date={date:yyyy-MM-dd}";
            var response = await _httpClient.GetFromJsonAsync<List<Ship>>(golApiUrl);
            return response ?? new List<Ship>();
        }
        private async Task<List<Ship>> GetLatamFlights(string origin, string destiny, DateTime date)
        {
            var golApiUrl = $"https://dev.reserve.com.br/airapi/latam/flights?departureCity={origin}&arrivalCity={destiny}&departureDate={date:yyyy-MM-dd}";
            var response = await _httpClient.GetFromJsonAsync<List<Ship>>(golApiUrl);
            return response ?? new List<Ship>();
        }
        private List<Ship> CombineTranslateFlights(List<Ship> golFlights, List<Ship> latamFlights)
        {

            return golFlights.Concat(latamFlights).ToList();
        }
        public class Ship

        {
            public string FlightNumber { get; set; }
            public string Carrier { get; set; }
            public DateTime DepartureDate { get; set; }
            public DateTime ArrivalDate { get; set; }
            public string OriginAirport { get; set; }
            public string DestinationAirport { get; }
            public decimal FarePrice { get; }
        }
    }
}
