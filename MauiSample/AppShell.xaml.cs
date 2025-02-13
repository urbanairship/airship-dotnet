namespace MauiSample;

public enum Tabs
{
    homeTab,
    inboxTab,
    settingsTab
}

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
    }

    public void SwitchtoTab(Tabs item)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            switch (item)
            {
                case Tabs.homeTab:
                    Shell.Current.CurrentItem = home;
                    break;

                case Tabs.inboxTab:
                    Shell.Current.CurrentItem = inbox;
                    break;

                case Tabs.settingsTab:
                    Shell.Current.CurrentItem = settings;
                    break;
            }
        });
    }
}

