/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AirshipDotNet.MessageCenter;
using AirshipDotNet.Platforms.iOS;
using Foundation;
using Airship;

namespace AirshipDotNet.Platforms.iOS.Modules
{
    /// <summary>
    /// iOS implementation of Airship Message Center module.
    /// </summary>
    public class AirshipMessageCenter : IAirshipMessageCenter
    {
        private readonly AirshipModule _module;

        public AirshipMessageCenter(AirshipModule module)
        {
            _module = module;
        }

        internal class AirshipMessageCenterDisplayDelegate : global::Airship.UAMessageCenterDisplayDelegate
        {
            private readonly Action<string?> handler;

            public AirshipMessageCenterDisplayDelegate(Action<string?> handler)
            {
                this.handler = handler;
            }

            public override void DisplayMessageCenterForMessageID(string messageId)
            {
                handler?.Invoke(messageId);
            }

            public override void DisplayMessageCenter()
            {

            }

            public override void DismissMessageCenter()
            {
                handler?.Invoke(null);
            }
        }

        /// <summary>
        /// Gets all messages from the message center.
        /// </summary>
        /// <returns>List of messages.</returns>
        public Task<List<Message>> GetMessages()
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
        public async Task<Message?> GetMessage(string messageId)
        {
            var messages = await GetMessages();
            return messages.FirstOrDefault(m => m.MessageId == messageId);
        }

        /// <summary>
        /// Gets the unread message count.
        /// </summary>
        /// <returns>The number of unread messages.</returns>
        public async Task<int> GetUnreadCount()
        {
            var messages = await GetMessages();
            return messages.Count(m => m.Unread);
        }

        /// <summary>
        /// Gets the total message count.
        /// </summary>
        /// <returns>The total number of messages.</returns>
        public async Task<int> GetCount()
        {
            var messages = await GetMessages();
            return messages.Count;
        }

        /// <summary>
        /// Marks messages as read.
        /// </summary>
        /// <param name="messageIds">The message IDs to mark as read.</param>
        public Task MarkRead(params string[] messageIds)
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
        public Task Delete(params string[] messageIds)
        {
            return Task.Run(() =>
            {
                AWAirshipWrapper.Shared.MessageCenter.Inbox.DeleteWithMessageIDs(messageIds, () => { });
            });
        }

        /// <summary>
        /// Displays the message center.
        /// </summary>
        public Task Display()
        {
            var tcs = new TaskCompletionSource<bool>();

            NSRunLoop.Main.InvokeOnMainThread(() =>
            {
                AWAirshipWrapper.Shared.MessageCenter.Display();
                tcs.SetResult(true);
            });

            return tcs.Task;
        }

        /// <summary>
        /// Displays a specific message.
        /// </summary>
        /// <param name="messageId">The message ID to display.</param>
        public Task DisplayMessage(string messageId)
        {
            var tcs = new TaskCompletionSource<bool>();

            NSRunLoop.Main.InvokeOnMainThread(() =>
            {
                AWAirshipWrapper.Shared.MessageCenter.DisplayWithMessageID(messageId);
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

        private EventHandler<MessageCenterEventArgs>? onMessageCenterDisplay;
        private AirshipMessageCenterDisplayDelegate? messageCenterDisplayDelegate;

        /// <summary>
        /// Add/remove the Message Center display listener.
        /// </summary>
        public event EventHandler<MessageCenterEventArgs> OnMessageCenterDisplay
        {
            add
            {
                onMessageCenterDisplay += value;
                if (messageCenterDisplayDelegate == null)
                {
                    messageCenterDisplayDelegate = new AirshipMessageCenterDisplayDelegate((messageId) =>
                    {
                        onMessageCenterDisplay?.Invoke(this, new MessageCenterEventArgs(messageId));
                    });
                    UAirship.MessageCenter.WeakDisplayDelegate = messageCenterDisplayDelegate;
                }
            }
            remove
            {
                onMessageCenterDisplay -= value;

                if (onMessageCenterDisplay == null)
                {
                    UAirship.MessageCenter.WeakDisplayDelegate = null;
                    messageCenterDisplayDelegate = null;
                }
            }
        }

    }
}
