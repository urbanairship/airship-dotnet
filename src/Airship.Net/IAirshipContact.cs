/* Copyright Airship and Contributors */

using System.Collections.Generic;
using System.Threading.Tasks;
using AirshipDotNet.Channel;
using AirshipDotNet.Contact;
using AirshipDotNet.Attributes;

namespace AirshipDotNet
{
    /// <summary>
    /// Airship Contact interface.
    /// </summary>
    public interface IAirshipContact
    {
        /// <summary>
        /// Gets the named user ID.
        /// </summary>
        /// <returns>The named user ID or null if not set.</returns>
        Task<string?> GetNamedUser();

        /// <summary>
        /// Identifies the contact with a named user ID.
        /// </summary>
        /// <param name="namedUserId">The named user ID.</param>
        Task Identify(string namedUserId);

        /// <summary>
        /// Resets the contact.
        /// </summary>
        Task Reset();

        /// <summary>
        /// Fetches the contact subscription lists.
        /// </summary>
        /// <returns>Dictionary of subscription lists by type.</returns>
        Task<Dictionary<string, List<string>>> FetchSubscriptionLists();

        /// <summary>
        /// Edit contact tag groups.
        /// </summary>
        /// <returns>A TagGroupsEditor for contact tag groups.</returns>
        TagGroupsEditor EditTagGroups();

        /// <summary>
        /// Edit contact attributes.
        /// </summary>
        /// <returns>An AttributeEditor for contact attributes.</returns>
        AttributeEditor EditAttributes();

        /// <summary>
        /// Edit contact subscription lists.
        /// </summary>
        /// <returns>A SubscriptionListEditor for contact subscription lists.</returns>
        Contact.SubscriptionListEditor EditSubscriptionLists();
    }
}