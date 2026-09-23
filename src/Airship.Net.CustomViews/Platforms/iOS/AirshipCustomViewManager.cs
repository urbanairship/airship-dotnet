/* Copyright Airship and Contributors */

using Airship;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using UIKit;

namespace AirshipDotNet.CustomViews
{
    /// <summary>
    /// iOS implementation of <see cref="IAirshipCustomViewManager"/>.
    /// </summary>
    public class AirshipCustomViewManager : IAirshipCustomViewManager
    {
        private static readonly Lazy<AirshipCustomViewManager> SharedInstance =
            new(() => new AirshipCustomViewManager());

        /// <summary>
        /// The shared custom view manager.
        /// </summary>
        public static IAirshipCustomViewManager Shared => SharedInstance.Value;

        // Held so the managed factories are not collected while the native side holds
        // the corresponding blocks.
        private readonly Dictionary<string, Func<AirshipCustomViewArguments, View?>> _factories = new();

        private AirshipCustomViewManager()
        {
        }

        /// <inheritdoc />
        public void Register(string name, Func<AirshipCustomViewArguments, View?> factory)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(factory);

            lock (_factories)
            {
                _factories[name] = factory;
            }

            UACustomViewManager.Shared.Register(name, nativeArgs =>
            {
                Func<AirshipCustomViewArguments, View?>? current;
                lock (_factories)
                {
                    _factories.TryGetValue(name, out current);
                }

                if (current == null)
                {
                    return null!;
                }

                var view = current(ToArguments(nativeArgs));
                if (view == null)
                {
                    return null!;
                }

                var mauiContext = Application.Current?.Handler?.MauiContext;
                if (mauiContext == null)
                {
                    // No MAUI context yet; the Scene renders its fallback (empty) view.
                    return null!;
                }

                return view.ToPlatform(mauiContext);
            });
        }

        /// <inheritdoc />
        public void Unregister(string name)
        {
            ArgumentNullException.ThrowIfNull(name);

            lock (_factories)
            {
                _factories.Remove(name);
            }

            UACustomViewManager.Shared.Unregister(name);
        }

        private static AirshipCustomViewArguments ToArguments(UACustomViewArguments args) =>
            new(
                args.Name,
                args.PropertiesJson,
                new AirshipCustomViewSizeInfo(args.SizeInfo.IsAutoHeight, args.SizeInfo.IsAutoWidth));
    }
}
