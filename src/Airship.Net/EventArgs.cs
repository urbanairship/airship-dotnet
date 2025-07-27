/* Copyright Airship and Contributors */

using System;

namespace AirshipDotNet
{
    /// <summary>
    /// Event args for channel creation events.
    /// </summary>
    public class ChannelEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the channel ID.
        /// </summary>
        public string ChannelId { get; }
        
        /// <summary>
        /// Initializes a new instance of the ChannelEventArgs class.
        /// </summary>
        /// <param name="channelId">The channel ID.</param>
        public ChannelEventArgs(string channelId)
        {
            ChannelId = channelId;
        }
    }
    
    /// <summary>
    /// Event args for push notification status update events.
    /// </summary>
    public class PushNotificationStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the push notification status.
        /// </summary>
        public PushNotificationStatus Status { get; }
        
        /// <summary>
        /// Initializes a new instance of the PushNotificationStatusEventArgs class.
        /// </summary>
        /// <param name="status">The push notification status.</param>
        public PushNotificationStatusEventArgs(PushNotificationStatus status)
        {
            Status = status;
        }
    }
    
    /// <summary>
    /// Event args for deep link events.
    /// </summary>
    public class DeepLinkEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the deep link.
        /// </summary>
        public string DeepLink { get; }
        
        /// <summary>
        /// Initializes a new instance of the DeepLinkEventArgs class.
        /// </summary>
        /// <param name="deepLink">The deep link.</param>
        public DeepLinkEventArgs(string deepLink)
        {
            DeepLink = deepLink;
        }
    }
    
    /// <summary>
    /// Event args for message center display events.
    /// </summary>
    public class MessageCenterEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the message ID, or null if displaying the message center list.
        /// </summary>
        public string? MessageId { get; }
        
        /// <summary>
        /// Initializes a new instance of the MessageCenterEventArgs class.
        /// </summary>
        /// <param name="messageId">The message ID, or null if displaying the message center list.</param>
        public MessageCenterEventArgs(string? messageId)
        {
            MessageId = messageId;
        }
    }
}