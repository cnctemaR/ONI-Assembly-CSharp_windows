using System;
using System.Data.Common;
using System.Threading.Tasks;
using System.Transactions;

namespace System.Data.ProviderBase
{
	internal abstract class DbConnectionClosed : DbConnectionInternal
	{
		protected DbConnectionClosed(ConnectionState state, bool hidePassword, bool allowSetConnectionString)
			: base(state, hidePassword, allowSetConnectionString)
		{
		}

		public override string ServerVersion
		{
			get
			{
				throw ADP.ClosedConnectionError();
			}
		}

		protected override void Activate(Transaction transaction)
		{
			throw ADP.ClosedConnectionError();
		}

		public override DbTransaction BeginTransaction(IsolationLevel il)
		{
			throw ADP.ClosedConnectionError();
		}

		public override void ChangeDatabase(string database)
		{
			throw ADP.ClosedConnectionError();
		}

		internal override void CloseConnection(DbConnection owningObject, DbConnectionFactory connectionFactory)
		{
		}

		protected override void Deactivate()
		{
			throw ADP.ClosedConnectionError();
		}

		public override void EnlistTransaction(Transaction transaction)
		{
			throw ADP.ClosedConnectionError();
		}

		protected override DbReferenceCollection CreateReferenceCollection()
		{
			throw ADP.ClosedConnectionError();
		}

		internal override bool TryOpenConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource<DbConnectionInternal> retry, DbConnectionOptions userOptions)
		{
			return base.TryOpenConnectionInternal(outerConnection, connectionFactory, retry, userOptions);
		}
	}
}
