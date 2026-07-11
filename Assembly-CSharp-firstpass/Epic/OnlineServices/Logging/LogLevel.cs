using System;

namespace Epic.OnlineServices.Logging
{
	public enum LogLevel
	{
		Off,
		Fatal = 100,
		Error = 200,
		Warning = 300,
		Info = 400,
		Verbose = 500,
		VeryVerbose = 600
	}
}
