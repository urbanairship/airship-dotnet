using System.ComponentModel;
using System.Runtime.CompilerServices;
using AirshipDotNet;
using AirshipDotNet.MessageCenter;
using System.Windows.Input;


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

        public async void Refresh()
        {
            try
            {
                // Using new modular API - Channel.GetChannelId()
                var channelId = await AirshipDotNet.Airship.Channel.GetChannelId();
                ChannelId = channelId;
                ShowEnablePushButton = !AirshipDotNet.Airship.Push.UserNotificationsEnabled;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing channel: {ex.Message}");
            }
        }

        private static async void PerformOnChannelIdClicked()
        {
            try
            {
                var channel = await AirshipDotNet.Airship.Channel.GetChannelId();
                if (!string.IsNullOrEmpty(channel))
                {
                    await Clipboard.Default.SetTextAsync(channel);
                    Console.WriteLine("Channel ID '{0}' copied to clipboard!", channel);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying channel ID: {ex.Message}");
            }
        }

        private static async void PerformOnEnablePushButtonClicked()
        {
            try
            {
                // Enable push notifications with fallback to system settings if permission was denied
                var args = new EnableUserPushNotificationsArgs(PromptPermissionFallback.SystemSettings);
                var enabled = await AirshipDotNet.Airship.Push.EnableUserNotifications(args);

                // Check the current notification permission status
                var status = await AirshipDotNet.Airship.Push.GetPushNotificationStatus();

                Console.WriteLine($"Push notifications enabled: {enabled}");
                Console.WriteLine($"Permission status: {status.NotificationPermissionStatus}, Opt-in: {status.IsOptIn}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enabling push notifications: {ex.Message}");
            }
        }

        private static async void PerformOnMessageCenterButtonClicked()
        {
            try
            {
                await AirshipDotNet.Airship.MessageCenter.Display();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error displaying message center: {ex.Message}");
            }
        }

        private static void PerformOnPrefCenterButtonClicked()
        {
            OpenPreferenceCenter("app_default");
        }

        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private static async void OpenPreferenceCenter(string prefCenterId)
        {
            await AirshipDotNet.Airship.PreferenceCenter.Open(prefCenterId);
        }
    }
}