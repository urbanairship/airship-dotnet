﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using AirshipDotNet;
using System.Windows.Input;

# if ANDROID
using UrbanAirship.PreferenceCenter;
#elif IOS
using Airship;
#endif

namespace MauiSample
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        // Internal Fields
        private string _channelId = null;
        private Boolean _showEnablePushButton = false;


        // Commands
        public ICommand OnChannelIdClicked { get; }
        public ICommand OnEnablePushButtonClicked { get; }
        public ICommand OnMessageCenterButtonClicked { get; }
        public ICommand OnPrefCenterButtonClicked { get; }

        // Properties
        public string ChannelId
        {
            get => _channelId;
            set
            {
                if (_channelId != value)
                {
                    _channelId = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean ShowEnablePushButton
        {
            get => _showEnablePushButton;
            set
            {
                if (_showEnablePushButton != value)
                {
                    _showEnablePushButton = value;
                    OnPropertyChanged();
                }
            }
        }

        // INotifyPropertyChanged Impl
        public event PropertyChangedEventHandler PropertyChanged;

        public HomePageViewModel()
        {
            OnChannelIdClicked = new Command(PerformOnChannelIdClicked);
            OnEnablePushButtonClicked = new Command(PerformOnEnablePushButtonClicked);
            OnMessageCenterButtonClicked = new Command(PerformOnMessageCenterButtonClicked);
            OnPrefCenterButtonClicked = new Command(PerformOnPrefCenterButtonClicked);

            AirshipDotNet.Airship.Instance.OnChannelCreation += OnChannelEvent;
            AirshipDotNet.Airship.Instance.OnPushNotificationStatusUpdate += OnPushNotificationStatusEvent;

            Refresh();
        }

        ~HomePageViewModel()
        {
            AirshipDotNet.Airship.Instance.OnChannelCreation -= OnChannelEvent;
            AirshipDotNet.Airship.Instance.OnPushNotificationStatusUpdate -= OnPushNotificationStatusEvent;
        }

        private void OnChannelEvent(object sender, EventArgs e) => Refresh();

        private void OnPushNotificationStatusEvent(object sender, EventArgs e) => Refresh();

        public void Refresh()
        {
            ChannelId = AirshipDotNet.Airship.Instance.ChannelId;
            ShowEnablePushButton = !AirshipDotNet.Airship.Instance.UserNotificationsEnabled;
        }

        private static void PerformOnChannelIdClicked()
        {
            var channel = AirshipDotNet.Airship.Instance.ChannelId;
            Clipboard.Default.SetTextAsync(channel);
            Console.WriteLine("Channel ID '{0}' copied to clipboard!", channel);
        }

        private static void PerformOnEnablePushButtonClicked()
        {
            AirshipDotNet.Airship.Instance.UserNotificationsEnabled = true;
        }

        private static void PerformOnMessageCenterButtonClicked()
        {
            AirshipDotNet.Airship.Instance.DisplayMessageCenter();
        }

        private static void PerformOnPrefCenterButtonClicked()
        {
            OpenPreferenceCenter("app_default");
        }

        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private static void OpenPreferenceCenter(string prefCenterId)
        {
#if ANDROID
            PreferenceCenter.Shared().Open(prefCenterId);
#elif IOS
            // SDK 19: Access PreferenceCenter through UAirship.PreferenceCenter
            UAirship.PreferenceCenter.OpenPreferenceCenter(prefCenterId);
#endif
        }
    }
}