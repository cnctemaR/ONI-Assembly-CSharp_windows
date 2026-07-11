using System;
using System.Threading;

namespace System.Data.Odbc
{
	internal sealed class OdbcEnvironment
	{
		private OdbcEnvironment()
		{
		}

		internal static OdbcEnvironmentHandle GetGlobalEnvironmentHandle()
		{
			OdbcEnvironmentHandle odbcEnvironmentHandle = OdbcEnvironment.s_globalEnvironmentHandle as OdbcEnvironmentHandle;
			if (odbcEnvironmentHandle == null)
			{
				object obj = OdbcEnvironment.s_globalEnvironmentHandleLock;
				lock (obj)
				{
					odbcEnvironmentHandle = OdbcEnvironment.s_globalEnvironmentHandle as OdbcEnvironmentHandle;
					if (odbcEnvironmentHandle == null)
					{
						odbcEnvironmentHandle = new OdbcEnvironmentHandle();
						OdbcEnvironment.s_globalEnvironmentHandle = odbcEnvironmentHandle;
					}
				}
			}
			return odbcEnvironmentHandle;
		}

		internal static void ReleaseObjectPool()
		{
			object obj = Interlocked.Exchange(ref OdbcEnvironment.s_globalEnvironmentHandle, null);
			if (obj != null)
			{
				(obj as OdbcEnvironmentHandle).Dispose();
			}
		}

		private static object s_globalEnvironmentHandle;

		private static object s_globalEnvironmentHandleLock = new object();
	}
}
