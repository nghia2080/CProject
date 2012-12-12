using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace AntaresShell.BaseClasses
{
    /// <summary>
    /// Provide a base class that supports Command property for all List View Items.
    /// </summary>
    public class CommandableListViewItem : SelectorItem
    {
        /// <summary>
        /// The identifier for the Command dependecy property.
        /// This variable cannot be initialized inside the get accessor of the property.
        /// </summary>
        private static readonly DependencyProperty _commandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(CommandableListViewItem), new PropertyMetadata(null));

        /// <summary>
        /// The identifier for the CommandParameter dependecy property.
        /// This variable cannot be initialized inside the get accessor of the property.
        /// </summary>
        private static readonly DependencyProperty _commandParameterProperty = DependencyProperty.Register(
            "CommandParameter", typeof(object), typeof(CommandableListViewItem), new PropertyMetadata(null));

        /// <summary>
        /// The identifier for the TitleParameter dependecy property.
        /// This variable cannot be initialized inside the get accessor of the property.
        /// </summary>
        private static readonly DependencyProperty _titleParameterProperty = DependencyProperty.Register(
            "Name", typeof(string), typeof(CommandableListViewItem), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the CommandableListViewItem class.
        /// Initialize data and methods for CommandableListViewItem class.
        /// </summary>
        public CommandableListViewItem()
        {
            IsTabStop = false;
            Name = "CommandableListViewItem";
            PointerEntered += delegate { Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1); };
            PointerExited += delegate { Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1); };
        }

        /// <summary>
        /// Gets the identifier for the TitleParameter dependecy property.
        /// </summary>
        /// <value>
        /// The identifier for the TitleParameter dependecy property.
        /// </value>
        public static DependencyProperty TitleParameterProperty
        {
            get { return _titleParameterProperty; }
        }

        /// <summary>
        /// Gets the identifier for the Command dependecy property.
        /// </summary>
        /// <value>
        /// The identifier for the Command dependecy property.
        /// </value>
        public static DependencyProperty CommandProperty
        {
            get { return _commandProperty; }
        }

        /// <summary>
        /// Gets the identifier for the CommandParameter dependecy property.
        /// </summary>
        /// <value>
        /// The identifier for the CommandParameter dependecy property.
        /// </value>
        public static DependencyProperty CommandParameterProperty
        {
            get { return _commandParameterProperty; }
        }

        /// <summary>
        /// Gets or sets the Command that will be invoked by this control.
        /// </summary>
        /// <value>
        /// The Command that is attached to this instance.
        /// </value>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Command parameter that will be passed to the Command when it is invoked.
        /// </summary>
        /// <value>
        /// Command parameter that will be passed to the Command method.
        /// </value>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Name of the control.
        /// </summary>
        /// <value>
        /// The title of the control.
        /// </value>
        public virtual string Title
        {
            get { return GetValue(TitleParameterProperty) as string; }
            set { SetValue(TitleParameterProperty, value); }
        }

        /// <summary>
        /// Handles OnTapped event and invoke Command method.
        /// </summary>
        /// <param name="e">TappedRouted event arguments.</param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            Command.Execute(CommandParameter);
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }
    }
}