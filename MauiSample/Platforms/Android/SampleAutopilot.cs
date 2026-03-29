using System;
using AirshipDotNet;
using Android.Content;
using Android.Runtime;
using Android.Util;
using AndroidX.Core.Content;
using MauiSample;
using UrbanAirship;

namespace MauiSample
{
    [Register("sample.SampleAutopilot")]
    public class SampleAutopilot : Autopilot
    {
        public override void OnAirshipReady(UAirship airship)
        {
            AirshipListener airshipListener = new();
            airship.PushManager.NotificationListener = airshipListener;
            airship.PushManager.AddPushListener(airshipListener);
            airship.PushManager.AddPushTokenListener(airshipListener);
            airship.Channel.AddChannelListener(airshipListener);
        }

    }
}

