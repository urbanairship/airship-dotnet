using System.Windows.Input;
using AirshipDotNet;
using AirshipDotNet.MessageCenter;
using AirshipDotNet.Controls;
using Microsoft.Maui.Controls;

namespace MauiSample;

public partial class MessageCenterPage : ContentPage
{
    public ICommand RefreshCommand { private set; get; }

    public MessageCenterPage()
    {
        InitializeComponent();

        RefreshCommand = new Command(
            execute: () => Refresh(),
            canExecute: () => !refreshView.IsRefreshing
        );
    }

    protected override async void OnAppearing()
    {
        refreshView.BindingContext = this;
        await RefreshAsync();
    }

    public void Refresh()
    {
        // Fire and forget the async refresh
        _ = RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        try
        {
            var messages = await AirshipDotNet.Airship.MessageCenter.GetMessagesAsync();
            listView.ItemsSource = messages;
            refreshView.IsRefreshing = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error refreshing messages: {ex.Message}");
            refreshView.IsRefreshing = false;
        }
    }

    void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var message = e.SelectedItem as Message;

        var messagePage = new MessagePage
        {
            MessageId = message.MessageId
        };

        messagePage.LoadStarted += onLoadStarted;
        messagePage.LoadFinished += onLoadFinished;
        messagePage.Closed += onClosed;
        messagePage.LoadFailed += onLoadFailed;

        Navigation.PushAsync(messagePage);
    }

    private void onLoadStarted(object sender, MessageLoadStartedEventArgs e)
    {
        Console.WriteLine("MessageCenterPage onLoadStarted was reached.");
    }

    private void onLoadFinished(object sender, MessageLoadFinishedEventArgs e)
    {
        Console.WriteLine("MessageCenterPage onLoaded was reached.");
    }

    private void onClosed(object sender, MessageClosedEventArgs e)
    {
        Console.WriteLine("MessageCenterPage onClosed was reached.");
        Navigation.PopAsync();
    }

    private void onLoadFailed(object sender, MessageLoadFailedEventArgs e)
    {
        Console.WriteLine($"MessageCenterPage onLoadFailed was reached: {e.Error}");
    }
}