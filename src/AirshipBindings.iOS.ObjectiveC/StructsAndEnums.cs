using ObjCRuntime;

namespace Airship
{
	[Native]
	public enum UAAirshipLogLevel : long
	{
		Undefined = -1,
		None = 0,
		Error = 1,
		Warn = 2,
		Info = 3,
		Debug = 4,
		Verbose = 5
	}

	[Native]
	public enum UAChannelScope : long
	{
		App = 0,
		Web = 1,
		Email = 2,
		Sms = 3
	}

	[Native]
	public enum UAChannelType : long
	{
		Email = 0,
		Sms = 1,
		Open = 2
	}

	[Native]
	public enum UACloudSite : long
	{
		Us = 0,
		Eu = 1
	}
}
