using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace airmonitor.ViewModels
{
    public class DetailsViewModel : INotifyPropertyChanged
    {
        private string _caqiDescription =
            "Możesz bezpiecznie wyjść z domu bez swojej maski anty-smogowej i nie bać się o swoje zdrowie.";

        private string _caqiTitle = "Świetna jakość!";

        private int _caqiValue = 100;

        private double _humidityValue = 0.95;

        private int _pm10Percent = 135;

        private int _pm10Value = 67;

        private int _pm25Percent = 137;

        private int _pm25Value = 34;

        private int _pressureValue = 1000;

        public int CaqiValue
        {
            get => _caqiValue;
            set => SetProperty(ref _caqiValue, value);
        }

        public string CaqiTitle
        {
            get => _caqiTitle;
            set => SetProperty(ref _caqiTitle, value);
        }

        public string CaqiDescription
        {
            get => _caqiDescription;
            set => SetProperty(ref _caqiDescription, value);
        }

        public int Pm25Value
        {
            get => _pm25Value;
            set => SetProperty(ref _pm25Value, value);
        }

        public int Pm25Percent
        {
            get => _pm25Percent;
            set => SetProperty(ref _pm25Percent, value);
        }

        public int Pm10Value
        {
            get => _pm10Value;
            set => SetProperty(ref _pm10Value, value);
        }

        public int Pm10Percent
        {
            get => _pm10Percent;
            set => SetProperty(ref _pm10Percent, value);
        }

        public double HumidityValue
        {
            get => _humidityValue;
            set => SetProperty(ref _humidityValue, value);
        }

        public int PressureValue
        {
            get => _pressureValue;
            set => SetProperty(ref _pressureValue, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            field = value;

            RaisePropertyChanged(propertyName);

            return true;
        }
    }
}