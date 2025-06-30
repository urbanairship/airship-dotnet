using Foundation;
using ObjCRuntime;
using UIKit;
using Airship;
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
        // SDK 19: Load config from AirshipConfig.plist using DefaultConfigWithError
        NSError configError;
        UAConfig config = UAConfig.DefaultConfigWithError(out configError);
        
        if (config == null || configError != null)
        {
            throw new InvalidOperationException($"Failed to load Airship configuration: {configError?.LocalizedDescription ?? "Unknown error"}");
        }

        // Log config details using Console.WriteLine which works with --console flag
        Console.WriteLine("🚀🚀🚀 AIRSHIP CONFIG LOADED 🚀🚀🚀");
        Console.WriteLine($"📱 App Key: {config.DefaultAppKey ?? "<null>"}");
        Console.WriteLine($"📱 App Secret: {config.DefaultAppSecret ?? "<null>"}");
        Console.WriteLine($"📱 In Production: {config.InProduction}");
        Console.WriteLine($"📱 Development Log Level: {config.DevelopmentLogLevel} (numeric: {(int)config.DevelopmentLogLevel})");
        Console.WriteLine($"📱 Production Log Level: {config.ProductionLogLevel} (numeric: {(int)config.ProductionLogLevel})");
        Console.WriteLine($"📱 Site: {config.Site} (numeric: {(int)config.Site})");
        Console.WriteLine("🚀🚀🚀 END CONFIG 🚀🚀🚀");

        // SDK 19: LogLevel is now set in the config, not directly
        // Validation happens automatically

        WarnIfSimulator();

        // Bootstrap the Airship SDK
        // SDK 19: TakeOff now requires an error parameter
        NSError error;
        bool success = UAirship.TakeOff(config, launchOptions as NSDictionary<NSString, NSObject>, out error);
        
        if (!success || error != null)
        {
            throw new InvalidOperationException($"Failed to initialize Airship: {error?.LocalizedDescription ?? "Unknown error"}");
        }

        // Log the actual runtime state after TakeOff
        Console.WriteLine("✅✅✅ AIRSHIP INITIALIZED ✅✅✅");
        Console.WriteLine($"✈️ Log Level: {config.DevelopmentLogLevel} (development), {config.ProductionLogLevel} (production)");
        Console.WriteLine($"✈️ Is Flying: {success}");
        Console.WriteLine($"✈️ Channel ID: {UAirship.Channel?.Identifier ?? "<not yet created>"}");
        Console.WriteLine("✅✅✅ END INITIALIZATION ✅✅✅");

        // SDK 19: ResetBadge is now ResetBadgeWithCompletionHandler
        // TODO: Fix crash - "Unsupported type encoding: <v@?@"NSError">16"
        // This appears to be a binding issue with the completion handler
        /*
        UAirship.Push.ResetBadgeWithCompletionHandler((error) => 
        {
            if (error != null)
            {
                Console.WriteLine($"Failed to reset badge: {error.LocalizedDescription}");
            }
        });
        */

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
