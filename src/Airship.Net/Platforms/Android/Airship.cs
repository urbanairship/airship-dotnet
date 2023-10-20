﻿/* Copyright Airship and Contributors */

using Java.Util;
using Java.Util.Concurrent;
using UrbanAirship;
using UrbanAirship.Automation;
using UrbanAirship.Actions;
using UrbanAirship.Channel;
//using Urbanairship.Contacts;
using UrbanAirship.MessageCenter;
using UrbanAirship.Push;
using AttributeEditor = AirshipDotNet.Attributes.AttributeEditor;
using ChannelSubscriptionListEditor = AirshipDotNet.Channel.SubscriptionListEditor;
using ContactSubscriptionListEditor = AirshipDotNet.Contact.SubscriptionListEditor;

namespace AirshipDotNet
{
    /// <summary>
    /// Provides cross-platform access to a common subset of functionality between the iOS and Android SDKs
    /// </summary>
    public class Airship : Java.Lang.Object, IDeepLinkListener, IAirship, IInboxListener, MessageCenterClass.IOnShowMessageCenterListener, UrbanAirship.Channel.IAirshipChannelListener, IPushNotificationStatusListener
    {
        private static readonly Lazy<Airship> sharedAirship = new(() =>
        {
            Airship instance = new();
            instance.Init();
            return instance;
        });

        private void Init()
        {
            UAirship.Shared().Channel.AddChannelListener(this);

            //Adding Push notification status listener
            UAirship.Shared().PushManager.AddNotificationStatusListener(this);

            //Adding Inbox updated listener
            MessageCenterClass.Shared().Inbox.AddListener(this);
        }

        /// <summary>
        /// Add/remove the channel creation listener.
        /// </summary>
        public event EventHandler<ChannelEventArgs>? OnChannelCreation;

        /// <summary>
        /// Add/remove the push notification status listener.
        /// </summary>
        public event EventHandler<PushNotificationStatusEventArgs>? OnPushNotificationStatusUpdate;

        private EventHandler<DeepLinkEventArgs>? onDeepLinkReceived;

        /// <summary>
        /// Add/remove the deep link listener.
        /// </summary>
        public event EventHandler<DeepLinkEventArgs> OnDeepLinkReceived
        {
            add
            {
                onDeepLinkReceived += value;
                UAirship.Shared().DeepLinkListener = this;
            }

            remove
            {
                onDeepLinkReceived -= value;

                if (onDeepLinkReceived == null)
                {
                    UAirship.Shared().DeepLinkListener = null;
                }
            }
        }

        /// <summary>
        /// Add/remove the Message Center updated listener.
        /// </summary>
        public event EventHandler? OnMessageCenterUpdated;

        private EventHandler<MessageCenterEventArgs>? onMessageCenterDisplay;

        /// <summary>
        /// Add/remove the Message Center display listener.
        /// </summary>
        public event EventHandler<MessageCenterEventArgs> OnMessageCenterDisplay
        {
            add
            {
                onMessageCenterDisplay += value;
                MessageCenterClass.Shared().SetOnShowMessageCenterListener(this);
            }

            remove
            {
                onMessageCenterDisplay -= value;

                if (onMessageCenterDisplay == null)
                {
                    MessageCenterClass.Shared().SetOnShowMessageCenterListener(null);
                }
            }
        }

        public static Airship Instance => sharedAirship.Value;

        public bool UserNotificationsEnabled
        {
            get => UAirship.Shared().PushManager.UserNotificationsEnabled;
            set => UAirship.Shared().PushManager.UserNotificationsEnabled = value;
        }

        public Features EnabledFeatures
        {
            get => FeaturesFromUAFeatures(UAirship.Shared().PrivacyManager.EnabledFeatures);
            set => UAirship.Shared().PrivacyManager.SetEnabledFeatures(UaFeaturesFromFeatures(value));
        }

        public void EnableFeatures(Features features) => UAirship.Shared().PrivacyManager.Enable(UaFeaturesFromFeatures(features));

