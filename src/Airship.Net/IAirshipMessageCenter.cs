/* Copyright Airship and Contributors */

using System.Collections.Generic;
using System.Threading.Tasks;
using AirshipDotNet.MessageCenter;

namespace AirshipDotNet
{
    /// <summary>
    /// Airship Message Center interface.
    /// </summary>
    public interface IAirshipMessageCenter
    {
        /// <summary>
        /// Gets all messages from the message center.
        /// </summary>
        /// <returns>List of messages.</returns>
        Task<List<Message>> GetMessages();

        /// <summary>
        /// Gets a specific message by ID.
        /// </summary>
        /// <param name="messageId">The message ID.</param>
        /// <returns>The message or null if not found.</returns>
        Task<Message?> GetMessage(string messageId);

        /// <summary>
        /// Gets the unread message count.
        /// </summary>
        /// <returns>The number of unread messages.</returns>
        Task<int> GetUnreadCount();

        /// <summary>
        /// Gets the total message count.
        /// </summary>
        /// <returns>The total number of messages.</returns>
        Task<int> GetCount();

        /// <summary>
        /// Marks messages as read.
        /// </summary>
        /// <param name="messageIds">The message IDs to mark as read.</param>
        Task MarkRead(params string[] messageIds);

        /// <summary>
        /// Deletes messages.
        /// </summary>
        /// <param name="messageIds">The message IDs to delete.</param>
        Task Delete(params string[] messageIds);

        /// <summary>
        /// Displays the message center.
        /// </summary>
        Task Display();

        /// <summary>
        /// Displays a specific message.
        /// </summary>
        /// <param name="messageId">The message ID to display.</param>
        Task DisplayMessage(string messageId);

        /// <summary>
        /// Add/remove the Message Center display listener.
        /// </summary>
        public event EventHandler<MessageCenterEventArgs> OnMessageCenterDisplay;

        /// <summary>
        /// Add/remove the Message Center updated listener.
        /// </summary>
        public event EventHandler<EventArgs> OnMessagesUpdated;
    }
}
