using AntaresShell.IO;
using Newtonsoft.Json.Linq;
using Repository.MODELs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Repository.Repositories
{
    public sealed class WeatherRepository
    {
        private readonly LocalStorageManager _weatherLocal = new LocalStorageManager("Cache\\Weather.esec");
        private const double Epsilon = 1e-9;

        private WeatherRepository()
        {
            _geolocator = new Geolocator();

        }

        private readonly Geolocator _geolocator;
        private CancellationTokenSource _cts;
        private List<DayWeatherModel> _cache;

        private static WeatherRepository _instance;
        public static WeatherRepository Instance
        {
            get { return _instance ?? (_instance = new WeatherRepository()); }
        }

        async private Task<LocationModel> GetCurrentLocationAsync()
        {
            try
            {
                // Get cancellation token
                _cts = new CancellationTokenSource();
                var token = _cts.Token;

                // Carry out the operation
                var pos = await _geolocator.GetGeopositionAsync().AsTask(token);

                return new LocationModel
                {
                    Latitude = pos.Coordinate.Latitude,
                    Longtitude = pos.Coordinate.Longitude,
                    Accuracy = pos.Coordinate.Accuracy
                };
            }
            catch (UnauthorizedAccessException)
            {
                return new LocationModel { Latitude = 0.0, Longtitude = 0.0, Accuracy = double.MaxValue };
            }
            catch (TaskCanceledException)
            {
                return new LocationModel { Latitude = 0.0, Longtitude = 0.0, Accuracy = double.MaxValue };
            }
            finally
            {
                _cts = null;
            }
        }

        async private Task<string> GetWeatherInfoStringAsync()
        {
            var locationModel = await GetCurrentLocationAsync();
            if (Math.Abs(locationModel.Longtitude - 0) < Epsilon && Math.Abs(locationModel.Latitude - 0) < Epsilon)
                return null;
            try
            {
                var httpClient = new HttpClient { MaxResponseContentBufferSize = 256000 };
                // Limit the max buffer size for the response so we don't get overwhelmed

                var response = await httpClient.GetAsync(
                    string.Format("http://api.aerisapi.com/forecasts/?p={0},{1}&limit=15&client_id=HBrt0EqYb6LTH3nXvrNie&client_secret=KJsPg1sqmn1jzsm2NlCh41ttqCBtMc553NTfXnEA",
                    locationModel.Latitude.ToString().Replace(',', '.'), locationModel.Longtitude.ToString().Replace(',', '.')));
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

        private readonly SemaphoreSlim _sl = new SemaphoreSlim(1);

        async public Task<List<DayWeatherModel>> GetWeatherInfoAsync(LocationModel location = null)
        {
            await _sl.WaitAsync();

            try
            {
                if (_cache != null)
                {
                    return _cache;
                }

                _cache = await _weatherLocal.RestoreAsync<List<DayWeatherModel>>();

                var json = await GetWeatherInfoStringAsync();

                var response = JObject.Parse(json);
                var days = (JArray)response["response"][0]["periods"];

                var localCacheNull = false;

                foreach (var model in days.Select(day => new DayWeatherModel
                    {
                        Date = (DateTime)day["validTime"],
                        AverageTempuratureC = (double)day["avgTempC"],
                        AverageTempuratureF = (double)day["avgTempF"],
                        MinmaxTempuratureC = (string)day["minTempC"] + "° / " + (string)day["maxTempC"],
                        WeatherCoded = (string)day["weatherPrimaryCoded"]
                    }))
                {
                    if (_cache == null)
                    {
                        localCacheNull = true;
                        _cache = new List<DayWeatherModel> { model };
                    }
                    else
                    {
                        if (localCacheNull)
                        {
                            _cache.Add(model);
                        }
                        else
                        {
                            var tempModel = model;
                            var index = _cache.FindIndex(p => (p.Date.Day == tempModel.Date.Day
                                                               && p.Date.Month == tempModel.Date.Month
                                                               && p.Date.Year == tempModel.Date.Year));

                            if (index >= 0)
                            {
                                _cache[index] = model;
                            }
                            else
                            {
                                _cache.Add(model);
                            }
                        }
                    }
                }

                await _weatherLocal.SaveAsync(_cache);

                return _cache;
            }
            catch (Exception)
            {
                return _cache;
            }
            finally
            {
                _sl.Release();
            }
        }

        public async Task<DayWeatherModel> GetWeather(DateTime date, LocationModel location = null)
        {
            if (_cache == null)
            {
                await GetWeatherInfoAsync(location);
                if (_cache == null)
                {
                    return null;
                }
            }

            return _cache.FirstOrDefault(p => (p.Date.Day == date.Day
                                           && p.Date.Month == date.Month
                                           && p.Date.Year == date.Year));
        }

        public void ClearCache()
        {
            _cache = null;
        }
    }
}
