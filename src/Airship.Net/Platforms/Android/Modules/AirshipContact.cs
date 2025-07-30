/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AirshipDotNet.Channel;
using AirshipDotNet.Contact;
using AirshipDotNet.Attributes;
using UrbanAirship;
using Com.Urbanairship.Contacts;
using Java.Util;

namespace AirshipDotNet
{
    /// <summary>
    /// Android implementation of Airship Contact module.
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
            return Task.FromResult(_module.UAirship.Contact.NamedUserId);
        }

        /// <summary>
        /// Resets the contact.
        /// </summary>
        public Task Reset()
        {
            _module.UAirship.Contact.Reset();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Identifies the contact with a named user ID.
        /// </summary>
        /// <param name="namedUserId">The named user ID.</param>
        public Task Identify(string namedUserId)
        {
            _module.UAirship.Contact.Identify(namedUserId);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Fetches the contact subscription lists.
        /// </summary>
        /// <returns>Dictionary of subscription lists by scope.</returns>
        public async Task<Dictionary<string, List<string>>> FetchSubscriptionLists()
        {
            var pendingResult = _module.UAirship.Contact.FetchSubscriptionListsPendingResult();
            var result = await _module.WrapPendingResult<HashMap>(pendingResult);

            var dictionary = new Dictionary<string, List<string>>();
            if (result != null)
            {
                foreach (var key in result.KeySet())
                {
                    if (key != null)
                    {
                        var keyStr = key.ToString()!;
                        var value = result.Get(key as Java.Lang.Object);

                        if (value is HashSet hashSet)
                        {
                            var list = new List<string>();
                            var iterator = hashSet.Iterator();
                            while (iterator.HasNext)
                            {
                                var item = iterator.Next()?.ToString();
                                if (item != null)
                                {
                                    list.Add(item);
                                }
                            }
                            dictionary.Add(keyStr, list);
                        }
                    }
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Edit contact tag groups.
        /// </summary>
        /// <returns>A TagGroupsEditor for contact tag groups.</returns>
        public TagGroupsEditor EditTagGroups()
        {
            return new TagGroupsEditor((operations) =>
            {
                var editor = _module.UAirship.Contact.EditTagGroups();

                foreach (var operation in operations)
                {
                    switch (operation.operationType)
                    {
                        case TagGroupsEditor.OperationType.ADD:
                            editor.AddTags(operation.group, operation.tags);
                            break;
                        case TagGroupsEditor.OperationType.SET:
                            editor.SetTags(operation.group, operation.tags);
                            break;
                        case TagGroupsEditor.OperationType.REMOVE:
                            editor.RemoveTags(operation.group, operation.tags);
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
                var editor = _module.UAirship.Contact.EditAttributes();

                foreach (var operation in operations)
                {
                    if (operation.OperationType == AirshipDotNet.Attributes.AttributeEditor.OperationType.REMOVE)
                    {
                        editor.RemoveAttribute(operation.Key);
                    }
                    else if (operation is AirshipDotNet.Attributes.AttributeEditor.SetAttributeOperation<string> stringOp)
                    {
                        editor.SetAttribute(stringOp.Key, stringOp.Value);
                    }
                    else if (operation is AirshipDotNet.Attributes.AttributeEditor.SetAttributeOperation<int> intOp)
                    {
                        editor.SetAttribute(intOp.Key, intOp.Value);
                    }
                    else if (operation is AirshipDotNet.Attributes.AttributeEditor.SetAttributeOperation<long> longOp)
                    {
                        editor.SetAttribute(longOp.Key, longOp.Value);
                    }
                    else if (operation is AirshipDotNet.Attributes.AttributeEditor.SetAttributeOperation<float> floatOp)
                    {
                        editor.SetAttribute(floatOp.Key, floatOp.Value);
                    }
                    else if (operation is AirshipDotNet.Attributes.AttributeEditor.SetAttributeOperation<double> doubleOp)
                    {
                        editor.SetAttribute(doubleOp.Key, doubleOp.Value);
                    }
                    else if (operation is AirshipDotNet.Attributes.AttributeEditor.SetAttributeOperation<DateTime> dateOp)
                    {
                        // Convert DateTime to Java Date
                        long epochSeconds = new DateTimeOffset(dateOp.Value).ToUnixTimeSeconds();
                        var date = new Date(epochSeconds * 1000);
                        editor.SetAttribute(dateOp.Key, date);
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
                var editor = _module.UAirship.Contact.EditSubscriptionLists();

                foreach (var operation in operations)
                {
                    // Determine the scope
                    Scope scope = Scope.App;
                    if (operation.Scope == "app")
                    {
                        scope = Scope.App;
                    }
                    else if (operation.Scope == "web")
                    {
                        scope = Scope.Web;
                    }
                    else if (operation.Scope == "email")
                    {
                        scope = Scope.Email;
                    }
                    else if (operation.Scope == "sms")
                    {
                        scope = Scope.Sms;
                    }

                    if (operation.OperationType == Contact.SubscriptionListEditor.OperationType.SUBSCRIBE)
                    {
                        editor.Subscribe(operation.List, scope);
                    }
                    else if (operation.OperationType == Contact.SubscriptionListEditor.OperationType.UNSUBSCRIBE)
                    {
                        editor.Unsubscribe(operation.List, scope);
                    }
                }

                editor.Apply();
            });
        }
    }
}