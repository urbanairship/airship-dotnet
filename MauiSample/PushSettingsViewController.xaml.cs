using AirshipDotNet;

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
            var id = await AirshipDotNet.Airship.Channel.GetChannelIdAsync();
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
            var id = await AirshipDotNet.Airship.Channel.GetChannelIdAsync();
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

    async void AddNamedUser(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(namedUserLabel.Text))
            {
                await AirshipDotNet.Airship.Contact.ResetAsync();
            }
            else
            {
                await AirshipDotNet.Airship.Contact.IdentifyAsync(namedUserLabel.Text);
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
            var tags = await AirshipDotNet.Airship.Channel.GetTagsAsync();

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

    async void UpdateNamedUser()
    {
        namedUserLabel.Text = "";
        try
        {
            var namedUser = await AirshipDotNet.Airship.Contact.GetNamedUserAsync();
            namedUserLabel.Placeholder = namedUser ?? "named user";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateNamedUser: {ex.Message}");
            namedUserLabel.Placeholder = "Error loading named user";
        }
    }
}