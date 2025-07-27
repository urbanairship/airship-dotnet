/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AirshipDotNet.Channel;
using AirshipDotNet.Attributes;
using UrbanAirship;
using Java.Util;

namespace AirshipDotNet
{
    /// <summary>
    /// Android implementation of Airship Channel module.
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
        public Task<string?> GetChannelIdAsync()
        {
            return Task.FromResult(_module.UAirship.Channel.Id);
        }

        /// <summary>
        /// Gets the channel tags.
        /// </summary>
        /// <returns>The current tags.</returns>
        public Task<List<string>> GetTagsAsync()
        {
            var tags = _module.UAirship.Channel.Tags;
            var result = tags?.ToList() ?? new List<string>();
            return Task.FromResult(result);
        }

        /// <summary>
        /// Fetches the channel subscription lists.
        /// </summary>
        /// <returns>List of subscription list IDs.</returns>
        public async Task<List<string>> FetchSubscriptionListsAsync()
        {
            var pendingResult = _module.UAirship.Channel.FetchSubscriptionListsPendingResult();
            var result = await _module.WrapPendingResult<HashSet>(pendingResult);

            var list = new List<string>();
            if (result != null)
            {
                var iterator = result.Iterator();
                while (iterator.HasNext)
                {
                    var item = iterator.Next()?.ToString();
                    if (item != null)
                    {
                        list.Add(item);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Edit channel tags.
        /// </summary>
        /// <returns>A TagEditor for channel tags.</returns>
        public AirshipDotNet.Channel.TagEditor EditTags()
        {
            return new AirshipDotNet.Channel.TagEditor((clear, addTags, removeTags) =>
            {
                var editor = _module.UAirship.Channel.EditTags();

                if (clear)
                {
                    editor = editor.Clear();
                }

                if (addTags != null && addTags.Length > 0)
                {
                    editor = editor.AddTags(addTags);
                }

                if (removeTags != null && removeTags.Length > 0)
                {
                    editor = editor.RemoveTags(removeTags);
                }

                editor.Apply();
            });
        }

        /// <summary>
        /// Edit channel tag groups.
        /// </summary>
        /// <returns>A TagGroupsEditor for channel tag groups.</returns>
        public AirshipDotNet.Channel.TagGroupsEditor EditTagGroups()
        {
            return new AirshipDotNet.Channel.TagGroupsEditor((operations) =>
            {
                var editor = _module.UAirship.Channel.EditTagGroups();

                foreach (var operation in operations)
                {
                    switch (operation.operationType)
                    {
                        case AirshipDotNet.Channel.TagGroupsEditor.OperationType.ADD:
                            editor.AddTags(operation.group, operation.tags);
                            break;
                        case AirshipDotNet.Channel.TagGroupsEditor.OperationType.SET:
                            editor.SetTags(operation.group, operation.tags);
                            break;
                        case AirshipDotNet.Channel.TagGroupsEditor.OperationType.REMOVE:
                            editor.RemoveTags(operation.group, operation.tags);
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
        public AirshipDotNet.Attributes.AttributeEditor EditAttributes()
        {
            return new AirshipDotNet.Attributes.AttributeEditor((operations) =>
            {
                var editor = _module.UAirship.Channel.EditAttributes();

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
        /// Edit channel subscription lists.
        /// </summary>
        /// <returns>A SubscriptionListEditor for channel subscription lists.</returns>
        public AirshipDotNet.Channel.SubscriptionListEditor EditSubscriptionLists()
        {
            return new AirshipDotNet.Channel.SubscriptionListEditor((operations) =>
            {
                var editor = _module.UAirship.Channel.EditSubscriptionLists();

                foreach (var operation in operations)
                {
                    if (operation.OperationType == AirshipDotNet.Channel.SubscriptionListEditor.OperationType.SUBSCRIBE)
                    {
                        editor.Subscribe(operation.List);
                    }
                    else if (operation.OperationType == AirshipDotNet.Channel.SubscriptionListEditor.OperationType.UNSUBSCRIBE)
                    {
                        editor.Unsubscribe(operation.List);
                    }
                }

                editor.Apply();
            });
        }
    }
}