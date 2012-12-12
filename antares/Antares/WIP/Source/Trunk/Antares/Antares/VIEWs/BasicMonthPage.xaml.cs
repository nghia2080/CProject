using Antares.VIEWMODELs;
using AntaresShell.Common;
using System;
using System.Collections.Generic;
using AntaresShell.Common.MessageTemplates;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Antares.VIEWs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BasicMonthPage
    {
        public BasicMonthPage()
        {
            NavigationCacheMode = NavigationCacheMode.Enabled;

            InitializeComponent();
            GotoDate.ValueChanged += GotoDate_ValueChanged;

            DynamicArea.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(DynamicArea_PointerPressed), true);
            DynamicArea.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(DynamicArea_PointerReleased), true);

            //DynamicArea.PointerPressed += DynamicArea_PointerPressed;
            //DynamicArea.PointerReleased += DynamicArea_PointerReleased;
        }

        void DynamicArea_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if(_firstPoint == null)
            {
                return;
            }

            var secondPoint = e.GetCurrentPoint(null);

            if(secondPoint.PointerId == _firstPoint.PointerId)
            {
                var deltaX = secondPoint.Position.X - _firstPoint.Position.X;
                var deltaY = secondPoint.Position.Y - _firstPoint.Position.Y;
               
                // Horizontal scrolling
                if(Math.Abs(deltaX) >  Math.Abs(deltaY))
                {
                    var left = deltaX > 0;
                    if(left)
                    {
                        ((BasicMonthViewModel)this.DataContext).ExecutePrevious(null);
                    }
                    else
                    {
                        ((BasicMonthViewModel)this.DataContext).ExecuteNext(null);
                    }
                }
            }
        }

        private PointerPoint _firstPoint;
        void DynamicArea_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            _firstPoint = e.GetCurrentPoint(null);
        }

        void GotoDate_ValueChanged(object sender, EventArgs e)
        {
            Messenger.Instance.Notify(new GotoMonth { Target = GotoDate.Value});
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            
        }
    }
}
