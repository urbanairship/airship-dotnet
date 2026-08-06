using AirshipDotNet;

namespace MauiSample;

public partial class HomePage : ContentPage
{
    HomePageViewModel viewModel = new HomePageViewModel();
    EventHandler<EmbeddedInfoUpdatedEventArgs>? embeddedInfoUpdatedHandler;

    public HomePage()
	{
		InitializeComponent();
        BindingContext = viewModel;
   	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Track the screen view
        await AirshipDotNet.Airship.Analytics.TrackScreen("HomePage");

        // Register the "test" tag so the embedded message can be targeted to this device.
        AirshipDotNet.Airship.Channel.EditTags().AddTag("test").Apply();

        if (embeddedInfoUpdatedHandler == null)
        {
            embeddedInfoUpdatedHandler = (_, _) => RefreshEmbeddedPlaceholder();
            AirshipDotNet.Airship.InApp.EmbeddedInfoUpdated += embeddedInfoUpdatedHandler;
        }
        RefreshEmbeddedPlaceholder();

        viewModel.Refresh();
    }

    protected override void OnDisappearing()
    {
        if (embeddedInfoUpdatedHandler != null)
        {
            AirshipDotNet.Airship.InApp.EmbeddedInfoUpdated -= embeddedInfoUpdatedHandler;
            embeddedInfoUpdatedHandler = null;
        }
        base.OnDisappearing();
    }

    private void RefreshEmbeddedPlaceholder() =>
        EmbeddedPlaceholder.IsVisible = !AirshipDotNet.Airship.InApp.IsEmbeddedAvailable("test");
}
