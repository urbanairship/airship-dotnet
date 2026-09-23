/* Copyright Airship and Contributors */

using Android.Content;
using Com.Urbanairship.Android.Layout;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using AndroidView = Android.Views.View;

// The native types share their simple names with the shared managed ones, so alias them.
using NativeManager = Com.Urbanairship.Android.Layout.AirshipCustomViewManager;
using NativeArgs = Com.Urbanairship.Android.Layout.AirshipCustomViewArguments;

namespace AirshipDotNet.CustomViews
{
    /// <summary>
    /// Android implementation of <see cref="IAirshipCustomViewManager"/>.
    /// </summary>
    public class AirshipCustomViewManager : IAirshipCustomViewManager
    {
        private static readonly Lazy<AirshipCustomViewManager> SharedInstance =
            new(() => new AirshipCustomViewManager());

        /// <summary>
        /// The shared custom view manager.
        /// </summary>
        public static IAirshipCustomViewManager Shared => SharedInstance.Value;

        // Held so the handlers are not collected while the native manager holds them.
        private readonly Dictionary<string, HandlerWrapper> _handlers = new();

        private AirshipCustomViewManager()
        {
        }

        /// <inheritdoc />
        public void Register(string name, Func<AirshipCustomViewArguments, View?> factory)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(factory);

            var wrapper = new HandlerWrapper(factory);
            lock (_handlers)
            {
                _handlers[name] = wrapper;
            }

            // The native type is a Kotlin `object`; the binding exposes it as a singleton.
            NativeManager.Instance!.Register(name, wrapper);
        }

        /// <inheritdoc />
        public void Unregister(string name)
        {
            ArgumentNullException.ThrowIfNull(name);

            lock (_handlers)
            {
                _handlers.Remove(name);
            }

            NativeManager.Instance!.Unregister(name);
        }

        private class HandlerWrapper : Java.Lang.Object, IAirshipCustomViewHandler
        {
            private readonly Func<AirshipCustomViewArguments, View?> _factory;

            internal HandlerWrapper(Func<AirshipCustomViewArguments, View?> factory)
            {
                _factory = factory;
            }

            public AndroidView OnCreateView(Context context, NativeArgs args)
            {
                var view = _factory(ToArguments(args));
                var mauiContext = Application.Current?.Handler?.MauiContext;

                if (view == null || mauiContext == null)
                {
                    // Nothing to show; hand back an empty view so the Scene still lays out.
                    return new AndroidView(context);
                }

                return view.ToPlatform(mauiContext);
            }

            private static AirshipCustomViewArguments ToArguments(NativeArgs args)
            {
                // SizeInfo binds as a method, not a property.
                var sizeInfo = args.GetSizeInfo();

                return new AirshipCustomViewArguments(
                    args.Name,
                    args.Properties?.ToString(),
                    new AirshipCustomViewSizeInfo(sizeInfo.IsAutoHeight, sizeInfo.IsAutoWidth));
            }
        }
    }
}
