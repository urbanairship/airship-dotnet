using AirshipDotNet;

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
            // On iOS, the permission request is handled by the delegate we registered
            // This will trigger the system permission dialog
            await DisplayAlert("Location Permission", "The location permission request has been initiated. Please check the system dialog.", "OK");
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
