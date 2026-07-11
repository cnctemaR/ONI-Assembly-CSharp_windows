using System;

namespace System.Data.ProviderBase
{
	internal sealed class DbConnectionClosedNeverOpened : DbConnectionClosed
	{
		private DbConnectionClosedNeverOpened()
			: base(ConnectionState.Closed, false, true)
		{
		}

		internal static readonly DbConnectionInternal SingletonInstance = new DbConnectionClosedNeverOpened();
	}
}
