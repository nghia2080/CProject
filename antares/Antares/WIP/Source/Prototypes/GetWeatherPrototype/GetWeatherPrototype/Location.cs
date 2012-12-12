using GetWeatherPrototype.Common;

namespace GetWeatherPrototype
{
    public class Location : BindableBase
    {
        private double _latitude;
        public double Latitude
        {
            get { return _latitude; }
            set { SetProperty(ref _latitude, value); }
        }

        private double _longtitude;
        public double Longtitude
        {
            get { return _longtitude; }
            set { SetProperty(ref _longtitude, value); }
        }

        private double _accuracy;
        public double Accuracy
        {
            get { return _accuracy; }
            set { SetProperty(ref _accuracy, value); }
        }

        public override string ToString()
        {
            return string.Format("Latitude = {0}, Longtitude = {1}, Accuracy = {2}", _latitude, _longtitude, _accuracy);
        }
    }
}
