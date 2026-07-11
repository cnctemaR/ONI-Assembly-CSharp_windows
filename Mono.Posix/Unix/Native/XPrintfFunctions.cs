using System;

namespace Mono.Unix.Native
{
	internal class XPrintfFunctions
	{
		internal static XPrintfFunctions.XPrintf printf = new XPrintfFunctions.XPrintf(new CdeclFunction("msvcrt", "printf", typeof(int)).Invoke);

		internal static XPrintfFunctions.XPrintf fprintf = new XPrintfFunctions.XPrintf(new CdeclFunction("msvcrt", "fprintf", typeof(int)).Invoke);

		internal static XPrintfFunctions.XPrintf snprintf = new XPrintfFunctions.XPrintf(new CdeclFunction("MonoPosixHelper", "Mono_Posix_Stdlib_snprintf", typeof(int)).Invoke);

		internal static XPrintfFunctions.XPrintf syslog = new XPrintfFunctions.XPrintf(new CdeclFunction("MonoPosixHelper", "Mono_Posix_Stdlib_syslog2", typeof(int)).Invoke);

		internal delegate object XPrintf(object[] parameters);
	}
}
