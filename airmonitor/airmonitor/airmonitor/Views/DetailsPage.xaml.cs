using System;
using Xamarin.Forms;

namespace airmonitor
{
    public partial class DetailsPage : ContentPage
    {
        public DetailsPage()
        {
            InitializeComponent();
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("Co to jest CAQI?",
                "Przy pomocy skali CAQI określa się poziom zanieczyszczenia powietrza w odniesieniu do maksymalnego poziomu zanieczyszczenia. Im mniejsza wartość wskaźnika tym czystsze powietrze.",
                "Zamknij");
        }
    }
}