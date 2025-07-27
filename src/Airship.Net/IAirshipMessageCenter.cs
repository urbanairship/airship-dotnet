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
        Task<List<Message>> GetMessagesAsync();

        /// <summary>
        /// Gets a specific message by ID.
        /// </summary>
        /// <param name="messageId">The message ID.</param>
        /// <returns>The message or null if not found.</returns>
        Task<Message?> GetMessageAsync(string messageId);

        /// <summary>
        /// Gets the unread message count.
        /// </summary>
        /// <returns>The number of unread messages.</returns>
        Task<int> GetUnreadCountAsync();

        /// <summary>
        /// Gets the total message count.
        /// </summary>
        /// <returns>The total number of messages.</returns>
        Task<int> GetCountAsync();

        /// <summary>
        /// Marks messages as read.
        /// </summary>
        /// <param name="messageIds">The message IDs to mark as read.</param>
        Task MarkReadAsync(params string[] messageIds);

        /// <summary>
        /// Deletes messages.
        /// </summary>
        /// <param name="messageIds">The message IDs to delete.</param>
        Task DeleteAsync(params string[] messageIds);

        /// <summary>
        /// Displays the message center.
        /// </summary>
        Task DisplayAsync();

        /// <summary>
        /// Displays a specific message.
        /// </summary>
        /// <param name="messageId">The message ID to display.</param>
        Task DisplayMessageAsync(string messageId);
    }
}