        public void DisableFeatures(Features features) => UAirship.Shared().PrivacyManager.Disable(UaFeaturesFromFeatures(features));

        public bool IsFeatureEnabled(Features feature) => EnabledFeatures.HasFlag(feature);

        public bool IsAnyFeatureEnabled() => EnabledFeatures != Features.None;

        private static int[] UaFeaturesFromFeatures(Features features)
        {
            List<int> uAFeatures = new();

            if (features.HasFlag(Features.InAppAutomation))
            {
                uAFeatures.Add(PrivacyManager.FeatureInAppAutomation);
            }
            if (features.HasFlag(Features.MessageCenter))
            {
                uAFeatures.Add(PrivacyManager.FeatureMessageCenter);
            }
            if (features.HasFlag(Features.Push))
            {
                uAFeatures.Add(PrivacyManager.FeaturePush);
            }
            if (features.HasFlag(Features.Analytics))
            {
                uAFeatures.Add(PrivacyManager.FeatureAnalytics);
            }
            if (features.HasFlag(Features.TagsAndAttributes))
            {
                uAFeatures.Add(PrivacyManager.FeatureTagsAndAttributes);
            }
            if (features.HasFlag(Features.Contacts))
            {
                uAFeatures.Add(PrivacyManager.FeatureContacts);
            }

            return uAFeatures.ToArray();
        }

        private static Features FeaturesFromUAFeatures(int uAFeatures)
        {
            Features features = Features.None;

            if ((uAFeatures & PrivacyManager.FeatureInAppAutomation) == PrivacyManager.FeatureInAppAutomation)
            {
                features |= Features.InAppAutomation;
            }

            if ((uAFeatures & PrivacyManager.FeatureMessageCenter) == PrivacyManager.FeatureMessageCenter)
            {
                features |= Features.MessageCenter;
            }

            if ((uAFeatures & PrivacyManager.FeaturePush) == PrivacyManager.FeaturePush)
            {
                features |= Features.Push;
            }

            if ((uAFeatures & PrivacyManager.FeatureAnalytics) == PrivacyManager.FeatureAnalytics)
            {
                features |= Features.Analytics;
            }

            if ((uAFeatures & PrivacyManager.FeatureTagsAndAttributes) == PrivacyManager.FeatureTagsAndAttributes)
            {
                features |= Features.TagsAndAttributes;
            }

            if ((uAFeatures & PrivacyManager.FeatureContacts) == PrivacyManager.FeatureContacts)
            {
                features |= Features.Contacts;
            }

            return features;
        }


        public IEnumerable<string> Tags => UAirship.Shared().Channel.Tags;

        public string? ChannelId => UAirship.Shared().Channel.Id;

        public void GetNamedUser(Action<string> namedUser) => namedUser(UAirship.Shared().Contact.NamedUserId);

        public void ResetContact() => UAirship.Shared().Contact.Reset();

        public void IdentifyContact(string namedUserId) => UAirship.Shared().Contact.Identify(namedUserId);

        public Channel.TagEditor EditDeviceTags()
        {
            return new Channel.TagEditor(DeviceTagHelper);
        }

        private void DeviceTagHelper(bool clear, string[] addTags, string[] removeTags)
        {
            var editor = UAirship.Shared().Channel.EditTags();

            if (clear)
            {
                editor = editor.Clear();
            }

            editor.AddTags(addTags).RemoveTags(removeTags).Apply();
        }

