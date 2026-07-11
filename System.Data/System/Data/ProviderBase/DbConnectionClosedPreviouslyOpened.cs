using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace System.Data.ProviderBase
{
	internal sealed class DbConnectionClosedPreviouslyOpened : DbConnectionClosed
	{
		private DbConnectionClosedPreviouslyOpened()
			: base(ConnectionState.Closed, true, true)
		{
		}

		internal override bool TryReplaceConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource<DbConnectionInternal> retry, DbConnectionOptions userOptions)
		{
			return this.TryOpenConnection(outerConnection, connectionFactory, retry, userOptions);
		}

		internal static readonly DbConnectionInternal SingletonInstance = new DbConnectionClosedPreviouslyOpened();
	}
}
