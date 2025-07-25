﻿using System;
using AirshipDotNet;

namespace MauiSample;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();

        // Register deep link event handler
        AirshipDotNet.Airship.Instance.OnDeepLinkReceived += OnDeepLinkReceived;
    }

    private void OnDeepLinkReceived(object sender, DeepLinkEventArgs e)
    {
        Uri uri = new Uri(e.DeepLink);
        Console.WriteLine("Deeplink Received! uri = " + uri);

        if (uri.Host.ToLower() == "deeplink")
        {
            var components = uri.AbsolutePath.ToLower().Split(separator:"/", StringSplitOptions.RemoveEmptyEntries);
            if (components.First() != null) {
                switch (components.First())
                {
                    case "home":
                        ((AppShell)App.Current.MainPage).SwitchtoTab(Tabs.homeTab);
                        return;

                    case "inbox":
                        ((AppShell)App.Current.MainPage).SwitchtoTab(Tabs.inboxTab);
                        return;

                    case "settings":
                        ((AppShell)App.Current.MainPage).SwitchtoTab(Tabs.settingsTab);
                        return;

                    default:
                        break;
                }
            }
        }

        Console.WriteLine("App does not know how to handle deepLink" + uri);
    }
}

