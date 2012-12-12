using System;
using AntaresShell.BaseClasses;

namespace Repository.MODELs
{
    public class DayWeatherModel : BindableBase
    {
        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        private double? _averageTempuratureC;
        public double? AverageTempuratureC
        {
            get { return _averageTempuratureC; }
            set { SetProperty(ref _averageTempuratureC, value); }
        }

        private string _minmaxTempuratureC;
        public string MinmaxTempuratureC
        {
            get { return _minmaxTempuratureC; }
            set { SetProperty(ref _minmaxTempuratureC, value); }
        }

        private double? _averageTempuratureF;
        public double? AverageTempuratureF
        {
            get { return _averageTempuratureF; }
            set { SetProperty(ref _averageTempuratureF, value); }
        }

        private string _weatherCoded;
        public string WeatherCoded
        {
            get { return _weatherCoded; }
            set
            {
                SetProperty(ref _weatherCoded, value);
                //[coverage]:[intensity]:[weather]

                if(string.IsNullOrEmpty(value))
                {
                    return;
                }

                value = value.ToUpperInvariant();
                var codes = value.Split(':');
                switch (codes[2])
                {
                    case "CL":
                        WeatherType = WeatherType.sunny;
                        break;
                    case "FW":
                        WeatherType = WeatherType.barelycloudy;
                        break;
                    case "SC":
                        WeatherType = WeatherType.morecloudyday;
                        break;
                    case "BK":
                    case "OV":
                        WeatherType = WeatherType.mostcloudy;
                        break;

                    case "A":
                        WeatherType = WeatherType.hail;
                        break;

                    case "BD":
                    case "BN":
                    case "F":
                    case "H":
                    case "K":
                    case "L":
                        WeatherType = WeatherType.windy;
                        break;

                    case "T":
                        WeatherType = WeatherType.storm;
                        break;

                    case "ZF":
                    case "ZL":
                    case "IC":
                    case "IF":
                    case "IP":
                    case "FR":
                    case "WM":
                        WeatherType = WeatherType.heavysnow;
                        break;

                    case "S":
                    case "SW":
                    case "SL":
                    case "BS":
                        WeatherType = (codes[1] == "H" || codes[1] == "VH")
                                          ? WeatherType.heavysnow
                                          : WeatherType.lightsnow;
                        break;
                    case "R":
                    case "RS":
                    case "ZR":
                    case "WP":
                    case "RW":
                        WeatherType = (codes[1] == "H" || codes[1] == "VH")
                                         ? WeatherType.heavyrain
                                         : WeatherType.lightrain;
                        break;
     
                }
            }
        }

        private WeatherType _weatherType = WeatherType.notavailable;
        public WeatherType WeatherType
        {
            get { return _weatherType; }
            set { SetProperty(ref _weatherType, value); }
        }
    }

    public enum WeatherType
    {
        notavailable = 0,
        sunny = 1,
        windy = 2,
        storm = 3,
        mostcloudy = 4,
        morecloudyday = 5,
        morecloudy = 6,
        lightsnow = 7,
        lightrain = 8,
        heavysnow = 9,
        heavyrain = 10,
        hail = 11,
        barelycloudy = 12
    }
}
