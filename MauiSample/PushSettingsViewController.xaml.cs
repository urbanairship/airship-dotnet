using AirshipDotNet;

namespace MauiSample;

public partial class PushSettingsViewController : ContentPage
{
    public PushSettingsViewController()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        enabledPushSwitch.On = Airship.Instance.UserNotificationsEnabled;
        channelId.Detail = Airship.Instance.ChannelId != null ? Airship.Instance.ChannelId : "";
        UpdateNamedUser();
        UpdateTagsCell();
    }

    void displayFeatures(object sender, EventArgs e)
    {
        Navigation.PushAsync(new FeaturesViewController());
    }

    void enablePush_OnChanged(object sender, EventArgs e)
    {
        Airship.Instance.UserNotificationsEnabled = enabledPushSwitch.On;
    }

    void CopyChannelID(object sender, EventArgs e)
    {
        if (Airship.Instance.ChannelId != null)
        {
            Clipboard.Default.SetTextAsync(Airship.Instance.ChannelId);
            DisplayAlert("Alert", "Channel ID copied to clipboard!", "OK");
        }
    }

    void AddNamedUser(object sender, EventArgs e)
    {
        if (namedUserLabel.Text == null)
        {
            Airship.Instance.ResetContact();
        }
        else
        {
            Airship.Instance.IdentifyContact(namedUserLabel.Text);
        }
        UpdateNamedUser();
        DisplayAlert("Alert", "Named user added successufully", "OK");
    }

    void AddTag(object sender, EventArgs e)
    {
        string tagToAdd = tagLabel.Text;
        Airship.Instance.EditDeviceTags()
                .AddTags(new string[] { tagToAdd })
                .Apply();
        UpdateTagsCell();
    }

    void UpdateTagsCell()
    {
        tagLabel.Text = "";
        IEnumerable<string> tags = Airship.Instance.Tags;

        string str = "";
        foreach (string tag in tags)
        {
            str = str + tag + "\n";
        }
        tagsList.Text = str;
    }

    void UpdateNamedUser()
    {
        namedUserLabel.Text = "";
        // FIXME:
    //    Airship.Instance.GetNamedUser(namedUser =>
    //    {
    //        Device.BeginInvokeOnMainThread(() =>
    //        {
    //            namedUserLabel.Placeholder = namedUser != null ? namedUser : "named user";
    //        });
    //    });
    //}
}