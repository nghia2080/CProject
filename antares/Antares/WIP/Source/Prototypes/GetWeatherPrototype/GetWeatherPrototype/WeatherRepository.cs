using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Windows.Devices.Geolocation;

namespace GetWeatherPrototype
{
    public sealed class WeatherRepository
    {
        private WeatherRepository()
        {
            _geolocator = new Geolocator();
        }

        private readonly Geolocator _geolocator;
        private CancellationTokenSource _cts;
        private static WeatherRepository _instance;
        public static WeatherRepository Instance
        {
            get { return _instance ?? (_instance = new WeatherRepository()); }
        }

        async public Task<Location> GetCurrentLocation()
        {
            try
            {
                // Get cancellation token
                _cts = new CancellationTokenSource();
                var token = _cts.Token;

                // Carry out the operation
                var pos = await _geolocator.GetGeopositionAsync().AsTask(token);

                return new Location
                    {
                        Latitude = pos.Coordinate.Latitude,
                        Longtitude = pos.Coordinate.Longitude,
                        Accuracy = pos.Coordinate.Accuracy
                    };
            }
            catch (UnauthorizedAccessException)
            {
                return new Location { Latitude = 0.0, Longtitude = 0.0, Accuracy = double.MaxValue };
            }
            catch (TaskCanceledException)
            {
                return new Location { Latitude = 0.0, Longtitude = 0.0, Accuracy = double.MaxValue };
            }
            finally
            {
                _cts = null;
            }

        }

        async public Task<string> GetWeatherInfoString(Location location)
        {
            try
            {
                var httpClient = new HttpClient { MaxResponseContentBufferSize = 256000 };
                // Limit the max buffer size for the response so we don't get overwhelmed

                var response = await httpClient.GetAsync(
                    string.Format("http://api.aerisapi.com/forecasts/?p={0},{1}&days=7&client_id=HBrt0EqYb6LTH3nXvrNie&client_secret=KJsPg1sqmn1jzsm2NlCh41ttqCBtMc553NTfXnEA",
                    location.Latitude, location.Longtitude));
                response.EnsureSuccessStatusCode();

                var responseBodyAsText = await response.Content.ReadAsStringAsync();
                return responseBodyAsText;
            }
            catch (HttpRequestException hre)
            {
                return hre.ToString();
            }
            catch (Exception ex)
            {
                // For debugging
                return ex.ToString();
            }
        }

        public List<DayWeather> GetWeatherInfo(string json)
        {
            var response = JObject.Parse(json);
            var days = (JArray)response["response"][0]["periods"];

            return days.Select(t => new DayWeather
                {
                    Date = (DateTime)t["validTime"],
                    AverageTempuratureC = (double)t["avgTempC"],
                    AverageTempuratureF = (double)t["avgTempF"],
                    WeatherCoded = (string)t["weatherPrimaryCoded"]
                }).ToList();
        }
    }
}
