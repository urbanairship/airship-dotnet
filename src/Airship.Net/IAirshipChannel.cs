/* Copyright Airship and Contributors */

using System.Collections.Generic;
using System.Threading.Tasks;
using AirshipDotNet.Channel;
using AirshipDotNet.Attributes;

namespace AirshipDotNet
{
    /// <summary>
    /// Airship Channel interface.
    /// </summary>
    public interface IAirshipChannel
    {
        /// <summary>
        /// Gets the channel ID.
        /// </summary>
        /// <returns>The channel ID or null if not created yet.</returns>
        Task<string?> GetChannelId();

        /// <summary>
        /// Gets the channel tags.
        /// </summary>
        /// <returns>List of tags.</returns>
        Task<List<string>> GetTags();

        /// <summary>
        /// Fetches the channel subscription lists.
        /// </summary>
        /// <returns>List of subscription list IDs.</returns>
        Task<List<string>> FetchSubscriptionLists();

        /// <summary>
        /// Edit channel tags.
        /// </summary>
        /// <returns>A TagEditor for channel tags.</returns>
        TagEditor EditTags();

        /// <summary>
        /// Edit channel tag groups.
        /// </summary>
        /// <returns>A TagGroupsEditor for channel tag groups.</returns>
        TagGroupsEditor EditTagGroups();

        /// <summary>
        /// Edit channel attributes.
        /// </summary>
        /// <returns>An AttributeEditor for channel attributes.</returns>
        AttributeEditor EditAttributes();

        /// <summary>
        /// Edit channel subscription lists.
        /// </summary>
        /// <returns>A SubscriptionListEditor for channel subscription lists.</returns>
        SubscriptionListEditor EditSubscriptionLists();
    }
}