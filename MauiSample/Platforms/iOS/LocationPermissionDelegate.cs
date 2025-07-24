using System;
using CoreLocation;
using Foundation;
using UIKit;
using Airship;

namespace MauiSample.Platforms.iOS
{
    public class LocationPermissionDelegate : UAAirshipPermissionDelegate
    {
        private readonly CLLocationManager locationManager = new CLLocationManager();

        [Export("checkPermissionStatusWithCompletionHandler:")]
        public override void CheckPermissionStatus(Action<UAPermissionStatus> completionHandler)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var status = GetCurrentPermissionStatus();
                completionHandler?.Invoke(status);
            });
        }

        [Export("requestPermissionWithCompletionHandler:")]
        public override void RequestPermission(Action<UAPermissionStatus> completionHandler)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var currentStatus = GetCurrentPermissionStatus();
                
                // If already determined, return current status
                if (currentStatus != UAPermissionStatus.notDetermined)
                {
                    completionHandler?.Invoke(currentStatus);
                    return;
                }
                
                // Only request when app is active
                if (UIApplication.SharedApplication.ApplicationState != UIApplicationState.Active)
                {
                    completionHandler?.Invoke(UAPermissionStatus.notDetermined);
                    return;
                }
                
                // Request location permission
                locationManager.RequestAlwaysAuthorization();
                
                // Wait for app to become active again (in case system prompt was shown)
                await WaitForActiveStateAsync();
                
                var finalStatus = GetCurrentPermissionStatus();
                completionHandler?.Invoke(finalStatus);
            });
        }

        private UAPermissionStatus GetCurrentPermissionStatus()
        {
            var status = locationManager.AuthorizationStatus;
            
            return status switch
            {
                CLAuthorizationStatus.NotDetermined => UAPermissionStatus.notDetermined,
                CLAuthorizationStatus.Restricted => UAPermissionStatus.denied,
                CLAuthorizationStatus.Denied => UAPermissionStatus.denied,
                CLAuthorizationStatus.AuthorizedAlways => UAPermissionStatus.granted,
                CLAuthorizationStatus.AuthorizedWhenInUse => UAPermissionStatus.granted,
                _ => UAPermissionStatus.notDetermined
            };
        }

        private async Task WaitForActiveStateAsync()
        {
            // If already active, return immediately
            if (UIApplication.SharedApplication.ApplicationState == UIApplicationState.Active)
            {
                return;
            }

            var tcs = new TaskCompletionSource<bool>();
            NSObject observer = null;
            
            observer = NSNotificationCenter.DefaultCenter.AddObserver(
                UIApplication.DidBecomeActiveNotification,
                (notification) =>
                {
                    tcs.TrySetResult(true);
                    observer?.Dispose();
                }
            );
            
            // Add timeout to prevent hanging
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30));
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);
            
            // Clean up observer
            observer?.Dispose();
            
            if (completedTask == timeoutTask)
            {
                Console.WriteLine("⚠️ Timeout waiting for app to become active");
            }
        }
    }
}