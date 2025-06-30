using AirshipDotNet;

namespace MauiSample;

public partial class FeaturesViewController : ContentPage
{
    public FeaturesViewController()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        enabledPushFeatureSwitch.On = AirshipDotNet.Airship.Instance.IsFeatureEnabled(Features.Push);
        enableMessageCenterFeatureSwitch.On = AirshipDotNet.Airship.Instance.IsFeatureEnabled(Features.MessageCenter);
        enableInAppAutomationFeatureSwitch.On = AirshipDotNet.Airship.Instance.IsFeatureEnabled(Features.InAppAutomation);
        EnableAnalyticsFeatureSwitch.On = AirshipDotNet.Airship.Instance.IsFeatureEnabled(Features.Analytics);
        enableTagsAndAttributesFeatureSwitch.On = AirshipDotNet.Airship.Instance.IsFeatureEnabled(Features.TagsAndAttributes);
        enableContactsFeatureSwitch.On = AirshipDotNet.Airship.Instance.IsFeatureEnabled(Features.Contacts);
    }

    void enablePushFeature_OnChanged(object sender, EventArgs e)
    {
        if (enabledPushFeatureSwitch.On)
        {
            AirshipDotNet.Airship.Instance.EnableFeatures(Features.Push);
        }
        else
        {
            AirshipDotNet.Airship.Instance.DisableFeatures(Features.Push);
        }
    }

    void enableMessageCenterFeature_OnChanged(object sender, EventArgs e)
    {
        if (enableMessageCenterFeatureSwitch.On)
        {
            AirshipDotNet.Airship.Instance.EnableFeatures(Features.MessageCenter);
        }
        else
        {
            AirshipDotNet.Airship.Instance.DisableFeatures(Features.MessageCenter);
        }
    }

    void enableInAppAutomationFeature_OnChanged(object sender, EventArgs e)
    {
        if (enableInAppAutomationFeatureSwitch.On)
        {
            AirshipDotNet.Airship.Instance.EnableFeatures(Features.InAppAutomation);
        }
        else
        {
            AirshipDotNet.Airship.Instance.DisableFeatures(Features.InAppAutomation);
        }
    }

    void enableAnalyticsFeature_OnChanged(object sender, EventArgs e)
    {
        if (EnableAnalyticsFeatureSwitch.On)
        {
            AirshipDotNet.Airship.Instance.EnableFeatures(Features.Analytics);
        }
        else
        {
            AirshipDotNet.Airship.Instance.DisableFeatures(Features.Analytics);
        }
    }

    void enableTagsAndAttributesFeature_OnChanged(object sender, EventArgs e)
    {
        if (enableTagsAndAttributesFeatureSwitch.On)
        {
            AirshipDotNet.Airship.Instance.EnableFeatures(Features.TagsAndAttributes);
        }
        else
        {
            AirshipDotNet.Airship.Instance.DisableFeatures(Features.TagsAndAttributes);
        }
    }

    void enableContactsFeature_OnChanged(object sender, EventArgs e)
    {
        if (enableContactsFeatureSwitch.On)
        {
            AirshipDotNet.Airship.Instance.EnableFeatures(Features.Contacts);
        }
        else
        {
            AirshipDotNet.Airship.Instance.DisableFeatures(Features.Contacts);
        }
    }

}
