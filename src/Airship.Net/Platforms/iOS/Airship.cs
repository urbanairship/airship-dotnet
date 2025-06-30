/* Copyright Airship and Contributors */

using Foundation;
using Airship;
using AirshipDotNet.Analytics;
using AirshipDotNet.Attributes;

namespace AirshipDotNet
{
    /// <summary>
    /// Provides cross-platform access to a common subset of functionality between the iOS and Android SDKs
    /// </summary>
    public class Airship : UADeepLinkDelegate, IAirship
    {
        private static readonly Lazy<Airship> sharedAirship = new(() =>
        {
            Airship instance = new();
            instance.Initialize();
            return instance;
        });

        private void Initialize()
        {
            // Load unreferenced modules
            // AirshipAutomation.Init() - not available in SDK 19 ObjectiveC bindings

            // TODO: SDK 19 Compatibility - UAirshipNotificationChannelCreated doesn't exist
            // Channel creation notification has been removed in SDK 19
            // Need to find alternative way to observe channel creation events
            /*
            NSNotificationCenter.DefaultCenter.AddObserver(aName: (NSString)UAirshipNotificationChannelCreated.Name, (notification) =>
            {
                string channelID = notification.UserInfo[UAirshipNotificationChannelCreated.ChannelIDKey].ToString();
                OnChannelCreation?.Invoke(this, new ChannelEventArgs(channelID));
            });
            */

            // Note: Push notification status update event is not directly available in SDK 19
            // This functionality may need to be implemented differently using the new SDK APIs

            //Adding Inbox updated Listener
            NSNotificationCenter.DefaultCenter.AddObserver(aName: (NSString)"com.urbanairship.notification.message_list_updated", (notification) =>
            {
                OnMessageCenterUpdated?.Invoke(this, EventArgs.Empty);
            });
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
                // TODO: SDK 19 Compatibility - UAirship.WeakDeepLinkDelegate is now an instance property
                // UAirship is now a static class and doesn't have WeakDeepLinkDelegate property
                // Deep link handling needs to be implemented differently in SDK 19
                // UAirship.WeakDeepLinkDelegate = this;
            }
            remove
            {
                onDeepLinkReceived -= value;

                if (onDeepLinkReceived == null)
                {
                    // TODO: SDK 19 Compatibility - UAirship.WeakDeepLinkDelegate is now an instance property
                    // UAirship is now a static class and doesn't have WeakDeepLinkDelegate property
                    // UAirship.WeakDeepLinkDelegate = null;
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
                // TODO: SDK 19 Compatibility - UAirship.MessageCenter doesn't exist
                // UAirship.MessageCenter.WeakDisplayDelegate = this;
            }
            remove
            {
                onMessageCenterDisplay -= value;

                if (onMessageCenterDisplay == null)
                {
                    // TODO: SDK 19 Compatibility - UAirship.MessageCenter doesn't exist
                    // UAirship.MessageCenter.WeakDisplayDelegate = null;
                }
            }
        }

        public static Airship Instance => sharedAirship.Value;

        public bool UserNotificationsEnabled
        {
            get => UAirship.Push.UserPushNotificationsEnabled;
            set => UAirship.Push.UserPushNotificationsEnabled = value;
        }

        public Features EnabledFeatures
        {
            get
            {
                return FeaturesFromUAFeature(UAirship.PrivacyManager.EnabledFeatures);
            }
            set
            {
                UAirship.PrivacyManager.EnabledFeatures = UAFeatureFromFeatures(value);
            }
        }

        public void EnableFeatures(Features features)
        {
            UAirship.PrivacyManager.EnableFeatures(UAFeatureFromFeatures(features));
        }

        public void DisableFeatures(Features features)
        {
            UAirship.PrivacyManager.DisableFeatures(UAFeatureFromFeatures(features));
        }

        public bool IsFeatureEnabled(Features feature) => EnabledFeatures.HasFlag(feature);
        
        public bool IsAnyFeatureEnabled() => EnabledFeatures != Features.None;

