using airmonitor.Models;
using airmonitor.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace airmonitor.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        private HomeViewModel ViewModel => BindingContext as HomeViewModel;

        public HomePage()
        {
            InitializeComponent();

            BindingContext = new HomeViewModel(Navigation);
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ViewModel.GoToDetailsCommand.Execute(e.Item as Measurement);
        }
    }
}