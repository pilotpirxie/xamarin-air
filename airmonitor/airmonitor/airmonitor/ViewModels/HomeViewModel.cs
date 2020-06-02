using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using airmonitor.Models;
using airmonitor.Views;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace airmonitor.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly DatabaseHelper _databaseHelper;
        private readonly INavigation _navigation;

        private ICommand _goToDetailsCommand;

        private ICommand _infoWindowClickedCommand;

        private bool _isBusy;

        private List<Measurement> _items;

        private List<MapLocation> _locations;

        private ICommand _refreshListCommand;

        public HomeViewModel(INavigation navigation)
        {
            _navigation = navigation;
            _databaseHelper = new DatabaseHelper();
            Initialize();
        }

        public ICommand GoToDetailsCommand =>
            _goToDetailsCommand ?? (_goToDetailsCommand = new Command<Measurement>(OnGoToDetails));

        public ICommand InfoWindowClickedCommand => _infoWindowClickedCommand ??
                                                    (_infoWindowClickedCommand =
                                                        new Command<string>(OnInfoWindowClickedCommand));

        public ICommand RefreshList => _refreshListCommand ?? (_refreshListCommand = new Command(OnRefresh));


        public List<Measurement> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public List<MapLocation> Locations
        {
            get => _locations;
            set => SetProperty(ref _locations, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private async Task Initialize()
        {
            IsBusy = true;
            try
            {
                var me = _databaseHelper.Select();
                if (me != null && DateTime.Now.Subtract(me.DateTime).TotalMinutes < 60)
                    Items = JsonConvert.DeserializeObject<List<Measurement>>(me.Measurement);
                else
                    Items = await FetchData();

                Locations = Items.Select(i => new MapLocation
                {
                    Address = i.Installation.Address.Description,
                    Description = "CAQI: " + i.CurrentDisplayValue,
                    Position = new Position(i.Installation.Location.Latitude, i.Installation.Location.Longitude)
                }).ToList();

                IsBusy = false;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error occured {e.Message}");
                throw e;
            }
        }

        private async Task<List<Measurement>> FetchData()
        {
            var location = await GetLocation();
            var installations = await GetInstallations(location, maxResults: 5);
            var data = await GetMeasurementsForInstallations(installations);
            var itemData = new List<Measurement>(data);
            _databaseHelper.Truncate();
            _databaseHelper.Insert(itemData);
            return itemData;
        }

        private void OnGoToDetails(Measurement item)
        {
            _navigation.PushAsync(new DetailsPage(item));
        }

        private void OnInfoWindowClickedCommand(string address)
        {
            var item = Items.First(i => i.Installation.Address.Description.Equals(address));

            _navigation.PushAsync(new DetailsPage(item));
        }

        private void OnRefresh()
        {
            IsBusy = true;
            OnRefreshAsync();
            IsBusy = false;
        }

        private async Task OnRefreshAsync()
        {
            await FetchData();
        }

        private async Task<IEnumerable<Installation>> GetInstallations(Location location, double maxDistanceInKm = 5,
            int maxResults = -1)
        {
            if (location == null)
            {
                Debug.WriteLine("No location data.");
                return null;
            }

            var query = GetQuery(new Dictionary<string, object>
            {
                {
                    "lat",
                    location.Latitude
                },
                {
                    "lng",
                    location.Longitude
                },
                {
                    "maxDistanceKM",
                    maxDistanceInKm
                },
                {
                    "maxResults",
                    maxResults
                }
            });
            var url = GetAirlyApiUrl(App.AirlyApiInstallationUrl, query);

            var response = await GetHttpResponseAsync<IEnumerable<Installation>>(url);
            return response;
        }

        private async Task<IEnumerable<Measurement>> GetMeasurementsForInstallations(
            IEnumerable<Installation> installations)
        {
            if (installations == null)
            {
                Debug.WriteLine("No installations data.");
                return null;
            }

            var measurements = new List<Measurement>();

            foreach (var installation in installations)
            {
                var query = GetQuery(new Dictionary<string, object>
                {
                    {
                        "installationId", installation.Id
                    }
                });

                var url = GetAirlyApiUrl(App.AirlyApiMeasurementUrl, query);

                var response = await GetHttpResponseAsync<Measurement>(url);

                if (response != null)
                {
                    response.Installation = installation;
                    response.CurrentDisplayValue =
                        (int) Math.Round(response.Current?.Indexes?.FirstOrDefault()?.Value ?? 0);
                    measurements.Add(response);
                }
            }

            return measurements;
        }

        private static string GetQuery(IDictionary<string, object> args)
        {
            if (args == null) return null;

            var query = HttpUtility.ParseQueryString(string.Empty);

            foreach (var arg in args)
                if (arg.Value is double number)
                    query[arg.Key] = number.ToString(CultureInfo.InvariantCulture);
                else
                    query[arg.Key] = arg.Value?.ToString();

            return query.ToString();
        }

        private static string GetAirlyApiUrl(string path, string query)
        {
            var builder = new UriBuilder(App.AirlyApiUrl);
            builder.Port = -1;
            builder.Path += path;
            builder.Query = query;
            var url = builder.ToString();

            return url;
        }

        private static HttpClient GetHttpClient()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(App.AirlyApiUrl)
            };

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            client.DefaultRequestHeaders.Add("Accept-Language", "pl");
            client.DefaultRequestHeaders.Add("apikey", App.AirlyApiKey);
            return client;
        }

        private static async Task<T> GetHttpResponseAsync<T>(string url)
        {
            try
            {
                var client = GetHttpClient();
                var response = await client.GetAsync(url);

                if (response.Headers.TryGetValues("X-RateLimit-Limit-day", out
                        var dayLimit) &&
                    response.Headers.TryGetValues("X-RateLimit-Remaining-day", out
                        var dayLimitRemaining))
                    Debug.WriteLine(
                        $"Day limit: {dayLimit?.FirstOrDefault()}, remaining: {dayLimitRemaining?.FirstOrDefault()}");

                switch ((int) response.StatusCode)
                {
                    case 200:
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(content);
                        return result;
                    case 429:
                        Debug.WriteLine("Too many requests");
                        break;
                    default:
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Response error: {errorContent}");
                        return default;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return default;
        }

        private async Task<Location> GetLocation()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location == null)
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    location = await Geolocation.GetLocationAsync(request);
                }

                if (location != null)
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");

                return location;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return null;
        }
    }
}