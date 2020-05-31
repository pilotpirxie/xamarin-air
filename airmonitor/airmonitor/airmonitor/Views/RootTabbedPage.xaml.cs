using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace airmonitor.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RootTabbedPage : TabbedPage
    {
        public RootTabbedPage()
        {
            InitializeComponent();
        }
    }
}