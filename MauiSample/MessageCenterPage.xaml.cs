﻿using System.Windows.Input;
using AirshipDotNet;
using AirshipDotNet.MessageCenter;
using AirshipDotNet.MessageCenter.Controls;
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

    protected override void OnAppearing()
    {
        refreshView.BindingContext = this;
        Refresh();
    }

    public void Refresh()
    {
        AirshipDotNet.Airship.Instance.InboxMessages(messages =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                listView.ItemsSource = messages;
                refreshView.IsRefreshing = false;
            });
        });
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
        Console.WriteLine("MessageCenterPage onLoadFailed was reached.");
    }
}