        /* TODO: UAFeature is now a class, not an enum. Need to update this logic
        private static UAFeatures UaFeaturesFromFeatures(Features features)
        {
            UAFeatures uAFeatures = UAFeatures.None;

            if (features.HasFlag(Features.InAppAutomation))
            {
                uAFeatures |= UAFeatures.InAppAutomation;
            }
            if (features.HasFlag(Features.MessageCenter))
            {
                uAFeatures |= UAFeatures.MessageCenter;
            }
            if (features.HasFlag(Features.Push))
            {
                uAFeatures |= UAFeatures.Push;
            }
            if (features.HasFlag(Features.Analytics))
            {
                uAFeatures |= UAFeatures.Analytics;
            }
            if (features.HasFlag(Features.TagsAndAttributes))
            {
                uAFeatures |= UAFeatures.TagsAndAttributes;
            }
            if (features.HasFlag(Features.Contacts))
            {
                uAFeatures |= UAFeatures.Contacts;
            }

            return uAFeatures;
        }

        private static Features FeaturesFromUAFeatures(UAFeatures uAFeatures)
        {
            Features features = Features.None;

            if (uAFeatures.HasFlag(UAFeatures.InAppAutomation))
            {
                features |= Features.InAppAutomation;
            }
            if (uAFeatures.HasFlag(UAFeatures.MessageCenter))
            {
                features |= Features.MessageCenter;
            }
            if (uAFeatures.HasFlag(UAFeatures.Push))
            {
                features |= Features.Push;
            }
            if (uAFeatures.HasFlag(UAFeatures.Analytics))
            {
                features |= Features.Analytics;
            }
            if (uAFeatures.HasFlag(UAFeatures.TagsAndAttributes))
            {
                features |= Features.TagsAndAttributes;
            }
            if (uAFeatures.HasFlag(UAFeatures.Contacts))
            {
                features |= Features.Contacts;
            }

            return features;
        }
        */

        private static UAFeature UAFeatureFromFeatures(Features features)
        {
            var featureList = new List<UAFeature>();

            if (features.HasFlag(Features.InAppAutomation))
            {
                featureList.Add(UAFeature.InAppAutomation);
            }
            if (features.HasFlag(Features.MessageCenter))
            {
                featureList.Add(UAFeature.MessageCenter);
            }
            if (features.HasFlag(Features.Push))
            {
                featureList.Add(UAFeature.Push);
            }
            if (features.HasFlag(Features.Analytics))
            {
                featureList.Add(UAFeature.Analytics);
            }
            if (features.HasFlag(Features.TagsAndAttributes))
            {
                featureList.Add(UAFeature.TagsAndAttributes);
            }
            if (features.HasFlag(Features.Contacts))
            {
                featureList.Add(UAFeature.Contacts);
            }

            if (featureList.Count == 0)
            {
                return UAFeature.None;
            }

            return new UAFeature(featureList.ToArray());
        }

        private static Features FeaturesFromUAFeature(UAFeature uaFeature)
        {
            Features features = Features.None;

            if (uaFeature.Contains(UAFeature.InAppAutomation))
            {
                features |= Features.InAppAutomation;
            }
            if (uaFeature.Contains(UAFeature.MessageCenter))
            {
                features |= Features.MessageCenter;
            }
            if (uaFeature.Contains(UAFeature.Push))
            {
                features |= Features.Push;
            }
            if (uaFeature.Contains(UAFeature.Analytics))
            {
                features |= Features.Analytics;
            }
            if (uaFeature.Contains(UAFeature.TagsAndAttributes))
            {
                features |= Features.TagsAndAttributes;
            }
            if (uaFeature.Contains(UAFeature.Contacts))
            {
                features |= Features.Contacts;
            }

            return features;
        }

        public IEnumerable<string> Tags
        {
            get
            {
                var tags = UAirship.Channel.Tags;
                return tags ?? new string[0];
            }
        }

        public void FetchChannelSubscriptionLists(Action<List<string>> subscriptions)
        {
            // TODO: SDK 19 Compatibility - UAChannel.FetchSubscriptionListsWithCompletionHandler doesn't exist
            // This method has been removed in SDK 19
            // Channel subscription lists functionality may need to be accessed differently
            /*
            UAirship.Channel.FetchSubscriptionListsWithCompletionHandler((lists, error) =>
            {
                var list = new List<string>();
                if (lists is not null)
                {
                    for (nuint i = 0; i < lists.Count; i++)
                    {
                        var subscription = lists.GetItem<NSString>(i);
                        list.Add(subscription.ToString());
                    }
                }
                subscriptions(list);
            });
            */
            subscriptions(new List<string>());
        }

