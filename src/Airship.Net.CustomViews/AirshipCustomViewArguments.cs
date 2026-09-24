/* Copyright Airship and Contributors */

namespace AirshipDotNet.CustomViews
{
    /// <summary>
    /// Sizing information for a custom view, as declared by the Scene.
    /// </summary>
    public class AirshipCustomViewSizeInfo
    {
        /// <summary>
        /// Whether the Scene sized this view's height automatically.
        /// </summary>
        public bool IsAutoHeight { get; }

        /// <summary>
        /// Whether the Scene sized this view's width automatically.
        /// </summary>
        public bool IsAutoWidth { get; }

        /// <summary>
        /// Creates sizing information for a custom view.
        /// </summary>
        /// <param name="isAutoHeight">Whether the height is automatic.</param>
        /// <param name="isAutoWidth">Whether the width is automatic.</param>
        public AirshipCustomViewSizeInfo(bool isAutoHeight, bool isAutoWidth)
        {
            IsAutoHeight = isAutoHeight;
            IsAutoWidth = isAutoWidth;
        }
    }

    /// <summary>
    /// Arguments passed to a custom view factory when a Scene renders the view.
    /// </summary>
    public class AirshipCustomViewArguments
    {
        /// <summary>
        /// The name the view was registered under.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The JSON properties the Scene supplied for this view, or <c>null</c> if there were none.
        /// </summary>
        /// <remarks>
        /// This is the raw JSON string. Both platforms hand the properties across as JSON, so
        /// deserialize it with whatever shape your view expects.
        /// </remarks>
        public string? PropertiesJson { get; }

        /// <summary>
        /// Sizing information declared by the Scene.
        /// </summary>
        public AirshipCustomViewSizeInfo SizeInfo { get; }

        /// <summary>
        /// Creates custom view arguments.
        /// </summary>
        /// <param name="name">The registered view name.</param>
        /// <param name="propertiesJson">The Scene's JSON properties, or null.</param>
        /// <param name="sizeInfo">Sizing information.</param>
        public AirshipCustomViewArguments(string name, string? propertiesJson, AirshipCustomViewSizeInfo sizeInfo)
        {
            Name = name;
            PropertiesJson = propertiesJson;
            SizeInfo = sizeInfo;
        }
    }
}
