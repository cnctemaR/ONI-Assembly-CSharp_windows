using System;
using System.Security.Principal;

namespace Microsoft.SqlServer.Server
{
	public sealed class SqlContext
	{
		public static bool IsAvailable
		{
			get
			{
				return false;
			}
		}

		public static SqlPipe Pipe
		{
			get
			{
				return null;
			}
		}

		public static SqlTriggerContext TriggerContext
		{
			get
			{
				return null;
			}
		}

		public static WindowsIdentity WindowsIdentity
		{
			get
			{
				return null;
			}
		}
	}
}
