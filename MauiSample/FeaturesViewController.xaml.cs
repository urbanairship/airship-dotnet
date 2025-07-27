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
        var enabledFeatures = AirshipDotNet.Airship.PrivacyManager.EnabledFeatures;
        enabledPushFeatureSwitch.On = enabledFeatures.HasFlag(Features.Push);
        enableMessageCenterFeatureSwitch.On = enabledFeatures.HasFlag(Features.MessageCenter);
        enableInAppAutomationFeatureSwitch.On = enabledFeatures.HasFlag(Features.InAppAutomation);
        EnableAnalyticsFeatureSwitch.On = enabledFeatures.HasFlag(Features.Analytics);
        enableTagsAndAttributesFeatureSwitch.On = enabledFeatures.HasFlag(Features.TagsAndAttributes);
        enableContactsFeatureSwitch.On = enabledFeatures.HasFlag(Features.Contacts);
    }

    async void enablePushFeature_OnChanged(object sender, EventArgs e)
    {
        try
        {
            if (enabledPushFeatureSwitch.On)
            {
                await AirshipDotNet.Airship.PrivacyManager.EnableFeaturesAsync(Features.Push);
            }
            else
            {
                await AirshipDotNet.Airship.PrivacyManager.DisableFeaturesAsync(Features.Push);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error toggling push feature: {ex.Message}");
        }
    }

    async void enableMessageCenterFeature_OnChanged(object sender, EventArgs e)
    {
        try
        {
            if (enableMessageCenterFeatureSwitch.On)
            {
                await AirshipDotNet.Airship.PrivacyManager.EnableFeaturesAsync(Features.MessageCenter);
            }
            else
            {
                await AirshipDotNet.Airship.PrivacyManager.DisableFeaturesAsync(Features.MessageCenter);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error toggling message center feature: {ex.Message}");
        }
    }

    async void enableInAppAutomationFeature_OnChanged(object sender, EventArgs e)
    {
        try
        {
            if (enableInAppAutomationFeatureSwitch.On)
            {
                await AirshipDotNet.Airship.PrivacyManager.EnableFeaturesAsync(Features.InAppAutomation);
            }
            else
            {
                await AirshipDotNet.Airship.PrivacyManager.DisableFeaturesAsync(Features.InAppAutomation);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error toggling in-app automation feature: {ex.Message}");
        }
    }

    async void enableAnalyticsFeature_OnChanged(object sender, EventArgs e)
    {
        try
        {
            if (EnableAnalyticsFeatureSwitch.On)
            {
                await AirshipDotNet.Airship.PrivacyManager.EnableFeaturesAsync(Features.Analytics);
            }
            else
            {
                await AirshipDotNet.Airship.PrivacyManager.DisableFeaturesAsync(Features.Analytics);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error toggling analytics feature: {ex.Message}");
        }
    }

    async void enableTagsAndAttributesFeature_OnChanged(object sender, EventArgs e)
    {
        try
        {
            if (enableTagsAndAttributesFeatureSwitch.On)
            {
                await AirshipDotNet.Airship.PrivacyManager.EnableFeaturesAsync(Features.TagsAndAttributes);
            }
            else
            {
                await AirshipDotNet.Airship.PrivacyManager.DisableFeaturesAsync(Features.TagsAndAttributes);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error toggling tags and attributes feature: {ex.Message}");
        }
    }

    async void enableContactsFeature_OnChanged(object sender, EventArgs e)
    {
        try
        {
            if (enableContactsFeatureSwitch.On)
            {
                await AirshipDotNet.Airship.PrivacyManager.EnableFeaturesAsync(Features.Contacts);
            }
            else
            {
                await AirshipDotNet.Airship.PrivacyManager.DisableFeaturesAsync(Features.Contacts);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error toggling contacts feature: {ex.Message}");
        }
    }

}
