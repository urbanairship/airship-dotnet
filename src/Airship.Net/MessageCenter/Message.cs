﻿/* Copyright Airship and Contributors */

namespace AirshipDotNet.MessageCenter
{
    /// <summary>
    /// A Message model object.
    /// </summary>
    [Preserve(AllMembers = true)]
    public partial class Message
    {
        /// <summary>
        /// Gets the message ID.
        /// </summary>
        /// <value>The message ID.</value>
        public string MessageId { get; }

        /// <summary>
        /// Gets the message title.
        /// </summary>
        /// <value>The message title.</value>
        public string Title { get; }

        /// <summary>
        /// Gets the message sent date.
        /// </summary>
        /// <value>The message sent date.</value>
        public DateTime? SentDate { get; }

        /// <summary>
        /// Gets the message expiration date.
        /// </summary>
        /// <value>The message expiration date.</value>
        public DateTime? ExpirationDate { get; }

        /// <summary>
        /// Gets the unread status boolean value.
        /// </summary>
        /// <value>The unread status.</value>
        public bool Unread { get; }

        /// <summary>
        /// Gets the message icon url.
        /// </summary>
        /// <value>The message icon url.</value>
        public string? IconUrl { get; }

        /// <summary>
        /// Gets a dictionary of the message extras.
        /// </summary>
        /// <value>The dictionary of the message extras.</value>
        public Dictionary<string, string?>? Extras { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        public Message(string messageId, string title, DateTime? sentDate, DateTime? expirationDate, bool unread, string? iconUrl, Dictionary<string, string?>? extras)
        {
            MessageId = messageId;
            Title = title;
            SentDate = sentDate;
            ExpirationDate = expirationDate;
            Unread = unread;
            IconUrl = iconUrl;
            Extras = extras;
        }

    }
}