        public void FetchContactSubscriptionLists(Action<Dictionary<string, List<String>>> subscriptions)
        {
            // TODO: FetchSubscriptionListsWithCompletionHandler not exposed in SDK 19 ObjectiveC bindings
            // This functionality needs to be added to the ObjectiveC framework
            subscriptions(new Dictionary<string, List<string>>());
        }

        private string ScopeOrdinalToString(NSNumber ordinal)
            => ordinal.LongValue switch
            {
                0 => "app",
                1 => "web",
                2 => "email",
                3 => "sms",
                _ => "unknown",
            };

        public string? ChannelId => UAirship.Channel.Identifier;

        public void GetNamedUser(Action<string> namedUser)
        {
            // TODO: GetNamedUserIDWithCompletionHandler not exposed in SDK 19 ObjectiveC bindings
            // This functionality needs to be added to the ObjectiveC framework
            namedUser("");
        }

        public void ResetContact()
        {
            // TODO: SDK 19 Compatibility - UAirship.Contact is now async
            // Need to handle async Contact access properly
            // UAirship.Contact.Reset();
        }

        public void IdentifyContact(string namedUserId)
        {
            // TODO: SDK 19 Compatibility - UAirship.Contact is now async
            // Need to handle async Contact access properly
            // UAirship.Contact.Identify(namedUserId);
        }

        public Channel.TagEditor EditDeviceTags() => new(DeviceTagHelper);

        private void DeviceTagHelper(bool clear, string[] addTags, string[] removeTags)
        {
            var editor = UAirship.Channel.EditTags;
            if (editor != null)
            {
                if (clear)
                {
                    editor.ClearTags();
                }
                if (addTags != null && addTags.Length > 0)
                {
                    editor.AddTags(addTags);
                }
                if (removeTags != null && removeTags.Length > 0)
                {
                    editor.RemoveTags(removeTags);
                }
                editor.Apply();
            }
        }

        public void AddCustomEvent(CustomEvent customEvent)
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

            // SDK 19 uses double directly in the constructor
            UACustomEvent uaEvent;
            if (eventValue.HasValue)
            {
                uaEvent = new UACustomEvent(eventName, eventValue.Value);
            }
            else
            {
                // Single parameter constructor for events without value
                uaEvent = new UACustomEvent(eventName);
            }

            if (!string.IsNullOrEmpty(transactionId))
            {
                uaEvent.TransactionID = transactionId;
            }

            if (!string.IsNullOrEmpty(interactionId))
            {
                uaEvent.InteractionID = interactionId;
            }

            if (!string.IsNullOrEmpty(interactionType))
            {
                uaEvent.InteractionType = interactionType;
            }

            if (customEvent.PropertyList != null)
            {
                NSMutableDictionary<NSString, NSObject> propertyDictionary = new();
                foreach (dynamic property in customEvent.PropertyList)
                {
                    if (string.IsNullOrEmpty(property.Name))
                    {
                        continue;
                    }

                    NSString key = (NSString)property.Name;
                    NSObject? value = NSObject.FromObject(property.Value);

                    if (property is CustomEvent.Property<string[]> stringArrayProperty)
                    {
                        value = NSArray.FromObjects(stringArrayProperty.Value);
                    }
                    if (value != null)
                    {
                        propertyDictionary.SetValueForKey(value, key);
                    }

                }
                // TODO: SDK 19 Compatibility - SetPropertyWithBool/Double/String methods don't exist
                // UACustomEvent now uses a different API for setting properties
                // The Properties property might be available, or need to use a different approach
                /*
                foreach (var kvp in propertyDictionary)
                {
                    var key = kvp.Key.ToString();
                    var value = kvp.Value;
                    
                    if (value is NSNumber number)
                    {
                        if (number.ObjCType == "c") // bool
                            uaEvent.SetPropertyWithBool(number.BoolValue, key);
                        else
                            uaEvent.SetPropertyWithDouble(number.DoubleValue, key);
                    }
                    else if (value is NSString str)
                    {
                        uaEvent.SetPropertyWithString(str.ToString(), key);
                    }
                }
                */
                // For now, properties are not being set due to API changes
            }

            // TODO: SDK 19 Compatibility - UAAnalytics.RecordCustomEvent doesn't exist
            // The method has been renamed or moved in SDK 19
            // Need to find the new API for recording custom events
            // UAirship.Analytics.RecordCustomEvent(uaEvent);
        }

        public void TrackScreen(string screen) => UAirship.Analytics.TrackScreen(screen);

