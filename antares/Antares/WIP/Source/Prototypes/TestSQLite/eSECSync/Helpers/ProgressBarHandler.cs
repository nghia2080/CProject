using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace eSECSync.Helpers
{
    public class ProgressBarHandler
    {
        private ProgressBar _progressBar;
        public ProgressBar ProgressBar
        {
            get { return _progressBar; }
            set 
            {
                // assign 1 time.
                if(_progressBar!=null)
                {
                    return;
                }

                _progressBar = value;
                _progressBar.Visibility = Visibility.Collapsed;
                _progressBar.ValueChanged += _progressBar_ValueChanged;
            }
        }

        private async void _progressBar_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if(e.NewValue >= 100.0)
            {
                await Task.Delay(500);
                _progressBar.Value = 0.0;
            }
            else if(e.NewValue == 0.0)
            {
                _progressBar.Visibility = Visibility.Collapsed;
            }
            else 
            {
                _progressBar.Visibility = Visibility.Visible;
            }
        }

        public static ProgressBarHandler Instance
        {
            get
            {
                return Nested._instance;
            }
        }

        private class Nested
        {
            internal static readonly ProgressBarHandler _instance = new ProgressBarHandler();
        }
    }
}
