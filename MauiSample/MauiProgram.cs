using AirshipDotNet;
using AirshipDotNet.Embedded;
using AirshipDotNet.MessageCenter;

namespace MauiSample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseAirshipMessageCenter()
            .UseAirshipEmbedded()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("RobotoMono-Regular.ttf", "RobotoMonoRegular");
            });

		return builder.Build();
	}
}
