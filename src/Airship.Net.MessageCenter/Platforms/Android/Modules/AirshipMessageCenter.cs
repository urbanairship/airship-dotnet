/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AirshipDotNet.MessageCenter;
using AirshipDotNet.Platforms.Android;
using UrbanAirship.MessageCenter;
using Java.Util;

namespace AirshipDotNet.MessageCenter.Platforms.Android.Modules
{
    /// <summary>
    /// Android implementation of Airship MessageCenter module.
    /// </summary>
    public class AirshipMessageCenter : IAirshipMessageCenter
    {
        private readonly AirshipModule _module;

        public AirshipMessageCenter(AirshipModule module)
        {
            _module = module;
        }

        internal class AirshipMessageCenterDisplayDelegate : Java.Lang.Object, MessageCenterClass.IOnShowMessageCenterListener
        {
            private readonly Action<string?> handler;

            public AirshipMessageCenterDisplayDelegate(Action<string?> handler)
            {
                this.handler = handler;
            }

            public bool OnShowMessageCenter(string? messageId)
            {
                handler?.Invoke(messageId);
                return true;
            }
        }

        /// <summary>
        /// Displays the message center.
        /// </summary>
        public Task Display()
        {
            MessageCenterClass.Shared().ShowMessageCenter();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Displays a specific message.
        /// </summary>
        /// <param name="messageId">The message ID to display.</param>
        public Task DisplayMessage(string messageId)
        {
            MessageCenterClass.Shared().ShowMessageCenter(messageId);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the unread message count.
        /// </summary>
        /// <returns>The number of unread messages.</returns>
        public Task<int> GetUnreadCount()
        {
            var tcs = new TaskCompletionSource<int>();
            MessageCenterClass.Shared().Inbox.GetUnreadCount(count => tcs.SetResult(count));
            return tcs.Task;
        }

        /// <summary>
        /// Gets the total message count.
        /// </summary>
        /// <returns>The total number of messages.</returns>
        public Task<int> GetCount()
        {
            var tcs = new TaskCompletionSource<int>();
            MessageCenterClass.Shared().Inbox.GetCount(count => tcs.SetResult(count));
            return tcs.Task;
        }

        /// <summary>
        /// Gets all inbox messages.
        /// </summary>
        /// <returns>List of inbox messages.</returns>
        public Task<List<AirshipDotNet.MessageCenter.Message>> GetMessages()
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
        public Task<AirshipDotNet.MessageCenter.Message?> GetMessage(string messageId)
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
        public Task MarkRead(params string[] messageIds)
        {
            MessageCenterClass.Shared().Inbox.MarkMessagesRead(messageIds);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Deletes messages.
        /// </summary>
        /// <param name="messageIds">The message IDs to delete.</param>
        public Task Delete(params string[] messageIds)
        {
            MessageCenterClass.Shared().Inbox.DeleteMessages(messageIds);
            return Task.CompletedTask;
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
                    MessageCenterClass.Shared().SetOnShowMessageCenterListener(messageCenterDisplayDelegate);
                }
            }
            remove
            {
                onMessageCenterDisplay -= value;

                if (onMessageCenterDisplay == null)
                {
                    MessageCenterClass.Shared().SetOnShowMessageCenterListener(null);
                    messageCenterDisplayDelegate = null;
                }
            }
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