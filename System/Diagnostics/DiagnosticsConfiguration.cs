using System;
using System.Collections;
using System.Configuration;
using System.Threading;

namespace System.Diagnostics
{
	internal sealed class DiagnosticsConfiguration
	{
		public static IDictionary Settings
		{
			get
			{
				if (DiagnosticsConfiguration.settings == null)
				{
					object config = global::System.Configuration.ConfigurationSettings.GetConfig("system.diagnostics");
					if (config == null)
					{
						throw new Exception("INTERNAL configuration error: failed to get configuration 'system.diagnostics'");
					}
					Thread.MemoryBarrier();
					while (Interlocked.CompareExchange(ref DiagnosticsConfiguration.settings, config, null) == null)
					{
					}
					Thread.MemoryBarrier();
				}
				return (IDictionary)DiagnosticsConfiguration.settings;
			}
		}

		private static object settings;
	}
}
