using System;

namespace System.Data.ProviderBase
{
	internal sealed class DbConnectionClosedBusy : DbConnectionBusy
	{
		private DbConnectionClosedBusy()
			: base(ConnectionState.Closed)
		{
		}

		internal static readonly DbConnectionInternal SingletonInstance = new DbConnectionClosedBusy();
	}
}
