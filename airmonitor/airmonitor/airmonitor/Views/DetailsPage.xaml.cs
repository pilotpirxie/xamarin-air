using System;
using System.ComponentModel;
using airmonitor.Models;
using airmonitor.ViewModels;
using Xamarin.Forms;

namespace airmonitor.Views
{
    [DesignTimeVisible(false)]
    public partial class DetailsPage : ContentPage
    {
        public DetailsPage(Measurement item)
        {
            InitializeComponent();

            DetailsViewModel viewModel = BindingContext as DetailsViewModel;
            viewModel.Item = item;
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("Co to jest CAQI?",
                "Przy pomocy skali CAQI określa się poziom zanieczyszczenia powietrza w odniesieniu do maksymalnego poziomu zanieczyszczenia. Im mniejsza wartość wskaźnika tym czystsze powietrze.",
                "Zamknij");
        }
    }
}