using System.IO;
using System.Linq;
using System.Reflection;
using airmonitor.Views;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace airmonitor
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            InitializeApp();
        }

        public static string AirlyApiKey { get; private set; }
        public static string AirlyApiUrl { get; private set; }
        public static string AirlyApiMeasurementUrl { get; private set; }
        public static string AirlyApiInstallationUrl { get; private set; }

        private void InitializeApp()
        {
            LoadConfig();

            MainPage = new RootTabbedPage();
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