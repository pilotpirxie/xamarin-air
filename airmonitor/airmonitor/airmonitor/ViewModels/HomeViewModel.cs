using System.Windows.Input;
using Xamarin.Forms;

namespace airmonitor.ViewModels
{
    public class HomeViewModel
    {
        INavigation navigation;

        public HomeViewModel(INavigation navigation)
        {
            this.navigation = navigation;
        }

        ICommand _cmnd;

        public ICommand GoToDetailsCommand()
        {
            if (_cmnd != null)
            {
                return _cmnd;
                  
            }
            else
            {
                this._cmnd = new Command(OnGoToDetails);
                return _cmnd;
            }
        }

        private void OnGoToDetails()
        {
            navigation.PushAsync(new DetailsPage());
        }
    }
}
