using System.Windows.Input;
using Xamarin.Forms;

namespace airmonitor.ViewModels
{
    class HomeViewModel
    {
        INavigation navigation;

        public HomeViewModel(INavigation navigation)
        {
            this.navigation = navigation;
        }

        private ICommand _goToDetailsCommand;
        public ICommand GoToDetailsCommand => _goToDetailsCommand ?? (_goToDetailsCommand = new Command(OnGoToDetails));

        private void OnGoToDetails()
        {
            navigation.PushAsync(new DetailsPage());
        }
    }
}