        public void AssociateIdentifier(string key, string identifier)
        {
            // TODO: SDK 19 Compatibility - UAAnalytics.AssociateDeviceIdentifier doesn't exist
            // Need to find new API for associating device identifiers
            /*
            UAAssociatedIdentifiers identifiers = UAirship.Analytics.CurrentAssociatedDeviceIdentifiers;
            identifiers.SetWithIdentifier(identifier, key);
            UAirship.Analytics.AssociateDeviceIdentifier(identifiers);
            */
        }


        public void DisplayMessageCenter()
        {
            UAirship.MessageCenter.Display();
        }

        public void DisplayMessage(string messageId)
        {
            UAirship.MessageCenter.DisplayWithMessageID(messageId);
        }

        public void MarkMessageRead(string messageId)
        {
            string[] toRead = { messageId };
            UAirship.MessageCenter.Inbox.MarkReadWithMessageIDs(toRead, () => { });
        }

        public void DeleteMessage(string messageId)
        {
            string[] toDelete = { messageId };
            UAirship.MessageCenter.Inbox.DeleteWithMessageIDs(toDelete, () => { });
        }

        public void MessageCenterUnreadCount(Action<int> messageCount)
        {
            UAirship.MessageCenter.Inbox.GetUnreadCountSync(count => messageCount((int)count));
        }

        public void MessageCenterCount(Action<int> messageCount)
        {
            UAirship.MessageCenter.Inbox.GetMessagesSync(messages =>
            {
                messageCount(messages?.Length ?? 0);
            });
        }

