/* Copyright Airship and Contributors */

using System;

namespace AirshipDotNet.MessageCenter
{
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