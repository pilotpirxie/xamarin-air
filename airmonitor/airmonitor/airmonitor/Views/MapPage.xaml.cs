using airmonitor.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace airmonitor.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent();

            BindingContext = new HomeViewModel(Navigation);
        }

        private HomeViewModel ViewModel => BindingContext as HomeViewModel;

        private void Pin_InfoWindowClicked(object sender, PinClickedEventArgs e)
        {
            ViewModel.InfoWindowClickedCommand.Execute((sender as Pin).Address);
        }
    }
}