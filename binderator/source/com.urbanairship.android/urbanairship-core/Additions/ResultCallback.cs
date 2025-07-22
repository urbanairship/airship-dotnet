/*
 Copyright Airship and Contributors
*/

using System;

namespace UrbanAirship
{
    /// <summary>
    /// Generic ResultCallback implementation for handling async results
    /// </summary>
    public class ResultCallback : Java.Lang.Object, IResultCallback
    {
        private readonly Action<Java.Lang.Object?> action;

        public ResultCallback(Action<Java.Lang.Object?> action) => this.action = action;

        public void OnResult(Java.Lang.Object? result) => action(result);
    }
}