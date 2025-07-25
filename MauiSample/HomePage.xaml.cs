using AirshipDotNet;
#if IOS
using Airship;
#endif

namespace MauiSample;

public partial class HomePage : ContentPage
{
    HomePageViewModel viewModel = new HomePageViewModel();

    public HomePage()
	{
		InitializeComponent();
        BindingContext = viewModel;
   	}

    protected override void OnAppearing()
    {
        viewModel.Refresh();
    }

    private async void OnRequestLocationPermission(object sender, EventArgs e)
    {
        try
        {
#if IOS
            // Request location permission using Airship's permission manager
            UAPermissionsManager.RequestPermission(UAPermission.Location, (status) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    string message = status switch
                    {
                        UAPermissionStatus.Granted => "Location permission granted!",
                        UAPermissionStatus.Denied => "Location permission denied.",
                        UAPermissionStatus.NotDetermined => "Location permission not determined.",
                        _ => "Unknown permission status."
                    };
                    await DisplayAlert("Location Permission", message, "OK");
                });
            });
#elif ANDROID
            // Android implementation would go here
            await DisplayAlert("Location Permission", "Android location permission handling not yet implemented.", "OK");
#else
            await DisplayAlert("Location Permission", "Location permissions are not supported on this platform.", "OK");
#endif
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to request permission: {ex.Message}", "OK");
        }
    }
}
