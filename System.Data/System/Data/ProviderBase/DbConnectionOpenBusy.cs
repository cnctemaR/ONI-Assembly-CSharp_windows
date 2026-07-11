using System;

namespace System.Data.ProviderBase
{
	internal sealed class DbConnectionOpenBusy : DbConnectionBusy
	{
		private DbConnectionOpenBusy()
			: base(ConnectionState.Open)
		{
		}

		internal static readonly DbConnectionInternal SingletonInstance = new DbConnectionOpenBusy();
	}
}
