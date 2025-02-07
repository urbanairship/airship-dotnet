﻿/*
 Copyright Airship and Contributors
*/

using System;

using Android.App;
using Android.OS;
using System.Reflection;
using UrbanAirship.Analytics;
using UrbanAirship.Attributes;

[assembly: Permission(Name = "@PACKAGE_NAME@.permission.UA_DATA", ProtectionLevel=Android.Content.PM.Protection.Signature)]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.UA_DATA")]

[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.ACCESS_NETWORK_STATE")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
[assembly: UsesPermission(Name = "android.permission.VIBRATE")]
namespace UrbanAirship
{
	public partial class UAirship
	{
		static UAirship()
		{
			UAirship.Shared(new AirshipReadyCallback((UAirship airship) =>
			{
				// Register Airship Xamarin component
				var crossPlatformVersions = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(UACrossPlatformVersionAttribute), false);
				if (crossPlatformVersions.Length < 1) return;
				
				var version = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(UACrossPlatformVersionAttribute), false)[0] as UACrossPlatformVersionAttribute;
				airship.Analytics.RegisterSDKExtension(Extension.DotNet!, version!.Version);
			}));
		}
		public static void TakeOff(Application application, Action<UAirship> callback)
		{
			TakeOff(application, new AirshipReadyCallback (callback));
		}

		public static void TakeOff(Application application, AirshipConfigOptions configOptions, Action<UAirship> callback)
		{
			TakeOff(application, configOptions, new AirshipReadyCallback (callback));
		}

		public static ICancelable Shared (Action<UAirship> callback, Looper looper)
		{
			return Shared(looper, new AirshipReadyCallback (callback));
		}

		public static ICancelable Shared (Action<UAirship> callback)
		{
			return Shared(new AirshipReadyCallback (callback));
		}

		internal class AirshipReadyCallback(Action<UAirship> callback) : Java.Lang.Object, IOnReadyCallback
		{
			public void OnAirshipReady (UAirship airship) {
				callback.Invoke(airship);
			}
		}
	}
}

