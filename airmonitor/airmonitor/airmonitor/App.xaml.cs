using System.IO;
using System.Linq;
using System.Reflection;
using airmonitor.Models;
using airmonitor.Views;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace airmonitor
{
    public partial class App : Application
    {
        public static string DatabasePath;

        public App(string databasePath)
        {

            DatabasePath = databasePath;

            InitializeComponent();

            InitializeApp();
        }

        public static string AirlyApiKey { get; private set; }
        public static string AirlyApiUrl { get; private set; }
        public static string AirlyApiMeasurementUrl { get; private set; }
        public static string AirlyApiInstallationUrl { get; private set; }

        private DatabaseHelper _databaseHelper;

        private void InitializeApp()
        {
            LoadConfig();

            _databaseHelper = new DatabaseHelper();

            MainPage = new RootTabbedPage();
        }

        private void OnSleep()
        {
            _databaseHelper.Dispose();
        }

        private void OnResume()
        {
            _databaseHelper.Connect();
        }

        private static void LoadConfig()
        {
            var assembly = Assembly.GetAssembly(typeof(App));
            var resourceNames = assembly.GetManifestResourceNames();
            var configName = resourceNames.FirstOrDefault(s => s.Contains("config.json"));

            using (var stream = assembly.GetManifestResourceStream(configName))
            {
                using (var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    var data = JObject.Parse(json);

                    AirlyApiKey = data["AirlyApiKey"].Value<string>();
                    AirlyApiUrl = data["AirlyApiUrl"].Value<string>();
                    AirlyApiMeasurementUrl = data["AirlyApiMeasurementUrl"].Value<string>();
                    AirlyApiInstallationUrl = data["AirlyApiInstallationUrl"].Value<string>();
                }
            }
        }
    }
}