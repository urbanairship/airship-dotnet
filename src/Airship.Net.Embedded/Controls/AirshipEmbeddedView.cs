/* Copyright Airship and Contributors */

using Microsoft.Maui.Controls;

namespace AirshipDotNet.Embedded.Controls
{
    /// <summary>
    /// A view that renders Airship embedded content for a given embedded ID.
    /// </summary>
    public class AirshipEmbeddedView : View
    {
        /// <summary>Bindable property for <see cref="EmbeddedId"/>.</summary>
        public static readonly BindableProperty EmbeddedIdProperty = BindableProperty.Create(
            nameof(EmbeddedId),
            typeof(string),
            typeof(AirshipEmbeddedView),
            default(string));

        /// <summary>Gets or sets the Airship embedded content ID to display.</summary>
        public string EmbeddedId
        {
            get => (string)GetValue(EmbeddedIdProperty);
            set => SetValue(EmbeddedIdProperty, value);
        }
    }
}
