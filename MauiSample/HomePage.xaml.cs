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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Track the screen view
        await AirshipDotNet.Airship.Analytics.TrackScreen("HomePage");
        
        viewModel.Refresh();
    }
}
