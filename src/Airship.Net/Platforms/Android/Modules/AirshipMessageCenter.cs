/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AirshipDotNet.MessageCenter;
using UrbanAirship.MessageCenter;
using Java.Util;

namespace AirshipDotNet
{
    /// <summary>
    /// Android implementation of Airship MessageCenter module.
    /// </summary>
    internal class AirshipMessageCenter : IAirshipMessageCenter
    {
        private readonly AirshipModule _module;

        internal AirshipMessageCenter(AirshipModule module)
        {
            _module = module;
        }

        /// <summary>
        /// Displays the message center.
        /// </summary>
        public Task DisplayAsync()
        {
            MessageCenterClass.Shared().ShowMessageCenter();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Displays a specific message.
        /// </summary>
        /// <param name="messageId">The message ID to display.</param>
        public Task DisplayMessageAsync(string messageId)
        {
            MessageCenterClass.Shared().ShowMessageCenter(messageId);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Marks a message as read.
        /// </summary>
        /// <param name="messageId">The message ID to mark as read.</param>
        public Task MarkMessageReadAsync(string messageId)
        {
            var messages = new List<string> { messageId };
            MessageCenterClass.Shared().Inbox.MarkMessagesRead(messages);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <param name="messageId">The message ID to delete.</param>
        public Task DeleteMessageAsync(string messageId)
        {
            var messages = new List<string> { messageId };
            MessageCenterClass.Shared().Inbox.DeleteMessages(messages);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the unread message count.
        /// </summary>
        /// <returns>The number of unread messages.</returns>
        public Task<int> GetUnreadCountAsync()
        {
            var tcs = new TaskCompletionSource<int>();
            MessageCenterClass.Shared().Inbox.GetUnreadCount(count => tcs.SetResult(count));
            return tcs.Task;
        }

        /// <summary>
        /// Gets the total message count.
        /// </summary>
        /// <returns>The total number of messages.</returns>
        public Task<int> GetCountAsync()
        {
            var tcs = new TaskCompletionSource<int>();
            MessageCenterClass.Shared().Inbox.GetCount(count => tcs.SetResult(count));
            return tcs.Task;
        }

        /// <summary>
        /// Gets all inbox messages.
        /// </summary>
        /// <returns>List of inbox messages.</returns>
        public Task<List<AirshipDotNet.MessageCenter.Message>> GetMessagesAsync()
        {
            var tcs = new TaskCompletionSource<List<AirshipDotNet.MessageCenter.Message>>();

            MessageCenterClass.Shared().Inbox.GetMessages(messages =>
            {
                var messagesList = new List<AirshipDotNet.MessageCenter.Message>();

                foreach (var message in messages)
                {
                    var extras = new Dictionary<string, string?>();
                    foreach (var key in message.Extras.Keys)
                    {
                        extras.Add(key, message.Extras[key]);
                    }

                    DateTime? sentDate = FromDate(message.SentDate);
                    DateTime? expirationDate = FromDate(message.ExpirationDate);

                    var inboxMessage = new AirshipDotNet.MessageCenter.Message(
                        message.Id,
                        message.Title,
                        sentDate,
                        expirationDate,
                        message.IsRead,
                        message.ListIconUrl,
                        extras);

                    messagesList.Add(inboxMessage);
                }

                tcs.SetResult(messagesList);
            });

            return tcs.Task;
        }

        /// <summary>
        /// Gets a specific message by ID.
        /// </summary>
        /// <param name="messageId">The message ID.</param>
        /// <returns>The message or null if not found.</returns>
        public Task<AirshipDotNet.MessageCenter.Message?> GetMessageAsync(string messageId)
        {
            var tcs = new TaskCompletionSource<AirshipDotNet.MessageCenter.Message?>();

            MessageCenterClass.Shared().Inbox.GetMessage(messageId, message =>
            {
                if (message == null)
                {
                    tcs.SetResult(null);
                    return;
                }

                var extras = new Dictionary<string, string?>();
                foreach (var key in message.Extras.Keys)
                {
                    extras.Add(key, message.Extras[key]);
                }

                DateTime? sentDate = FromDate(message.SentDate);
                DateTime? expirationDate = FromDate(message.ExpirationDate);

                var inboxMessage = new AirshipDotNet.MessageCenter.Message(
                    message.Id,
                    message.Title,
                    sentDate,
                    expirationDate,
                    message.IsRead,
                    message.ListIconUrl,
                    extras);

                tcs.SetResult(inboxMessage);
            });

            return tcs.Task;
        }

        /// <summary>
        /// Marks messages as read.
        /// </summary>
        /// <param name="messageIds">The message IDs to mark as read.</param>
        public Task MarkReadAsync(params string[] messageIds)
        {
            MessageCenterClass.Shared().Inbox.MarkMessagesRead(messageIds);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Deletes messages.
        /// </summary>
        /// <param name="messageIds">The message IDs to delete.</param>
        public Task DeleteAsync(params string[] messageIds)
        {
            MessageCenterClass.Shared().Inbox.DeleteMessages(messageIds);
            return Task.CompletedTask;
        }

        private static DateTime? FromDate(Date? date)
        {
            if (date == null)
            {
                return null;
            }
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(date.Time);
        }
    }
}