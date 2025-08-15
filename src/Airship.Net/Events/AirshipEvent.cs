/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;

namespace AirshipDotNet.Events
{
    /// <summary>
    /// Base class for all Airship events.
    /// </summary>
    public class AirshipEvent
    {
        /// <summary>
        /// Gets the event type.
        /// </summary>
        public AirshipEventType EventType { get; }

        /// <summary>
        /// Gets the timestamp when the event was created.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Gets the event data.
        /// </summary>
        public object? Data { get; }

        /// <summary>
        /// Initializes a new instance of the AirshipEvent class.
        /// </summary>
        /// <param name="eventType">The type of the event.</param>
        /// <param name="data">The event data.</param>
        public AirshipEvent(AirshipEventType eventType, object? data = null)
        {
            EventType = eventType;
            Data = data;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Converts the event data to EventArgs.
        /// </summary>
        /// <returns>The EventArgs representation of the event data.</returns>
        public virtual EventArgs ToEventArgs()
        {
            return Data switch
            {
                EventArgs args => args,
                string str when EventType == AirshipEventType.ChannelCreated => new ChannelEventArgs(str),
                string str when EventType == AirshipEventType.DeepLinkReceived => new DeepLinkEventArgs(str),
                PushNotificationStatus status => new PushNotificationStatusEventArgs(status),
                Dictionary<string, object> dict => CreateEventArgsFromDictionary(dict),
                _ => EventArgs.Empty
            };
        }

        /// <summary>
        /// Creates EventArgs from a dictionary of event data.
        /// </summary>
        private EventArgs CreateEventArgsFromDictionary(Dictionary<string, object> data)
        {
            switch (EventType)
            {
                case AirshipEventType.ChannelCreated:
                    if (data.TryGetValue("channelId", out var channelId))
                        return new ChannelEventArgs(channelId?.ToString() ?? "");
                    break;
                    
                case AirshipEventType.DeepLinkReceived:
                    if (data.TryGetValue("deepLink", out var deepLink))
                        return new DeepLinkEventArgs(deepLink?.ToString() ?? "");
                    break;
                    
                case AirshipEventType.NotificationStatusChanged:
                    if (data.TryGetValue("status", out var statusObj) && statusObj is PushNotificationStatus status)
                        return new PushNotificationStatusEventArgs(status);
                    break;
            }
            
            return EventArgs.Empty;
        }
    }

    /// <summary>
    /// Generic typed event for specific event data types.
    /// </summary>
    public class AirshipEvent<T> : AirshipEvent where T : EventArgs
    {
        /// <summary>
        /// Gets the strongly-typed event data.
        /// </summary>
        public new T Data { get; }

        /// <summary>
        /// Initializes a new instance of the AirshipEvent class with typed data.
        /// </summary>
        public AirshipEvent(AirshipEventType eventType, T data) : base(eventType, data)
        {
            Data = data;
        }

        /// <summary>
        /// Converts the event data to EventArgs.
        /// </summary>
        public override EventArgs ToEventArgs()
        {
            return Data;
        }
    }
}