        public void AddCustomEvent(Analytics.CustomEvent customEvent)
        {
            if (customEvent == null || string.IsNullOrEmpty(customEvent.EventName))
            {
                return;
            }

            var eventName = customEvent.EventName;
            var eventValue = customEvent.EventValue;
            var transactionId = customEvent.TransactionId;
            var interactionType = customEvent.InteractionType;
            var interactionId = customEvent.InteractionId;

            var builder = new UrbanAirship.Analytics.CustomEvent.Builder(eventName);

            if (eventValue is not null)
            {
                builder.SetEventValue((double)eventValue);
            }

            if (!string.IsNullOrEmpty(transactionId))
            {
                builder.SetTransactionId(transactionId);
            }

            if (!string.IsNullOrEmpty(interactionType) && !string.IsNullOrEmpty(interactionId))
            {
                builder.SetInteraction(interactionType, interactionId);
            }

            if (customEvent.PropertyList != null)
            {
                foreach (dynamic property in customEvent.PropertyList)
                {
                    if (string.IsNullOrEmpty(property.name))
                    {
                        continue;
                    }

                    builder.AddProperty(property.name, property.value);
                }
            }

            UAirship.Shared().Analytics.AddEvent(builder.Build());
        }

        public void TrackScreen(string screen) => UAirship.Shared().Analytics.TrackScreen(screen);

        public void AssociateIdentifier(string key, string identifier)
        {
            if (identifier == null)
            {
                UAirship.Shared().Analytics.EditAssociatedIdentifiers().RemoveIdentifier(key).Apply();
            }
            else
            {
                UAirship.Shared().Analytics.EditAssociatedIdentifiers().AddIdentifier(key, identifier).Apply();
            }
        }

        public void DisplayMessageCenter() => MessageCenterClass.Shared().ShowMessageCenter();

        public void DisplayMessage(string messageId) => MessageCenterClass.Shared().ShowMessageCenter(messageId);

        public void MarkMessageRead(string messageId) => MessageCenterClass.Shared().Inbox.MarkMessagesRead(new List<String> { messageId });

        public void DeleteMessage(string messageId) => MessageCenterClass.Shared().Inbox.DeleteMessages(new List<String> { messageId });

        public void MessageCenterUnreadCount(Action<int> messageCount) => messageCount(MessageCenterClass.Shared().Inbox.UnreadCount);

        public void MessageCenterCount(Action<int> messageCount) => messageCount(MessageCenterClass.Shared().Inbox.Count);

        public void InboxMessages(Action<List<MessageCenter.Message>> listMessages)
        {
            var messagesList = new List<MessageCenter.Message>();
            var messages = MessageCenterClass.Shared().Inbox.Messages;
            foreach (var message in messages)
            {
                var extras = new Dictionary<string, string>();
                foreach (var key in message.Extras.KeySet())
                {
                    extras.Add(key, message.Extras.Get(key).ToString());
                }

                DateTime? sentDate = FromDate(message.SentDate);
                DateTime? expirationDate = FromDate(message.ExpirationDate);

                var inboxMessage = new MessageCenter.Message(
                    message.MessageId,
                    message.Title,
                    sentDate,
                    expirationDate,
                    message.IsRead,
                    message.ListIconUrl,
                    extras);

                messagesList.Add(inboxMessage);
            }

            listMessages(messagesList);
        }

        private Date FromDateTime(DateTime dateTime)
        {
            if (dateTime == null)
            {
                return null;
            }
            long epochSeconds = new DateTimeOffset((DateTime)dateTime).ToUnixTimeSeconds();
            return new Date(epochSeconds * 1000);
        }

        private static DateTime? FromDate(Date date)
        {
            if (date == null)
            {
                return null;
            }
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(date.Time);
        }

        public Channel.TagGroupsEditor EditContactTagGroups()
        {
            return new Channel.TagGroupsEditor((List<Channel.TagGroupsEditor.TagOperation> payload) =>
            {
                var editor = UAirship.Shared().Contact.EditTagGroups();
                TagGroupHelper(payload, editor);
                editor.Apply();
            });
        }

        public Channel.TagGroupsEditor EditChannelTagGroups()
        {
            return new Channel.TagGroupsEditor((List<Channel.TagGroupsEditor.TagOperation> payload) =>
            {
                var editor = UAirship.Shared().Channel.EditTagGroups();
                TagGroupHelper(payload, editor);
                editor.Apply();
            });
        }

        public AttributeEditor EditAttributes() => EditChannelAttributes();

