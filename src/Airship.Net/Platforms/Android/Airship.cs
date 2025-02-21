/* Copyright Airship and Contributors */

using Com.Urbanairship.Contacts;
using Java.Util;
using Java.Util.Concurrent;
using UrbanAirship;
using UrbanAirship.Automation;
using UrbanAirship.Actions;
using UrbanAirship.Channel;
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
                // TODO(SDK19): why is this not accessible?
                //MessageCenterClass.Shared().SetOnShowMessageCenterListener(this);
            }

            remove
            {
                onMessageCenterDisplay -= value;

                if (onMessageCenterDisplay == null)
                {
                    // TODO(SDK19): why is this not accessible?
                    // MessageCenterClass.Shared().SetOnShowMessageCenterListener(null);
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
            get => FeaturesFromUAFeatures(UAirship.Shared().PrivacyManager.EnabledFeatures.ToArray<PrivacyManager.Feature>()!);
            set => UAirship.Shared().PrivacyManager.SetEnabledFeatures(UaFeaturesFromFeatures(value));
        }

        public void EnableFeatures(Features features) => UAirship.Shared().PrivacyManager.Enable(UaFeaturesFromFeatures(features));

        public void DisableFeatures(Features features) => UAirship.Shared().PrivacyManager.Disable(UaFeaturesFromFeatures(features));

        public bool IsFeatureEnabled(Features feature) => EnabledFeatures.HasFlag(feature);

        public bool IsAnyFeatureEnabled() => EnabledFeatures != Features.None;

        private static PrivacyManager.Feature[] UaFeaturesFromFeatures(Features features)
        {
            List<PrivacyManager.Feature> uAFeatures = new();

            if (features.HasFlag(Features.InAppAutomation))
            {
                uAFeatures.Add(PrivacyManager.Feature.InAppAutomation);
            }
            if (features.HasFlag(Features.MessageCenter))
            {
                uAFeatures.Add(PrivacyManager.Feature.MessageCenter);
            }
            if (features.HasFlag(Features.Push))
            {
                uAFeatures.Add(PrivacyManager.Feature.Push);
            }
            if (features.HasFlag(Features.Analytics))
            {
                uAFeatures.Add(PrivacyManager.Feature.Analytics);
            }
            if (features.HasFlag(Features.TagsAndAttributes))
            {
                uAFeatures.Add(PrivacyManager.Feature.TagsAndAttributes);
            }
            if (features.HasFlag(Features.Contacts))
            {
                uAFeatures.Add(PrivacyManager.Feature.Contacts);
            }

            return uAFeatures.ToArray();
        }

        private static Features FeaturesFromUAFeatures(PrivacyManager.Feature[] uAFeatures)
        {
            Features features = Features.None;

            if (uAFeatures.Contains(PrivacyManager.Feature.InAppAutomation))
            {
                features |= Features.InAppAutomation;
            }

            if (uAFeatures.Contains(PrivacyManager.Feature.MessageCenter))
            {
                features |= Features.MessageCenter;
            }

            if (uAFeatures.Contains(PrivacyManager.Feature.Push))
            {
                features |= Features.Push;
            }

            if (uAFeatures.Contains(PrivacyManager.Feature.Analytics))
            {
                features |= Features.Analytics;
            }

            if (uAFeatures.Contains(PrivacyManager.Feature.TagsAndAttributes))
            {
                features |= Features.TagsAndAttributes;
            }

            if (uAFeatures.Contains(PrivacyManager.Feature.Contacts))
            {
                features |= Features.Contacts;
            }

            return features;
        }

        public IEnumerable<string> Tags => UAirship.Shared().Channel.Tags;

        private class ResultCallback : Java.Lang.Object, IResultCallback
        {
            Action<Java.Lang.Object?> action;

            internal ResultCallback(Action<Java.Lang.Object?> action)
            {
                this.action = action;
            }

            public void OnResult(Java.Lang.Object? result)
            {
                action.Invoke(result);
            }
        }

        private List<string> CastHashSetToList(HashSet set)
        {
            var list = new List<string>();

            var value = set.Iterator();
            if (value is not null)
            {
                while (value.HasNext)
                {
                    var nextValue = (string?)value.Next();
                    if (nextValue is not null)
                    {
                        list.Add(nextValue);
                    }
                }
            }
            return list;
        }

        public void FetchChannelSubscriptionLists(Action<List<string>> subscriptions)
        {
            PendingResult subscriptionsPendingResult = UAirship.Shared().Channel.FetchSubscriptionListsPendingResult();

            subscriptionsPendingResult.AddResultCallback(new ResultCallback((result) =>
            {
                var list = new List<string>();
                if (result is HashSet)
                {
                    list = CastHashSetToList((HashSet)result);
                }

                subscriptions(list);
            }));
        }

        public void FetchContactSubscriptionLists(Action<Dictionary<string, List<string>>> subscriptions)
        {
            PendingResult subscriptionsPendingResult = UAirship.Shared().Contact.FetchSubscriptionListsPendingResult();

            subscriptionsPendingResult.AddResultCallback(new ResultCallback((result) =>
            {
                Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
                if (result is not null)
                {
                    var typedResult = (HashMap)result;
                    foreach (string? key in typedResult.KeySet())
                    {
                        if (key is not null)
                        {
                            var typedValue = typedResult.Get(key);

                            if (typedValue is not null && typedValue is HashSet)
                            {
                                var list = CastHashSetToList((HashSet)typedValue);
                                dictionary.Add(key, list);
                            }
           
                        }
                    }
                }
                subscriptions(dictionary);
            }));
        }

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
                    if (string.IsNullOrEmpty(property.Name))
                    {
                        continue;
                    }

                    builder.AddProperty(property.Name, property.Value);
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

        public void MessageCenterUnreadCount(Action<int> messageCount) => MessageCenterClass.Shared().Inbox.UnreadCount(messageCount);

        public void MessageCenterCount(Action<int> messageCount) => MessageCenterClass.Shared().Inbox.Count(messageCount);

        public void InboxMessages(Action<List<MessageCenter.Message>> listMessages)
        {
            Console.WriteLine("--- INBOX MESSAGES!!!");
            
            MessageCenterClass.Shared().Inbox.GetMessages(messages =>
            {
                Console.WriteLine("--- GOT PENDING RESULT!!!");
                
                var messagesList = new List<MessageCenter.Message>();

                foreach (var message in messages)
                {
                    Console.WriteLine("--- MESSAGE: " + message.Id);
                    var extras = new Dictionary<string, string?>();
                    foreach (var key in message.Extras.Keys)
                    {
                        extras.Add(key, message.Extras[key]);
                    }

                    DateTime? sentDate = FromDate(message.SentDate);
                    DateTime? expirationDate = FromDate(message.ExpirationDate);

                    var inboxMessage = new MessageCenter.Message(
                        message.Id,
                        message.Title,
                        sentDate,
                        expirationDate,
                        message.IsRead,
                        message.ListIconUrl,
                        extras);

                    messagesList.Add(inboxMessage);
                }
                
                listMessages(messagesList);
            });
        }

        private Date FromDateTime(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return null;
            }
            long epochSeconds = new DateTimeOffset((DateTime)dateTime).ToUnixTimeSeconds();
            return new Date(epochSeconds * 1000);
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

        public Channel.TagGroupsEditor EditContactTagGroups()
        {
            return new Channel.TagGroupsEditor(payload =>
            {
                var editor = UAirship.Shared().Contact.EditTagGroups();
                TagGroupHelper(payload, editor);
                editor.Apply();
            });
        }

        public Channel.TagGroupsEditor EditChannelTagGroups()
        {
            return new Channel.TagGroupsEditor( payload =>
            {
                var editor = UAirship.Shared().Channel.EditTagGroups();
                TagGroupHelper(payload, editor);
                editor.Apply();
            });
        }

        public AttributeEditor EditAttributes() => EditChannelAttributes();

        public AttributeEditor EditChannelAttributes() =>
            new(operations =>
            {
                var editor = UAirship.Shared().Channel.EditAttributes();
                ApplyAttributesOperations(editor, operations);
                editor.Apply();
            });

        public AttributeEditor EditContactAttributes() =>
            new(operations =>
            {
                var editor = UAirship.Shared().Contact.EditAttributes();
                ApplyAttributesOperations(editor, operations);
                editor.Apply();
            });

        public ChannelSubscriptionListEditor EditChannelSubscriptionLists()
        {
            return new ChannelSubscriptionListEditor(payload =>
            {
                var editor = UAirship.Shared().Channel.EditSubscriptionLists();
                ApplyChannelSubscriptionListHelper(payload, editor);
                editor.Apply();
            });
        }

        public ContactSubscriptionListEditor EditContactSubscriptionLists()
        {
            return new ContactSubscriptionListEditor(payload =>
            {
                var editor = UAirship.Shared().Contact.EditSubscriptionLists();
                ApplyContactSubscriptionListHelper(payload, editor);
                editor.Apply();
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
            foreach (var tagOperation in payload)
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
                }
            }
        }

        private void ApplyChannelSubscriptionListHelper(List<ChannelSubscriptionListEditor.SubscriptionListOperation> operations, UrbanAirship.Channel.SubscriptionListEditor editor)
        {
            foreach (var operation in operations)
            {
                if (!Enum.IsDefined(typeof(ChannelSubscriptionListEditor.OperationType), operation.OperationType))
                {
                    continue;
                }

                switch (operation.OperationType)
                {
                    case ChannelSubscriptionListEditor.OperationType.SUBSCRIBE:
                        editor.Subscribe(operation.List);
                        break;
                    case ChannelSubscriptionListEditor.OperationType.UNSUBSCRIBE:
                        editor.Unsubscribe(operation.List);
                        break;
                }
            }
        }

        private void ApplyContactSubscriptionListHelper(List<ContactSubscriptionListEditor.SubscriptionListOperation> operations, ScopedSubscriptionListEditor editor)
        {
            foreach (var operation in operations)
            {
                if (!Enum.IsDefined(typeof(ContactSubscriptionListEditor.OperationType), operation.OperationType))
                {
                    continue;
                }

                string scope = operation.Scope;
                string[] scopes = { "app", "web", "email", "sms" };
                if (scopes.Contains(scope))
                {
                    Scope channelScope = Scope.App;

                    if (operation.Scope == "app")
                    {
                        channelScope = Scope.App;
                    }
                    else if (operation.Scope == "web")
                    {
                        channelScope = Scope.Web;
                    }
                    else if (operation.Scope == "email")
                    {
                        channelScope = Scope.Email;
                    }
                    else if (operation.Scope == "sms")
                    {
                        channelScope = Scope.Sms;
                    }
                    
                    switch (operation.OperationType)
                    {
                        case Contact.SubscriptionListEditor.OperationType.SUBSCRIBE:
                            editor.Subscribe(operation.List, channelScope);
                            break;
                        case Contact.SubscriptionListEditor.OperationType.UNSUBSCRIBE:
                            editor.Unsubscribe(operation.List, channelScope);
                            break;
                    }
                }
            }
        }

        public bool InAppAutomationEnabled
        {
            get => IsFeatureEnabled(Features.InAppAutomation);
            set
            {
                if (value)
                {
                    EnableFeatures(Features.InAppAutomation);
                }
                else
                {
                    DisableFeatures(Features.InAppAutomation);
                }
            }
        }

        public bool InAppAutomationPaused
        {
            get => InAppAutomation.Shared().Paused;
            set => InAppAutomation.Shared().Paused = value;
        }

        public TimeSpan InAppAutomationDisplayInterval
        {
            get => TimeSpan.FromMilliseconds(InAppAutomation.Shared().InAppMessaging!.DisplayInterval);
            set => InAppAutomation.Shared().InAppMessaging!.DisplayInterval = (long)value.TotalMilliseconds;
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