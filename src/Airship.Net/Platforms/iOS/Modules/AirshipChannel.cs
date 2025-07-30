/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AirshipDotNet.Channel;
using AirshipDotNet.Attributes;
using Foundation;
using Airship;

namespace AirshipDotNet
{
    /// <summary>
    /// iOS implementation of Airship Channel module.
    /// </summary>
    internal class AirshipChannel : IAirshipChannel
    {
        private readonly AirshipModule _module;

        internal AirshipChannel(AirshipModule module)
        {
            _module = module;
        }

        /// <summary>
        /// Gets the channel identifier.
        /// </summary>
        /// <returns>The channel ID or null if not created yet.</returns>
        public Task<string?> GetChannelId()
        {
            return Task.FromResult(UAirship.Channel.Identifier);
        }

        /// <summary>
        /// Gets the channel tags.
        /// </summary>
        /// <returns>The current tags.</returns>
        public Task<List<string>> GetTags()
        {
            var tags = UAirship.Channel.Tags;
            var result = tags?.ToList() ?? new List<string>();
            return Task.FromResult(result);
        }

        /// <summary>
        /// Fetches the channel subscription lists.
        /// </summary>
        /// <returns>List of subscription list IDs.</returns>
        public Task<List<string>> FetchSubscriptionLists()
        {
            var tcs = new TaskCompletionSource<List<string>>();

            // This is a wrapped method - handle it carefully
            AWAirshipWrapper.FetchChannelSubscriptionLists((lists, error) =>
            {
                var result = new List<string>();
                if (error != null)
                {
                    tcs.SetException(new AirshipException(error.LocalizedDescription));
                }
                else if (lists != null)
                {
                    for (nuint i = 0; i < lists.Count; i++)
                    {
                        var item = lists.GetItem<NSString>(i);
                        result.Add(item.ToString());
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
        /// Edit channel tags.
        /// </summary>
        /// <returns>A TagEditor for channel tags.</returns>
        public TagEditor EditTags()
        {
            return new TagEditor((clear, addTags, removeTags) =>
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
            });
        }

        /// <summary>
        /// Edit channel tag groups.
        /// </summary>
        /// <returns>A TagGroupsEditor for channel tag groups.</returns>
        public TagGroupsEditor EditTagGroups()
        {
            return new TagGroupsEditor((operations) =>
            {
                var editor = UAirship.Channel.EditTagGroups;

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
        /// Edit channel attributes.
        /// </summary>
        /// <returns>An AttributeEditor for channel attributes.</returns>
        public AttributeEditor EditAttributes()
        {
            return new AttributeEditor((operations) =>
            {
                var editor = UAirship.Channel.EditAttributes;

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
        /// Edit channel subscription lists.
        /// </summary>
        /// <returns>A SubscriptionListEditor for channel subscription lists.</returns>
        public SubscriptionListEditor EditSubscriptionLists()
        {
            return new SubscriptionListEditor((operations) =>
            {
                var editor = UAirship.Channel.EditSubscriptionLists;

                foreach (var operation in operations)
                {
                    if (operation.OperationType == SubscriptionListEditor.OperationType.SUBSCRIBE)
                    {
                        editor.Subscribe(operation.List);
                    }
                    else if (operation.OperationType == SubscriptionListEditor.OperationType.UNSUBSCRIBE)
                    {
                        editor.Unsubscribe(operation.List);
                    }
                }

                editor.Apply();
            });
        }
    }
}