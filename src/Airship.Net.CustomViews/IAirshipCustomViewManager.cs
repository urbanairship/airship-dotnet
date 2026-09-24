/* Copyright Airship and Contributors */

using Microsoft.Maui.Controls;

namespace AirshipDotNet.CustomViews
{
    /// <summary>
    /// Registers MAUI views that Airship Scenes can render inline.
    /// </summary>
    /// <remarks>
    /// A Scene references a custom view by name. When the Scene renders, the factory
    /// registered under that name is invoked on the UI thread and the resulting MAUI
    /// view is converted to a native view and embedded in the Scene's layout.
    /// Register before a Scene that uses the view can be displayed — typically during
    /// app startup.
    /// </remarks>
    public interface IAirshipCustomViewManager
    {
        /// <summary>
        /// Registers a factory for the custom view named <paramref name="name"/>.
        /// Registering the same name twice replaces the previous factory.
        /// </summary>
        /// <param name="name">The custom view name, as referenced by the Scene.</param>
        /// <param name="factory">
        /// Creates the view. Invoked on the UI thread each time a Scene needs the view.
        /// Returning <c>null</c> renders an empty view.
        /// </param>
        void Register(string name, Func<AirshipCustomViewArguments, View?> factory);

        /// <summary>
        /// Unregisters the factory for <paramref name="name"/>. No-op if nothing is registered.
        /// </summary>
        /// <param name="name">The custom view name.</param>
        void Unregister(string name);
    }
}