        public void InboxMessages(Action<List<MessageCenter.Message>> listMessages)
        {
            var messagesList = new List<MessageCenter.Message>();
            UAirship.MessageCenter.Inbox.GetMessagesSync(messages =>
            {
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

                        var inboxMessage = new MessageCenter.Message(
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

                listMessages(messagesList);
            });
        }

        private static NSDate? FromDateTime(DateTime? dateTime)
        {
            if (dateTime is null)
            {
                return null;
            }

            if (dateTime.Value.Kind == DateTimeKind.Unspecified)
            {
                dateTime = DateTime.SpecifyKind((DateTime)dateTime, DateTimeKind.Utc);
            }

            return (NSDate)dateTime;
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


        public Channel.TagGroupsEditor EditChannelTagGroups()
        {
            return new(payload =>
            {
                ChannelTagGroupHelper(payload, () =>
                {
                    // UpdateRegistration no longer needed in SDK 19
                });
            });
        }

        public Channel.TagGroupsEditor EditContactTagGroups()
        {
            return new(payload =>
            {
                ContactTagGroupHelper(payload);
            });
        }


        public AttributeEditor EditAttributes() => EditChannelAttributes();

        public AttributeEditor EditChannelAttributes()
        {
            return new(operations =>
            {
                ApplyChannelAttributesOperations(operations);
            });
        }

        public AttributeEditor EditContactAttributes()
        {
            return new(operations =>
            {
                ApplyContactAttributesOperations(operations);
            });
        }


        public Channel.SubscriptionListEditor EditChannelSubscriptionLists()
        {
            return new Channel.SubscriptionListEditor(payload =>
            {
                ApplyChannelSubscriptionListHelper(payload);
            });
        }

        public Contact.SubscriptionListEditor EditContactSubscriptionLists()
        {
            return new Contact.SubscriptionListEditor(payload =>
            {
                ApplyContactSubscriptionListHelper(payload);
            });
        }

        private void ApplyChannelAttributesOperations(List<AttributeEditor.IAttributeOperation> operations)
        {
            // TODO: SDK 19 Compatibility - UAChannel.EditAttributesWithBlock doesn't exist
            // Need to use new EditAttributes API pattern
            /*
            UAirship.Channel.EditAttributesWithBlock(editor =>
            {
                foreach (var operation in operations)
            {
                if (operation is AttributeEditor.SetAttributeOperation<string> stringOperation)
                {
                    editor.SetString(stringOperation.Value, stringOperation.Key);
                }

                if (operation is AttributeEditor.SetAttributeOperation<int> intOperation)
                {
                    editor.SetNumber(intOperation.Value, intOperation.Key);
                }

                if (operation is AttributeEditor.SetAttributeOperation<long> longOperation)
                {
                    editor.SetNumber(longOperation.Value, longOperation.Key);
                }

                if (operation is AttributeEditor.SetAttributeOperation<float> floatOperation)
                {
                    editor.SetNumber(floatOperation.Value, floatOperation.Key);
                }

                if (operation is AttributeEditor.SetAttributeOperation<double> doubleOperation)
                {
                    editor.SetNumber(doubleOperation.Value, doubleOperation.Key);
                }

                if (operation is AttributeEditor.SetAttributeOperation<DateTime> dateOperation)
                {
                    NSDate date = FromDateTime(dateOperation.Value);
                    editor.SetDate(date, dateOperation.Key);
                }

                if (operation is AttributeEditor.RemoveAttributeOperation removeOperation)
                {
                    editor.RemoveAttribute(removeOperation.Key);
                }
            }
            editor.Apply();
            });
            */
        }

        private void ApplyContactAttributesOperations(List<AttributeEditor.IAttributeOperation> operations)
        {
            // TODO: SDK 19 Compatibility - UAirship.Contact is now async
            // Need to handle async Contact access properly
            /*
            var editor = UAirship.Contact.EditAttributes;
            foreach (var operation in operations)
            {
                if (operation is AttributeEditor.SetAttributeOperation<string> stringOperation)
                {
                    editor.SetString(stringOperation.Value, stringOperation.Key);
                }

                if (operation is AttributeEditor.SetAttributeOperation<int> intOperation)
                {
                    editor.SetNumber(intOperation.Value, intOperation.Key);
                }

                if (operation is AttributeEditor.SetAttributeOperation<long> longOperation)
                {
                    editor.SetNumber(longOperation.Value, longOperation.Key);
                }

                if (operation is AttributeEditor.SetAttributeOperation<float> floatOperation)
                {
                    editor.SetNumber(floatOperation.Value, floatOperation.Key);
                }

                if (operation is AttributeEditor.SetAttributeOperation<double> doubleOperation)
                {
                    editor.SetNumber(doubleOperation.Value, doubleOperation.Key);
                }

                if (operation is AttributeEditor.SetAttributeOperation<DateTime> dateOperation)
                {
                    NSDate date = FromDateTime(dateOperation.Value);
                    editor.SetDate(date, dateOperation.Key);
                }

                if (operation is AttributeEditor.RemoveAttributeOperation removeOperation)
                {
                    editor.RemoveAttribute(removeOperation.Key);
                }
            }
            editor.Apply();
            */
        }

        private void ContactTagGroupHelper(List<Channel.TagGroupsEditor.TagOperation> operations)
        {
            // TODO: SDK 19 Compatibility - UAContact.EditTagGroupsWithBlock doesn't exist
            // Need to use new EditTagGroups API pattern
            /*
            UAirship.Contact.EditTagGroupsWithBlock(editor =>
            {
                var contactActions = new Dictionary<Channel.TagGroupsEditor.OperationType, Action<string, string[]>>()
                {
                    { Channel.TagGroupsEditor.OperationType.ADD, (group, t) => editor.AddTags(t, group) },
                    { Channel.TagGroupsEditor.OperationType.REMOVE, (group, t) => editor.RemoveTags(t, group) },
                    { Channel.TagGroupsEditor.OperationType.SET, (group, t) => editor.SetTags(t, group) }
                };

                foreach (Channel.TagGroupsEditor.TagOperation operation in operations)
                {
                    if (!Enum.IsDefined(typeof(Channel.TagGroupsEditor.OperationType), operation.operationType))
                    {
                        continue;
                    }

                    string[] tagArray = new string[operation.tags.Count];
                    operation.tags.CopyTo(tagArray, 0);
                    contactActions[operation.operationType](operation.group, tagArray);
                }

                editor.Apply();
            });
            */
        }

        private void ChannelTagGroupHelper(List<Channel.TagGroupsEditor.TagOperation> operations, Action finished)
        {
            // TODO: SDK 19 Compatibility - UAChannel doesn't have EditTagGroupsWithBlock
            // Channel tag groups functionality may have been moved or removed
            /*
            UAirship.Channel.EditTagGroupsWithBlock(editor =>
            {
                var channelActions = new Dictionary<Channel.TagGroupsEditor.OperationType, Action<string, string[]>>()
                {
                    { Channel.TagGroupsEditor.OperationType.ADD, (group, t) => editor.AddTags(t, group) },
                    { Channel.TagGroupsEditor.OperationType.REMOVE, (group, t) => editor.RemoveTags(t, group) },
                    { Channel.TagGroupsEditor.OperationType.SET, (group, t) => editor.SetTags(t, group) }
                };

                foreach (Channel.TagGroupsEditor.TagOperation operation in operations)
                {
                    if (!Enum.IsDefined(typeof(Channel.TagGroupsEditor.OperationType), operation.operationType))
                    {
                        continue;
                    }

                    string[] tagArray = new string[operation.tags.Count];
                    operation.tags.CopyTo(tagArray, 0);
                    channelActions[operation.operationType](operation.group, tagArray);
                }

                editor.Apply();
                finished();
            });
            */
            finished();
        }

        private void ApplyChannelSubscriptionListHelper(List<Channel.SubscriptionListEditor.SubscriptionListOperation> operations)
        {
            // TODO: SDK 19 Compatibility - UAChannel.EditSubscriptionListsWithBlock doesn't exist
            // Need to use new EditSubscriptionLists API pattern
            /*
            UAirship.Channel.EditSubscriptionListsWithBlock(editor =>
            {
                foreach (var operation in operations)
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

                editor.Apply();
            });
            */
        }

        private void ApplyContactSubscriptionListHelper(List<Contact.SubscriptionListEditor.SubscriptionListOperation> operations)
        {
            // TODO: SDK 19 Compatibility - UAContact.EditSubscriptionListsWithBlock doesn't exist
            // Need to use new EditSubscriptionLists API pattern
            /*
            UAirship.Contact.EditSubscriptionListsWithBlock(editor =>
            {

                foreach (var operation in operations)
                {
                    if (!Enum.IsDefined(typeof(Contact.SubscriptionListEditor.OperationType), operation.OperationType))
                    {
                        continue;
                    }

                    string scope = operation.Scope;
                    string[] scopes = { "app", "web", "email", "sms" };
                    if (scopes.Any(scope.Contains))
                    {
                        UAChannelScope channelScope = UAChannelScope.App;
                        if (operation.Scope == "app")
                        {
                            channelScope = UAChannelScope.App;
                        }
                        else if (operation.Scope == "web")
                        {
                            channelScope = UAChannelScope.Web;
                        }
                        else if (operation.Scope == "email")
                        {
                            channelScope = UAChannelScope.Email;
                        }
                        else if (operation.Scope == "sms")
                        {
                            channelScope = UAChannelScope.Sms;
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

                editor.Apply();
            });
            */
        }

        public void ReceivedDeepLink(NSUrl deepLink, Action completionHandler)
        {
            onDeepLinkReceived?.Invoke(this, new DeepLinkEventArgs(deepLink.AbsoluteString!));
            completionHandler();
        }

        // TODO: SDK 19 Compatibility - IUAMessageCenterDisplayDelegate causes type encoding crash
        // "Unsupported type encoding: <v@?@"NSError">16"
        // Message center delegate functionality temporarily disabled
        /*
        // IUAMessageCenterDisplayDelegate implementation
        public void DisplayMessageCenterForMessageID(string messageID) => onMessageCenterDisplay?.Invoke(this, new MessageCenterEventArgs(messageID));

        public void DisplayMessageCenter() => onMessageCenterDisplay?.Invoke(this, new MessageCenterEventArgs());

        public void DismissMessageCenter()
        {
            // Handle message center dismissal if needed
        }
        */

        public bool InAppAutomationPaused
        {
            get
            {
                // TODO: Paused property not exposed in SDK 19 ObjectiveC bindings
                // TEMPORARY WORKAROUND - See: /Context/SDK19-TEMPORARY-WORKAROUNDS.md section 7
                return false;
            }
            set
            {
                // TODO: Paused property not exposed in SDK 19 ObjectiveC bindings
                // TEMPORARY WORKAROUND - See: /Context/SDK19-TEMPORARY-WORKAROUNDS.md section 7
            }
        }

        public TimeSpan InAppAutomationDisplayInterval
        {
            get
            {
                // TODO: DisplayInterval property not exposed in SDK 19 ObjectiveC bindings
                // TEMPORARY WORKAROUND - See: /Context/SDK19-TEMPORARY-WORKAROUNDS.md section 7
                return TimeSpan.Zero;
            }
            set
            {
                // TODO: DisplayInterval property not exposed in SDK 19 ObjectiveC bindings
                // TEMPORARY WORKAROUND - See: /Context/SDK19-TEMPORARY-WORKAROUNDS.md section 7
            }
        }
    }
}
