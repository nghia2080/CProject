using System;
using System.Collections.Generic;

namespace AntaresShell.Common
{
    /// <summary>
    /// Not currrently being used in this demo, but this eventing mechanism implements the "weak-event" pattern and can be used in lieu
    /// of .NET events.  This is often useful when View Models need to signal Views, for example to start an animation, and a hard
    /// reference is undesirable.
    /// </summary>
    public class Messenger
    {
        /// <summary>
        /// Dictionary of all subscribers.
        /// </summary>
        private Dictionary<Type, List<Action<dynamic>>> _internalList;

        /// <summary>
        /// Gets the singleton instance of the <see cref="Messenger"/> class.
        /// </summary>
        /// <value>
        /// Identifier of an instance of the <see cref="Messenger"/> class.
        /// </value>
        public static Messenger Instance
        {
            get { return Nested._instance; }
        }

        /// <summary>
        /// Register to listen for a message of pageType T.
        /// </summary>
        /// <typeparam name="T">Type of the message.</typeparam>
        /// <param name="callback">Method which will be called when a message of pageType T is available.</param>
        public void Register<T>(Action<dynamic> callback)
        {
            Type messageType = typeof(T);
            List<Action<dynamic>> list;

            if (_internalList == null)
            {
                _internalList = new Dictionary<Type, List<Action<dynamic>>>();
            }

            if (!_internalList.ContainsKey(messageType))
            {
                list = new List<Action<dynamic>>();
                _internalList.Add(messageType, list);
            }
            else
            {
                list = _internalList[messageType];
            }

            list.Add(callback);
        }

        /// <summary>
        /// Send a message of pageType T to all listeners.
        /// </summary>
        /// <typeparam name="T">Type of the message.</typeparam>
        /// <param name="message">An object message of pageType T.</param>
        public void Notify<T>(T message)
        {
            Type messageType = typeof(T);
            if (_internalList == null)
            {
                return;
            }

            if (_internalList.ContainsKey(messageType))
            {
                // forward the message to all listeners
                foreach (var callback in _internalList[messageType])
                {
                    callback(message);
                }
            }
        }

        /// <summary>
        /// Lazy implementation of the singleton pattern.
        /// </summary>
        private class Nested
        {
            /// <summary>
            /// Instance of SonyRegionFactory for Singleton pattern.
            /// </summary>
            internal static readonly Messenger _instance = new Messenger();

            /// <summary>
            /// Explicit static constructor to tell C# compiler not to mark pageType as beforefieldinit.
            /// </summary>
            static Nested()
            {
            }
        }
    }
}