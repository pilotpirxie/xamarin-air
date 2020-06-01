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

namespace airmonitor.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;

        private ICommand _goToDetailsCommand;

        private bool _isBusy;

        private List<Measurement> _items;

        public HomeViewModel(INavigation navigation)
        {
            _navigation = navigation;
            Initialize();
        }

        public ICommand GoToDetailsCommand => _goToDetailsCommand ?? (_goToDetailsCommand = new Command<Measurement>(OnGoToDetails));

        public List<Measurement> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private async Task Initialize()
        {
            IsBusy = true;
            Console.Write("Starting...");
            Location location = await GetLocation();
            // DatabaseHelper helper = new DatabaseHelper();
            // MeasurementEntity me = helper.Select();

            IEnumerable<Installation> installations = await GetInstallations(location, maxResults: 2);
            IEnumerable<Measurement> data = await GetMeasurementsForInstallations(installations);
            Items = new List<Measurement>(data);
            IsBusy = false;

            try
            {


                /**
                await helper.InsertAsync(data); 
                if (me != null && DateTime.Now.Subtract(me.DateTime).TotalMinutes < 60)
                {
                    Items = new List<Measurement>(me.Measurement);
                }
                else
                {
                    
                    Items = new List<Measurement>(data);
                }
                **/
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured");
            }
        }

        private void OnGoToDetails(Measurement item)
        {
            _navigation.PushAsync(new DetailsPage(item));
        }

        private async Task<IEnumerable<Installation>> GetInstallations(Location location, double maxDistanceInKm = 3,
         int maxResults = -1)
        {
            if (location == null)
            {
                Debug.WriteLine("No location data.");
                return null;
            }

            string query = GetQuery(new Dictionary<string, object> {
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
            string url = GetAirlyApiUrl(App.AirlyApiInstallationUrl, query);

            IEnumerable<Installation> response = await GetHttpResponseAsync<IEnumerable<Installation>>(url);
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

            List<Measurement> measurements = new List<Measurement>();

            foreach (Installation installation in installations)
            {
                string query = GetQuery(new Dictionary<string, object> {{
                    "installationId", installation.Id
                }});
            
                string url = GetAirlyApiUrl(App.AirlyApiMeasurementUrl, query);

                Measurement response = await GetHttpResponseAsync<Measurement>(url);

                if (response != null)
                {
                    response.Installation = installation;
                    response.CurrentDisplayValue = (int)Math.Round(response.Current?.Indexes?.FirstOrDefault()?.Value ?? 0);
                    measurements.Add(response);
                }
            }

            return measurements;
        }

        private static string GetQuery(IDictionary<string, object> args)
        {
            if (args == null)
            {
                return null;
            }

            System.Collections.Specialized.NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);

            foreach (var arg in args)
            {
                if (arg.Value is double number)
                {
                    query[arg.Key] = number.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    query[arg.Key] = arg.Value?.ToString();
                }
            }

            return query.ToString();
        }

        private static string GetAirlyApiUrl(string path, string query)
        {
            UriBuilder builder = new UriBuilder(App.AirlyApiUrl);
            builder.Port = -1;
            builder.Path += path;
            builder.Query = query;
            string url = builder.ToString();

            return url;
        }

        private static HttpClient GetHttpClient()
        {
            HttpClient client = new HttpClient
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
                HttpClient client = GetHttpClient();
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.Headers.TryGetValues("X-RateLimit-Limit-day", out
                  var dayLimit) &&
                 response.Headers.TryGetValues("X-RateLimit-Remaining-day", out
                  var dayLimitRemaining))
                    Debug.WriteLine($"Day limit: {dayLimit?.FirstOrDefault()}, remaining: {dayLimitRemaining?.FirstOrDefault()}");

                switch ((int)response.StatusCode)
                {
                    case 200:
                        string content = await response.Content.ReadAsStringAsync();
                        T result = JsonConvert.DeserializeObject<T>(content);
                        return result;
                    case 429:
                        Debug.WriteLine("Too many requests");
                        break;
                    default:
                        string errorContent = await response.Content.ReadAsStringAsync();
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
                Location location = await Geolocation.GetLastKnownLocationAsync();

                if (location == null)
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    location = await Geolocation.GetLocationAsync(request);
                }

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");
                }

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