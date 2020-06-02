using airmonitor.Models;
using airmonitor.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace airmonitor.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            BindingContext = new HomeViewModel(Navigation);
        }

        private HomeViewModel ViewModel => BindingContext as HomeViewModel;

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ViewModel.GoToDetailsCommand.Execute(e.Item as Measurement);
        }
    }
}