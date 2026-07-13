using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Logs
{
	internal static class LogLevelExtensions
	{
		[NullableContext(1)]
		public static string FastToString(this LogLevel level, [Nullable(2)] IFormatProvider provider = null)
		{
			string text;
			switch (level)
			{
			case LogLevel.Spam:
				text = "Spam";
				break;
			case LogLevel.Trace:
				text = "Trace";
				break;
			case LogLevel.Info:
				text = "Info";
				break;
			case LogLevel.Warning:
				text = "Warning";
				break;
			case LogLevel.Error:
				text = "Error";
				break;
			case LogLevel.Assert:
				text = "Assert";
				break;
			default:
			{
				int num = (int)level;
				text = num.ToString(provider);
				break;
			}
			}
			return text;
		}

		public const LogLevel MaxLevel = LogLevel.Assert;
	}
}
