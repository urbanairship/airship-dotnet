using AirshipDotNet;
using AirshipDotNet.Analytics;

namespace MauiSample;

public partial class PushSettingsViewController : ContentPage
{
    public PushSettingsViewController()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        enabledPushSwitch.On = AirshipDotNet.Airship.Push.UserNotificationsEnabled;

        try
        {
            // Get channel ID asynchronously
            var id = await AirshipDotNet.Airship.Channel.GetChannelId();
            channelId.Detail = id ?? "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting channel ID: {ex.Message}");
            channelId.Detail = "";
        }

        UpdateNamedUser();
        UpdateTagsCell();
    }

    void displayFeatures(object sender, EventArgs e)
    {
        Navigation.PushAsync(new FeaturesViewController());
    }

    void enablePush_OnChanged(object sender, EventArgs e)
    {
        AirshipDotNet.Airship.Push.UserNotificationsEnabled = enabledPushSwitch.On;
    }

    async void CopyChannelID(object sender, EventArgs e)
    {
        try
        {
            var id = await AirshipDotNet.Airship.Channel.GetChannelId();
            if (!string.IsNullOrEmpty(id))
            {
                await Clipboard.Default.SetTextAsync(id);
                await DisplayAlert("Alert", "Channel ID copied to clipboard!", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error copying channel ID: {ex.Message}");
            await DisplayAlert("Error", "Failed to copy channel ID", "OK");
        }
    }

    void AddTag(object sender, EventArgs e)
    {
        string tagToAdd = tagLabel.Text;
        AirshipDotNet.Airship.Channel.EditTags()
                .AddTags(new string[] { tagToAdd })
                .Apply();
        UpdateTagsCell();
    }

    async void UpdateTagsCell()
    {
        tagLabel.Text = "";
        try
        {
            var tags = await AirshipDotNet.Airship.Channel.GetTags();

            string str = "";
            foreach (string tag in tags)
            {
                str = str + tag + "\n";
            }

            tagsList.Text = str;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating tags: {ex.Message}");
            tagsList.Text = "Error loading tags";
        }
    }

    // ==================== Analytics Tests ====================

    async void OnTrackEventClicked(object sender, EventArgs e)
    {
        try
        {
            var customEvent = new CustomEvent("test_event")
            {
                EventValue = 123.45,
                TransactionId = "txn_" + DateTime.Now.Ticks
            };
            customEvent.AddProperty("test_string", "hello");
            customEvent.AddProperty("test_number", 42);
            customEvent.AddProperty("test_bool", true);

            await AirshipDotNet.Airship.Analytics.TrackEvent(customEvent);
            Console.WriteLine("TrackEvent test completed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TrackEvent error: {ex}");
        }
    }

    async void OnAssociateIdentifierClicked(object sender, EventArgs e)
    {
        try
        {
            await AirshipDotNet.Airship.Analytics.AssociateIdentifier("test_key", "test_value_" + DateTime.Now.Ticks);
            Console.WriteLine("AssociateIdentifier test completed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AssociateIdentifier error: {ex}");
        }
    }

    // ==================== Contact Tests ====================

    async void UpdateNamedUser()
    {
        namedUserLabel.Text = "";
        try
        {
            var namedUser = await AirshipDotNet.Airship.Contact.GetNamedUser();
            namedUserLabel.Placeholder = namedUser ?? "named user";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateNamedUser: {ex.Message}");
            namedUserLabel.Placeholder = "Error loading named user";
        }
    }
    
    async void OnIdentifyContactClicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(namedUserLabel.Text))
            {
                await AirshipDotNet.Airship.Contact.Reset();
            }
            else
            {
                await AirshipDotNet.Airship.Contact.Identify(namedUserLabel.Text);
            }

            UpdateNamedUser();
            await DisplayAlert("Alert", "Named user added successfully", "OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding named user: {ex.Message}");
            await DisplayAlert("Error", "Failed to add named user", "OK");
        }
    }

    // ==================== In-App Automation Tests ====================

    async void OnSetPausedClicked(object sender, EventArgs e)
    {
        try
        {
            await AirshipDotNet.Airship.InApp.SetPaused(pauseAutomationSwitch.On);
            Console.WriteLine($"SetPaused test completed: {pauseAutomationSwitch.On}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SetPaused error: {ex}");
        }
    }

    async void OnSetDisplayIntervalClicked(object sender, EventArgs e)
    {
        try
        {
            var interval = TimeSpan.FromSeconds(30);
            await AirshipDotNet.Airship.InApp.SetDisplayInterval(interval);
            Console.WriteLine("SetDisplayInterval test completed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SetDisplayInterval error: {ex}");
        }
    }
}