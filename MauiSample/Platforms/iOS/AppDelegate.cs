using Foundation;
using ObjCRuntime;
using UIKit;
using UrbanAirship;
using System.Diagnostics;

namespace MauiSample;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
     
        // Populate AirshipConfig.plist with your app's info from https://go.urbanairship.com
        // or set runtime properties here.
        UAConfig config = UAConfig.DefaultConfig();

        // Log config details using Console.WriteLine which works with --console flag
        Console.WriteLine("🚀🚀🚀 AIRSHIP CONFIG LOADED 🚀🚀🚀");
        Console.WriteLine($"📱 App Key: {config.DefaultAppKey ?? "<null>"}");
        Console.WriteLine($"📱 App Secret: {config.DefaultAppSecret ?? "<null>"}");
        Console.WriteLine($"📱 In Production: {config.InProduction}");
        Console.WriteLine($"📱 Development Log Level: {config.DevelopmentLogLevel} (numeric: {(int)config.DevelopmentLogLevel})");
        Console.WriteLine($"📱 Production Log Level: {config.ProductionLogLevel} (numeric: {(int)config.ProductionLogLevel})");
        Console.WriteLine($"📱 Site: {config.Site} (numeric: {(int)config.Site})");
        Console.WriteLine("🚀🚀🚀 END CONFIG 🚀🚀🚀");

        // Set log level for debugging config loading (optional)
        // It will be set to the value in the loaded config upon takeOff
        UAirship.LogLevel = UALogLevel.Verbose;

        if (!config.Validate())
        {
            throw new RuntimeException("The AirshipConfig.plist must be a part of the app bundle and " +
                "include a valid appkey and secret for the selected production level.");
        }

        WarnIfSimulator();

        // Bootstrap the Airship SDK
        UAirship.TakeOff(config, launchOptions);

        // Log the actual runtime state after TakeOff
        Console.WriteLine("✅✅✅ AIRSHIP INITIALIZED ✅✅✅");
        Console.WriteLine($"✈️ UAirship.LogLevel after TakeOff: {UAirship.LogLevel} (numeric: {(int)UAirship.LogLevel})");
        Console.WriteLine($"✈️ Is Flying: {UAirship.IsFlying}");
        Console.WriteLine($"✈️ Channel ID: {UAirship.Channel?.Identifier ?? "<not yet created>"}");
        Console.WriteLine($"✈️ Shared Instance: {(UAirship.Shared != null ? "EXISTS" : "NULL")}");
        Console.WriteLine("✅✅✅ END INITIALIZATION ✅✅✅");

        UAirship.Push.ResetBadge();

        return base.FinishedLaunching(application, launchOptions);
    }

    private void WarnIfSimulator()
    {
        if (Runtime.Arch != Arch.SIMULATOR)
        {
            return;
        }

        Console.WriteLine("WARNING: You will not be able to receive push notifications in the simulator.");
    }
}
