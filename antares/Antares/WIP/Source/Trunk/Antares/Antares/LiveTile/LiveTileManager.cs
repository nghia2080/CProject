using System;
using System.Globalization;
using Antares.Converters;
using AntaresShell.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using AntaresShell.Common.MessageTemplates;
using Repository.MODELs;
using Repository.Repositories;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Antares.LiveTile
{
    /// <summary>
    /// Class LiveTileManager contains methods to get all messages from server and
    /// display on Live Tile of Start screen.
    /// </summary>
    public class LiveTileManager
    {
        /// <summary>
        /// Constant of tag text.
        /// </summary>
        private const string TEXT_ELEMENT = "text";

        /// <summary>
        /// Constant of tag Image.
        /// </summary>
        private const string IMAGE_ELEMENT = "image";

        /// <summary>
        /// Constant of tag Source.
        /// </summary>
        private const string TAG_SOURCE = "src";

        /// <summary>
        /// Constant of tag Alternative.
        /// </summary>
        private const string TAG_ALTERNATIVE = "alt";

        /// <summary>
        /// Constant of tag Value.
        /// </summary>
        private const string TAG_VALUE = "value";

        /// <summary>
        /// Constant of tag Badge.
        /// </summary>
        private const string TAG_BADGE = "/badge";

        /// <summary>
        /// Constant of tag Version.
        /// </summary>
        private const string TAG_VERSION = "version";

        /// <summary>
        /// Constant of Version value.
        /// </summary>
        private const string VERSION_VALUE = "1";

        /// <summary>
        /// Constant of tag binding.
        /// </summary>
        private const string TAG_BINDING = "binding";

        /// <summary>
        /// Constant of tag name in Live Tile.
        /// </summary>
        private const string TAG_NAME = "Name";

        /// <summary>
        /// Constant of tag logo in Live Tile.
        /// </summary>
        private const string TAG_LOGO = "Logo";

        /// <summary>
        /// An example of image description.
        /// </summary>
        private const string IMAGE_DESCRIPTION = "Image description";

        /// <summary>
        /// Constant of tag branding.
        /// </summary>
        private const string TAG_BRANDING = "branding";

        /// <summary>
        /// Constant of tag visual.
        /// </summary>
        private const string TAG_VISUAL = "visual";

        /// <summary>
        /// Path of normal wide Image.
        /// </summary>
        private const string WIDE_NORMAL_IMAGE = "ms-appx:///Assets/WideLogoBlank.png";

        /// <summary>
        /// Path of normal square Image.
        /// </summary>
        private const string SQUARE_NORMAL_IMAGE = "ms-appx:///Assets/Logo.png";

        /// <summary>
        /// Constant of max number of messages which Live Tile can display.
        /// </summary>
        private const int MAX_MESSAGE = 5;

        /// <summary>
        /// A dictionary contains all new and critical message to add to queue.
        /// Dictionary only contains Titles and Descriptions.
        /// </summary>
        private readonly Dictionary<string, string> _dictionary = new Dictionary<string, string>();

        /// <summary>
        /// Path of image was used in Wide Logo.
        /// </summary>
        private string _wideImagePath = string.Empty;

        /// <summary>
        /// Path of image was used in Square Logo.
        /// </summary>
        private string _squareImagePath = string.Empty;

        /// <summary>
        /// Initializes a new instance of the LiveTileManager class.
        /// </summary>
        private LiveTileManager()
        {
            Messenger.Instance.Register<UpdateTaskList>(MessageRetrieve);
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            ClearTileAndBadge();
        }

        private static readonly LiveTileManager _instance = new LiveTileManager();
        public static LiveTileManager Instance
        {
            get { return _instance; }
        }

        public void Start()
        {
            // invoke
        }

        /// <summary>
        /// Retrieve message when notified from Messenger.
        /// </summary>
        /// <param name="obj">Name of Message was pushed.</param>
        private void MessageRetrieve(object obj)
        {
            UpdateMessageTileAsync(obj);
        }

        private int _numberOfNew;

        /// <summary>
        /// Get all message from Repository to display in first Live Tile.
        /// </summary>
        private async void UpdateMessageTileAsync(object messages)
        {
            ClearTileAndBadge();
            _dictionary.Clear();
            IEnumerable<TaskModel> tasks = await TaskRepository.Instance.GetTaskListFor(DateTime.Now);

            if (tasks != null)
            {
                tasks = tasks.Union(await TaskRepository.Instance.GetTaskListFor(DateTime.Now.AddDays(1)));
            }
            else
            {
                tasks = await TaskRepository.Instance.GetTaskListFor(DateTime.Now.AddDays(1));
            }


            if (tasks != null)
            {
                tasks = tasks.Union(await TaskRepository.Instance.GetTaskListFor(DateTime.Now.AddDays(2)));
            }
            else
            {
                tasks = await TaskRepository.Instance.GetTaskListFor(DateTime.Now.AddDays(2));
            }


            _wideImagePath = WIDE_NORMAL_IMAGE;
            _squareImagePath = SQUARE_NORMAL_IMAGE;


            if (tasks != null)
            {
                _numberOfNew = tasks.Count();
                foreach (var item in tasks)
                {
                    if (_dictionary.Count < MAX_MESSAGE)
                    {
                        var key = intToTimeConverter.Convert(item.StartTime, null, null, null) + " " +
                                  Convert.ToDateTime(item.StartDate).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
                        if (!_dictionary.ContainsKey(key))
                        {
                            _dictionary.Add(key, item.Name + "\r\n" + item.Description
                                                            );
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                //if (_dictionary.Count > 0)
                //{
                //    _dictionary.Reverse();
                CreateTileQueue(_dictionary);
                //}

                //DisplayFirstTile(taskCount, _wideImagePath, _squareImagePath);
            }
        }

        IntToShortTimeConverter intToTimeConverter = new IntToShortTimeConverter();

        /// <summary>
        /// Dislay number of New Message with VAIO logo.
        /// </summary>
        /// <param name="numberOfNewMessages">Number of New messages taken from UpdateMessageTileAsync().</param>
        /// <param name="wideImagePath">Path of VAIO logo wide image.</param>
        /// <param name="squareImagePath">Path of VAIO logo square image.</param>
        private void DisplayFirstTile(int numberOfNewMessages, string wideImagePath, string squareImagePath)
        {
            var tileWideXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideImage);

            var imageElements = tileWideXml.GetElementsByTagName(IMAGE_ELEMENT);

            var imageElement0 = (XmlElement)imageElements.Item(0);
            imageElement0.SetAttribute(TAG_SOURCE, wideImagePath);
            imageElement0.SetAttribute(TAG_ALTERNATIVE, IMAGE_DESCRIPTION);

            var bindingElement = (XmlElement)tileWideXml.GetElementsByTagName(TAG_BINDING).Item(0);
            bindingElement.SetAttribute(TAG_BRANDING, TAG_NAME);

            var tileSquareXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquareImage);

            var imageSquareElements = tileSquareXml.GetElementsByTagName(IMAGE_ELEMENT);

            var imageSquareElement = (XmlElement)imageSquareElements.Item(0);
            imageSquareElement.SetAttribute(TAG_SOURCE, squareImagePath);
            imageSquareElement.SetAttribute(TAG_ALTERNATIVE, IMAGE_DESCRIPTION);

            var bindingWideElement = (XmlElement)tileSquareXml.GetElementsByTagName(TAG_BINDING).Item(0);
            bindingWideElement.SetAttribute(TAG_BRANDING, _numberOfNew > 0 ? "None" : TAG_NAME);

            // include the square template in the notification
            var subnode = tileWideXml.ImportNode(tileSquareXml.GetElementsByTagName(TAG_BINDING).Item(0), true);
            tileWideXml.GetElementsByTagName(TAG_VISUAL).Item(0).AppendChild(subnode);

            var tileWide = new TileNotification(tileWideXml);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileWide);

            if (_numberOfNew > 0)
            {
                UpdateBadgeWithNumber(_numberOfNew);
            }
        }

        /// <summary>
        /// Use a set of message in order to add into queue of Wide and Square type of Live Tile.
        /// </summary>
        /// <param name="allMessageDictionary">A dictionary contains all new and critical message to add to queue.</param>
        private void CreateTileQueue(Dictionary<string, string> allMessageDictionary)
        {
            var uniqueKey = 1;
            foreach (var key in allMessageDictionary.Keys)
            {
                var tileWideXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideText09);

                var textElements = tileWideXml.GetElementsByTagName(TEXT_ELEMENT);
                textElements.Item(0).AppendChild(tileWideXml.CreateTextNode(key));
                textElements.Item(1).AppendChild(tileWideXml.CreateTextNode(allMessageDictionary[key]));

                var bindingElement = (XmlElement)tileWideXml.GetElementsByTagName(TAG_BINDING).Item(0);
                bindingElement.SetAttribute(TAG_BRANDING, TAG_LOGO);

                var bindingWideElement = (XmlElement)tileWideXml.GetElementsByTagName(TAG_BINDING).Item(0);
                bindingWideElement.SetAttribute(TAG_BRANDING, allMessageDictionary.Count > 0 ? "None" : TAG_NAME);
                var tileNotification = new TileNotification(tileWideXml);

                var tag = string.Empty;
                if (!key.Equals(string.Empty))
                {
                    tag = key.Length > 16 ? new StringBuilder(key.Substring(0, 14)).Append(uniqueKey.ToString()).ToString()
                        : new StringBuilder(key).Append(uniqueKey.ToString()).ToString();

                    uniqueKey++;
                }

                if (allMessageDictionary.Count > 0)
                {
                    UpdateBadgeWithNumber(_numberOfNew);
                }
                // set the tag on the notifications
                if (!string.IsNullOrEmpty(tag))
                {
                    tileNotification.Tag = tag.Length >= 16 ? tag.Substring(0, 16) : tag;
                }

                TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
            }
        }

        /// <summary>
        /// Update badge of tile with a number.
        /// </summary>
        /// <param name="number">Number will be displayed.</param>
        private void UpdateBadgeWithNumber(int number)
        {
            // GetTemplateContent returns a Windows.Data.Xml.Dom.XmlDocument object containing the badge XML
            var badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);

            // Specify all of the required parameters for the badge
            var badgeElement = (XmlElement)badgeXml.SelectSingleNode(TAG_BADGE);
            badgeElement.SetAttribute(TAG_VALUE, string.Empty + number);
            badgeElement.SetAttribute(TAG_VERSION, VERSION_VALUE);

            // Create a badge notification from the Xml
            var badge = new BadgeNotification(badgeXml);

            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);
        }

        /// <summary>
        /// Clear all custom content and number to default state.
        /// </summary>
        private void ClearTileAndBadge()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
        }
    }
}