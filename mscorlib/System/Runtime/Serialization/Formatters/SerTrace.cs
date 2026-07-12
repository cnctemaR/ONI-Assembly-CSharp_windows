using System;
using System.Diagnostics;

namespace System.Runtime.Serialization.Formatters
{
	internal static class SerTrace
	{
		[Conditional("_LOGGING")]
		internal static void InfoLog(params object[] messages)
		{
		}

		[Conditional("SER_LOGGING")]
		internal static void Log(params object[] messages)
		{
			if (!(messages[0] is string))
			{
				messages[0] = messages[0].GetType().Name + " ";
				return;
			}
			int num = 0;
			object obj = messages[0];
			messages[num] = ((obj != null) ? obj.ToString() : null) + " ";
		}
	}
}
