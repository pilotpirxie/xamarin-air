using System.Windows.Input;
using Xamarin.Forms;

namespace airmonitor.ViewModels
{
    public class HomeViewModel
    {
        private ICommand _cmnd;
        private readonly INavigation navigation;

        public HomeViewModel(INavigation navigation)
        {
            this.navigation = navigation;
        }

        public ICommand GoToDetailsCommand()
        {
            if (_cmnd != null)
            {
                return _cmnd;
            }

            _cmnd = new Command(OnGoToDetails);
            return _cmnd;
        }

        private void OnGoToDetails()
        {
            navigation.PushAsync(new DetailsPage());
        }
    }
}