using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;

namespace AntaresShell.NavigatorProvider
{
    public class Navigator
    {
        private Frame _rootFrame;
        private Frame _rootProjectFrame;


        private Popup _mainPopup;

        public void SetRootFrame(Frame rootFrame)
        {
            _rootFrame = rootFrame;
        }

        public void SetRootProjectFrame(Frame rootProjectFrame)
        {
            _rootProjectFrame = rootProjectFrame;
        }


        public bool NavigateTo(Type pageType)
        {
            HideNotificator();
            BottomAppBar.IsOpen = false;
            TopAppBar.IsOpen = false;
            return _rootFrame.Navigate(pageType);
        }

        public bool NavigateTo(Type pageType, object parameter)
        {
            HideNotificator();
            BottomAppBar.IsOpen = false;
            TopAppBar.IsOpen = false;
            return _rootFrame.Navigate(pageType, parameter);
        }

        public bool NavigateToSubPage(Type pageType)
        {
            return _rootProjectFrame.Navigate(pageType);
        }

        public bool NavigateToSubPage(Type pageType, object parameter)
        {
            return _rootProjectFrame.Navigate(pageType, parameter);
        }


        /// <summary>
        /// Gets value of _instance.
        /// Instance of NavigationProvider for Singleton pattern.
        /// </summary>
        /// <value>A NavigationProvider.</value>
        public static Navigator Instance
        {
            get
            {
                return Nested.NestedInstance;
            }
        }

        private DispatcherTimer _timer;

        public void DisplayStatus(ConnectionStatus status)
        {
            _notificator.Visibility = Visibility.Visible;
            _notificator.Source = new BitmapImage(new Uri("ms-appx:///Assets/"+status+".png"));

             _timer.Start();
        }

        void TimerTick(object sender, object e)
        {
            _notificator.Visibility = Visibility.Collapsed;
            _timer.Stop();
        }

        public void HideNotificator()
        {
            _notificator.Visibility = Visibility.Collapsed;
            MainProgressBar.Visibility = Visibility.Collapsed;
        } 

        public void SetNotificator(Image img)
        {
            _notificator = img;
        }

        public void ShowApproveNotificator()
        {
            _approveNotificator.Visibility = Visibility.Visible;
        }
        public void HideApproveNotificator()
        {
            _approveNotificator.Visibility = Visibility.Collapsed;
        } 

        public void SetApproveNotificator(Image img)
        {
            _approveNotificator = img;
        }
        private Image _notificator;

        private Image _approveNotificator;

        public ProgressBar MainProgressBar { get; set; }

        public AppBar BottomAppBar { get; set; }

        public AppBar TopAppBar { get; set; }

        public Frame RootFrameOfPopup { get; set; }

        public Popup MainPopup
        {
            get { return _mainPopup; }
            set
            {
                _mainPopup = value;
                _mainPopup.Child.PointerPressed += (sender, args) =>
                {
                    if (sender.Equals(args.OriginalSource))
                    {
                        HideTimelinePopup();
                    }
                };
            }
        }

        public void ShowTimelinePopup(Type pageTypeViewModel, object parameter = null)
        {
            _mainPopup.IsOpen = true;
            TopAppBar.IsOpen = false;
            // Navigate to content of target page.
            ((Frame)((Panel)_mainPopup.Child).Children[0]).Navigate(pageTypeViewModel, parameter);
        }

        public void HideTimelinePopup()
        {
            TopAppBar.IsOpen = false;
            _mainPopup.IsOpen = false;
        }

        private Navigator()
        {
            _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 3) };
            _timer.Tick += TimerTick;
        }

        /// <summary>
        /// Lazy implementation of the singleton pattern.
        /// </summary>
        private class Nested
        {
            /// <summary>
            /// Instance of NavigationProvider for Singleton pattern.
            /// </summary>
            internal static readonly Navigator NestedInstance = new Navigator();
        }
    }

    public enum ConnectionStatus
    {
        Done,
        Error
    }
}
