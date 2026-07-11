using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Serialization.Formatters
{
	[ComVisible(true)]
	[SecurityCritical]
	public sealed class InternalRM
	{
		[Conditional("_LOGGING")]
		public static void InfoSoap(params object[] messages)
		{
		}

		public static bool SoapCheckEnabled()
		{
			return BCLDebug.CheckEnabled("SOAP");
		}
	}
}
