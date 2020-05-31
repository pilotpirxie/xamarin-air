using airmonitor.Views;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace airmonitor
{
    public partial class App : Application
    {
        public static string AirlyApiKey { get; private set; }
        public static string AirlyApiUrl { get; private set; }
        public static string AirlyApiMeasurementUrl { get; private set; }
        public static string AirlyApiInstallationUrl { get; private set; }

        public App()
        {
            InitializeComponent();

            InitializeApp();
        }

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
                    var dynamicJson = JObject.Parse(json);

                    AirlyApiKey = dynamicJson["AirlyApiKey"].Value<string>();
                    AirlyApiUrl = dynamicJson["AirlyApiUrl"].Value<string>();
                    AirlyApiMeasurementUrl = dynamicJson["AirlyApiMeasurementUrl"].Value<string>();
                    AirlyApiInstallationUrl = dynamicJson["AirlyApiInstallationUrl"].Value<string>();
                }
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
