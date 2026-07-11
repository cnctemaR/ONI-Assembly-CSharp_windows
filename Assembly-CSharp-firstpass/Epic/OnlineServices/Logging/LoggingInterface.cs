using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Logging
{
	public static class LoggingInterface
	{
		public static Result SetCallback(LogMessageFunc callback)
		{
			LogMessageFuncInternal logMessageFuncInternal = new LogMessageFuncInternal(LoggingInterface.LogMessageFunc);
			LoggingInterface.s_LogMessageFunc = callback;
			LoggingInterface.s_LogMessageFuncInternal = logMessageFuncInternal;
			Result result = LoggingInterface.EOS_Logging_SetCallback(logMessageFuncInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public static Result SetLogLevel(LogCategory logCategory, LogLevel logLevel)
		{
			Result result = LoggingInterface.EOS_Logging_SetLogLevel(logCategory, logLevel);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		[MonoPInvokeCallback]
		internal static void LogMessageFunc(IntPtr address)
		{
			LogMessage logMessage = null;
			if (Helper.TryMarshalGet<LogMessageInternal, LogMessage>(address, out logMessage))
			{
				LoggingInterface.s_LogMessageFunc(logMessage);
			}
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Logging_SetLogLevel(LogCategory logCategory, LogLevel logLevel);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Logging_SetCallback(LogMessageFuncInternal callback);

		private static LogMessageFuncInternal s_LogMessageFuncInternal;

		private static LogMessageFunc s_LogMessageFunc;
	}
}
