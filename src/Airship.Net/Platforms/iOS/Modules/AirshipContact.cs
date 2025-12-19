/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AirshipDotNet.Channel;
using AirshipDotNet.Contact;
using AirshipDotNet.Attributes;
using Foundation;
using Airship;

namespace AirshipDotNet.Platforms.iOS.Modules
{
    /// <summary>
    /// iOS implementation of Airship Contact module.
    /// </summary>
    internal class AirshipContact : IAirshipContact
    {
        private readonly AirshipModule _module;

        internal AirshipContact(AirshipModule module)
        {
            _module = module;
        }

        /// <summary>
        /// Gets the named user ID.
        /// </summary>
        /// <returns>The named user ID or null if not set.</returns>
        public Task<string?> GetNamedUser()
        {
            var tcs = new TaskCompletionSource<string?>();

            // This is a wrapped method - handle it carefully
            AWAirshipWrapper.GetNamedUserID((namedUserId, error) =>
            {
                if (error != null)
                {
                    tcs.SetException(new AirshipException(error.LocalizedDescription));
                }
                else
                {
                    // Return null for empty string to match expected behavior
                    tcs.SetResult(string.IsNullOrEmpty(namedUserId) ? null : namedUserId);
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// Identifies the contact with a named user ID.
        /// </summary>
        /// <param name="namedUserId">The named user ID.</param>
        public Task Identify(string namedUserId)
        {
            NSRunLoop.Main.BeginInvokeOnMainThread(() => UAirship.Contact.Identify(namedUserId));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Resets the contact.
        /// </summary>
        public Task Reset()
        {
            NSRunLoop.Main.BeginInvokeOnMainThread(() => UAirship.Contact.Reset());
            return Task.CompletedTask;
        }

        /// <summary>
        /// Fetches the contact subscription lists.
        /// </summary>
        /// <returns>Dictionary of subscription lists by type.</returns>
        public Task<Dictionary<string, List<string>>> FetchSubscriptionLists()
        {
            var tcs = new TaskCompletionSource<Dictionary<string, List<string>>>();

            // This is a wrapped method - handle it carefully
            AWAirshipWrapper.FetchContactSubscriptionLists((lists, error) =>
            {
                var result = new Dictionary<string, List<string>>();

                if (error != null)
                {
                    tcs.SetException(new AirshipException(error.LocalizedDescription));
                }
                else if (lists != null)
                {
                    foreach (var kvp in lists)
                    {
                        var list = new List<string>();
                        var scopes = kvp.Value as NSArray;
                        if (scopes != null)
                        {
                            for (nuint i = 0; i < scopes.Count; i++)
                            {
                                var scope = scopes.GetItem<NSString>(i);
                                list.Add(scope.ToString());
                            }
                        }
                        result.Add(kvp.Key.ToString(), list);
                    }
                    tcs.SetResult(result);
                }
                else
                {
                    tcs.SetResult(result);
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// Edit contact tag groups.
        /// </summary>
        /// <returns>A TagGroupsEditor for contact tag groups.</returns>
        public TagGroupsEditor EditTagGroups()
        {
            return new TagGroupsEditor((operations) =>
            {
                var editor = UAirship.Contact.EditTagGroups;

                foreach (var operation in operations)
                {
                    switch (operation.operationType)
                    {
                        case TagGroupsEditor.OperationType.ADD:
                            editor.AddTags(operation.tags.ToArray(), operation.group);
                            break;
                        case TagGroupsEditor.OperationType.SET:
                            editor.SetTags(operation.tags.ToArray(), operation.group);
                            break;
                        case TagGroupsEditor.OperationType.REMOVE:
                            editor.RemoveTags(operation.tags.ToArray(), operation.group);
                            break;
                    }
                }

                editor.Apply();
            });
        }

        /// <summary>
        /// Edit contact attributes.
        /// </summary>
        /// <returns>An AttributeEditor for contact attributes.</returns>
        public AttributeEditor EditAttributes()
        {
            return new AttributeEditor((operations) =>
            {
                var editor = UAirship.Contact.EditAttributes;

                foreach (var operation in operations)
                {
                    if (operation is AttributeEditor.RemoveAttributeOperation removeOp)
                    {
                        editor.RemoveAttribute(removeOp.Key);
                    }
                    else if (operation is AttributeEditor.SetAttributeOperation<string> stringOp)
                    {
                        editor.SetString(stringOp.Value, stringOp.Key);
                    }
                    else if (operation is AttributeEditor.SetAttributeOperation<int> intOp)
                    {
                        editor.SetNumber(NSNumber.FromInt32(intOp.Value), intOp.Key);
                    }
                    else if (operation is AttributeEditor.SetAttributeOperation<long> longOp)
                    {
                        editor.SetNumber(NSNumber.FromInt64(longOp.Value), longOp.Key);
                    }
                    else if (operation is AttributeEditor.SetAttributeOperation<float> floatOp)
                    {
                        editor.SetNumber(NSNumber.FromFloat(floatOp.Value), floatOp.Key);
                    }
                    else if (operation is AttributeEditor.SetAttributeOperation<double> doubleOp)
                    {
                        editor.SetNumber(NSNumber.FromDouble(doubleOp.Value), doubleOp.Key);
                    }
                    else if (operation is AttributeEditor.SetAttributeOperation<DateTime> dateOp)
                    {
                        // Convert DateTime to NSDate
                        var nsDate = NSDate.FromTimeIntervalSince1970(
                            (dateOp.Value.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
                        );
                        editor.SetDate(nsDate, dateOp.Key);
                    }
                }

                editor.Apply();
            });
        }

        /// <summary>
        /// Edit contact subscription lists.
        /// </summary>
        /// <returns>A SubscriptionListEditor for contact subscription lists.</returns>
        public Contact.SubscriptionListEditor EditSubscriptionLists()
        {
            return new Contact.SubscriptionListEditor((operations) =>
            {
                var editor = UAirship.Contact.EditSubscriptionLists;

                foreach (var operation in operations)
                {
                    var channelScope = ConvertToChannelScope(operation.Scope);

                    if (operation.OperationType == Contact.SubscriptionListEditor.OperationType.SUBSCRIBE)
                    {
                        editor.Subscribe(operation.List, channelScope);
                    }
                    else if (operation.OperationType == Contact.SubscriptionListEditor.OperationType.UNSUBSCRIBE)
                    {
                        editor.Unsubscribe(operation.List, channelScope);
                    }
                }

                editor.Apply();
            });
        }

        private static UAChannelScope ConvertToChannelScope(string scope)
        {
            // The Contact.SubscriptionListEditor stores scope as a string
            // We need to convert it to the appropriate UAChannelScope
            switch (scope?.ToLowerInvariant())
            {
                case "app":
                    return UAChannelScope.App;
                case "web":
                    return UAChannelScope.Web;
                case "email":
                    return UAChannelScope.Email;
                case "sms":
                    return UAChannelScope.Sms;
                default:
                    return UAChannelScope.App;
            }
        }
    }
}