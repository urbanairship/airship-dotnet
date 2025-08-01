/* Copyright Airship and Contributors */

using System;
using System.Threading.Tasks;
using Foundation;
using Airship;

namespace AirshipDotNet
{
    /// <summary>
    /// iOS-specific module for handling platform implementations.
    /// </summary>
    internal class AirshipModule
    {
        internal AirshipModule()
        {
        }

        internal AWAirshipWrapper Wrapper => AWAirshipWrapper.Shared;

        /// <summary>
        /// Helper method to wrap async callbacks into Tasks.
        /// </summary>
        internal Task<T> WrapAsync<T>(Action<Action<T>> method)
        {
            var tcs = new TaskCompletionSource<T>();
            method(result => tcs.SetResult(result));
            return tcs.Task;
        }

        /// <summary>
        /// Helper method to wrap async callbacks with error handling into Tasks.
        /// </summary>
        internal Task<T> WrapAsyncWithError<T>(Action<Action<T, Exception>> method)
        {
            var tcs = new TaskCompletionSource<T>();
            method((result, error) =>
            {
                if (error != null)
                {
                    tcs.SetException(new AirshipException(error.Message, error));
                }
                else
                {
                    tcs.SetResult(result);
                }
            });
            return tcs.Task;
        }
    }
}