        public AttributeEditor EditChannelAttributes() =>
            new((List<AttributeEditor.IAttributeOperation> operations) =>
            {
                var editor = UAirship.Shared().Channel.EditAttributes();
                ApplyAttributesOperations(editor, operations);
                editor.Apply();
            });

        /// <summary>
        public AttributeEditor EditContactAttributes() =>
            new((List<AttributeEditor.IAttributeOperation> operations) =>
            {
                var editor = UAirship.Shared().Contact.EditAttributes();
                ApplyAttributesOperations(editor, operations);
                editor.Apply();
            });

        public ChannelSubscriptionListEditor EditChannelSubscriptionLists()
        {
            return new Channel.SubscriptionListEditor((List<Channel.SubscriptionListEditor.SubscriptionListOperation> payload) =>
            {
                var editor = UAirship.Shared().Channel.EditSubscriptionLists();
                ApplyChannelSubscriptionListHelper(payload, editor);
                editor.Apply();
            });
        }

        public ContactSubscriptionListEditor EditContactSubscriptionLists()
        {
            return new Contact.SubscriptionListEditor((List<Contact.SubscriptionListEditor.SubscriptionListOperation> payload) =>
            {
                //var editor = UAirship.Shared().Contact.EditSubscriptionLists();
                ApplyContactSubscriptionListHelper(payload);
                //editor.Apply();
            });
        }

        private void ApplyAttributesOperations(UrbanAirship.Channel.AttributeEditor editor, List<AttributeEditor.IAttributeOperation> operations)
        {
            foreach (var operation in operations)
            {
                if (operation is AttributeEditor.SetAttributeOperation<string> stringOperation)
                {
                    editor.SetAttribute(stringOperation.Key, stringOperation.Value);
                }

                if (operation is AttributeEditor.SetAttributeOperation<int> intOperation)
                {
                    editor.SetAttribute(intOperation.Key, intOperation.Value);
                }

                if (operation is AttributeEditor.SetAttributeOperation<long> longOperation)
                {
                    editor.SetAttribute(longOperation.Key, longOperation.Value);
                }

                if (operation is AttributeEditor.SetAttributeOperation<float> floatOperation)
                {
                    editor.SetAttribute(floatOperation.Key, floatOperation.Value);
                }

                if (operation is AttributeEditor.SetAttributeOperation<double> doubleOperation)
                {
                    editor.SetAttribute(doubleOperation.Key, doubleOperation.Value);
                }

                if (operation is AttributeEditor.SetAttributeOperation<DateTime> dateOperation)
                {
                    var date = FromDateTime(dateOperation.Value);
                    editor.SetAttribute(dateOperation.Key, date!);
                }

                if (operation is AttributeEditor.RemoveAttributeOperation removeOperation)
                {
                    editor.RemoveAttribute(removeOperation.Key);
                }
            }
        }

        private static void TagGroupHelper(List<Channel.TagGroupsEditor.TagOperation> payload, TagGroupsEditor editor)
        {
            foreach (Channel.TagGroupsEditor.TagOperation tagOperation in payload)
            {

                switch (tagOperation.operationType)
                {
                    case Channel.TagGroupsEditor.OperationType.ADD:
                        editor.AddTags(tagOperation.group, tagOperation.tags);
                        break;
                    case Channel.TagGroupsEditor.OperationType.REMOVE:
                        editor.RemoveTags(tagOperation.group, tagOperation.tags);
                        break;
                    case Channel.TagGroupsEditor.OperationType.SET:
                        editor.SetTags(tagOperation.group, tagOperation.tags);
                        break;
                    default:
                        break;
                }
            }
        }

