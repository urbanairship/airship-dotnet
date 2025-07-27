/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AirshipDotNet.MessageCenter;
using Foundation;
using Airship;

namespace AirshipDotNet.Platforms.iOS.Modules
{
    /// <summary>
    /// iOS implementation of Airship Message Center module.
    /// </summary>
    internal class AirshipMessageCenter : IAirshipMessageCenter
    {
        private readonly AirshipModule _module;

        internal AirshipMessageCenter(AirshipModule module)
        {
            _module = module;
        }

        /// <summary>
        /// Gets all messages from the message center.
        /// </summary>
        /// <returns>List of messages.</returns>
        public Task<List<Message>> GetMessagesAsync()
        {
            var tcs = new TaskCompletionSource<List<Message>>();

            // This is a wrapped method - handle it carefully
            AWAirshipWrapper.GetMessages(messages =>
            {
                var messagesList = new List<Message>();

                if (messages != null)
                {
                    foreach (var message in messages)
                    {
                        var extras = new Dictionary<string, string?>();
                        if (message.Extra != null)
                        {
                            foreach (var key in message.Extra.Keys)
                            {
                                extras.Add(key.ToString(), message.Extra[key]?.ToString());
                            }
                        }

                        DateTime? sentDate = FromNSDate(message.SentDate);
                        DateTime? expirationDate = FromNSDate(message.ExpirationDate);

                        var inboxMessage = new Message(
                            message.Id,
                            message.Title,
                            sentDate,
                            expirationDate,
                            message.Unread,
                            message.ListIcon,
                            extras);

                        messagesList.Add(inboxMessage);
                    }
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
        public async Task<Message?> GetMessageAsync(string messageId)
        {
            var messages = await GetMessagesAsync();
            return messages.FirstOrDefault(m => m.MessageId == messageId);
        }

        /// <summary>
        /// Gets the unread message count.
        /// </summary>
        /// <returns>The number of unread messages.</returns>
        public async Task<int> GetUnreadCountAsync()
        {
            var messages = await GetMessagesAsync();
            return messages.Count(m => m.Unread);
        }

        /// <summary>
        /// Gets the total message count.
        /// </summary>
        /// <returns>The total number of messages.</returns>
        public async Task<int> GetCountAsync()
        {
            var messages = await GetMessagesAsync();
            return messages.Count;
        }

        /// <summary>
        /// Marks messages as read.
        /// </summary>
        /// <param name="messageIds">The message IDs to mark as read.</param>
        public Task MarkReadAsync(params string[] messageIds)
        {
            var tcs = new TaskCompletionSource<bool>();

            // This is a wrapped method - handle it carefully
            AWAirshipWrapper.MarkReadWithMessageIDs(messageIds, () =>
            {
                tcs.SetResult(true);
            });

            return tcs.Task;
        }

        /// <summary>
        /// Deletes messages.
        /// </summary>
        /// <param name="messageIds">The message IDs to delete.</param>
        public Task DeleteAsync(params string[] messageIds)
        {
            return Task.Run(() =>
            {
                UAirship.MessageCenter.Inbox.DeleteWithMessageIDs(messageIds, () => { });
            });
        }

        /// <summary>
        /// Displays the message center.
        /// </summary>
        public Task DisplayAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

            NSRunLoop.Main.InvokeOnMainThread(() =>
            {
                UAirship.MessageCenter.Display();
                tcs.SetResult(true);
            });

            return tcs.Task;
        }

        /// <summary>
        /// Displays a specific message.
        /// </summary>
        /// <param name="messageId">The message ID to display.</param>
        public Task DisplayMessageAsync(string messageId)
        {
            var tcs = new TaskCompletionSource<bool>();

            NSRunLoop.Main.InvokeOnMainThread(() =>
            {
                UAirship.MessageCenter.DisplayWithMessageID(messageId);
                tcs.SetResult(true);
            });

            return tcs.Task;
        }

        private static DateTime? FromNSDate(NSDate? date)
        {
            if (date == null)
            {
                return null;
            }
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(date.SecondsSince1970);
        }

        // Event handlers will be added in a future phase to maintain existing event patterns
        // For now, users can still use the legacy events through Airship.Instance if needed
    }
}