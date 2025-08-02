/* Copyright Airship and Contributors */

using System;
using System.Threading.Tasks;
using UrbanAirship;
using Java.Util.Concurrent;

namespace AirshipDotNet.Platforms.Android
{
    /// <summary>
    /// Android-specific module for handling platform implementations.
    /// </summary>
    public class AirshipModule
    {
        public AirshipModule()
        {
        }

        internal UAirship UAirship => UAirship.Shared();

        /// <summary>
        /// Helper class to convert PendingResult callbacks to Tasks.
        /// </summary>
        internal class TaskResultCallback : Java.Lang.Object, UrbanAirship.IResultCallback
        {
            private readonly TaskCompletionSource<Java.Lang.Object?> _tcs;

            public TaskResultCallback(TaskCompletionSource<Java.Lang.Object?> tcs)
            {
                _tcs = tcs;
            }

            public void OnResult(Java.Lang.Object? result)
            {
                _tcs.SetResult(result);
            }
        }

        /// <summary>
        /// Helper method to wrap PendingResult into Tasks.
        /// </summary>
        internal Task<T?> WrapPendingResult<T>(UrbanAirship.PendingResult pendingResult) where T : Java.Lang.Object
        {
            var tcs = new TaskCompletionSource<Java.Lang.Object?>();
            pendingResult.AddResultCallback(new TaskResultCallback(tcs));
            return tcs.Task.ContinueWith(t => t.Result as T);
        }
    }
}