        private void ApplyChannelSubscriptionListHelper(List<Channel.SubscriptionListEditor.SubscriptionListOperation> operations, UrbanAirship.Channel.SubscriptionListEditor editor)
        {
            foreach (Channel.SubscriptionListEditor.SubscriptionListOperation operation in operations)
            {
                if (!Enum.IsDefined(typeof(Channel.SubscriptionListEditor.OperationType), operation.OperationType))
                {
                    continue;
                }

                switch (operation.OperationType)
                {
                    case Channel.SubscriptionListEditor.OperationType.SUBSCRIBE:
                        editor.Subscribe(operation.List);
                        break;
                    case Channel.SubscriptionListEditor.OperationType.UNSUBSCRIBE:
                        editor.Unsubscribe(operation.List);
                        break;
                }
            }
        }

        // FIXME:
        //private void ApplyContactSubscriptionListHelper(List<Contact.SubscriptionListEditor.SubscriptionListOperation> operations, ScopedSubscriptionListEditor editor)
        private void ApplyContactSubscriptionListHelper(List<Contact.SubscriptionListEditor.SubscriptionListOperation> operations)
        {

            foreach (Contact.SubscriptionListEditor.SubscriptionListOperation operation in operations)
            {
                if (!Enum.IsDefined(typeof(Contact.SubscriptionListEditor.OperationType), operation.OperationType))
                {
                    continue;
                }

                //string scope = operation.scope;
                //string[] scopes = { "app", "web", "email", "sms" };
                //if (scopes.Any(scope.Contains))
                //{
                //    Scope channelScope = Scope.App;
                //    if (operation.Scope == "app")
                //    {
                //        channelScope = Scope.App;
                //    }
                //    else if (operation.scope == "web")
                //    {
                //        channelScope = Scope.Web;
                //    }
                //    else if (operation.Scope == "email")
                //    {
                //        channelScope = Scope.Email;
                //    }
                //    else if (operation.Scope == "sms")
                //    {
                //        channelScope = Scope.Sms;
                //    }

                //    switch (operation.OperationType)
                //    {
                //        case Contact.SubscriptionListEditor.OperationType.SUBSCRIBE:
                //            editor.Subscribe(operation.List, channelScope);
                //            break;
                //        case Contact.SubscriptionListEditor.OperationType.UNSUBSCRIBE:
                //            editor.Unsubscribe(operation.List, channelScope);
                //            break;
                //    }
                //}
            }
        }

        public bool InAppAutomationEnabled
        {
            get
            {
                return InAppAutomation.Shared().Enabled;
            }

            set
            {
                InAppAutomation.Shared().Enabled = value;
            }
        }

        public bool InAppAutomationPaused
        {
            get => InAppAutomation.Shared().Paused;
            set => InAppAutomation.Shared().Paused = value;
        }

        public TimeSpan InAppAutomationDisplayInterval
        {
            get => TimeSpan.FromMilliseconds(InAppAutomation.Shared().InAppMessageManager!.DisplayInterval);
            set => InAppAutomation.Shared().InAppMessageManager!.SetDisplayInterval((long)value.TotalMilliseconds, TimeUnit.Milliseconds!);
        }

        public bool OnDeepLink(string deepLink)
        {
            if (onDeepLinkReceived != null)
            {
                onDeepLinkReceived(this, new DeepLinkEventArgs(deepLink));
                return true;
            }

            return false;
        }

        public bool OnShowMessageCenter(string? messageId)
        {
            if (onMessageCenterDisplay != null)
            {
                onMessageCenterDisplay(this, new MessageCenterEventArgs(messageId));
                return true;
            }

            return false;
        }

        public void OnInboxUpdated() => OnMessageCenterUpdated?.Invoke(this, EventArgs.Empty);

        public void OnChannelCreated(string channelId) => OnChannelCreation?.Invoke(this, new ChannelEventArgs(channelId));

        public void OnChange(PushNotificationStatus status) => OnPushNotificationStatusUpdate?.Invoke(this,
            new PushNotificationStatusEventArgs(
                status.IsUserNotificationsEnabled,
                status.AreNotificationsAllowed,
                status.IsPushPrivacyFeatureEnabled,
                status.IsPushTokenRegistered,
                status.IsUserOptedIn,
                status.IsOptIn)
            );
    }
}