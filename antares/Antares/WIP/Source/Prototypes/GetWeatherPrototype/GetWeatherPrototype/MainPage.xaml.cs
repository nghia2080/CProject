using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Windows.Data.Json;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GetWeatherPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public Location MyLocation { get; set; }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        async private void BtnGetLocation_OnClick(object sender, RoutedEventArgs e)
        {
            txtLocation.Text = "Getting Data...";
            MyLocation = await WeatherRepository.Instance.GetCurrentLocation();
            txtLocation.Text = MyLocation.ToString();
            txtResponse.Text = "Getting weather data";
            txtResponse.Text = await WeatherRepository.Instance.GetWeatherInfoString(MyLocation);
            //var response = JObject.Parse(txtResponse.Text);
            //var days = (JArray) response["response"][0]["durations"];
            var weatherInfo = WeatherRepository.Instance.GetWeatherInfo(txtResponse.Text);
            //var weatherCodePrimary = 
        }
    }
}
