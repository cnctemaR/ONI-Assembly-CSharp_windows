using System;
using System.Data.Common;

namespace System.Data.SqlClient
{
	public sealed class SqlTransaction : DbTransaction, IDisposable, IDbTransaction
	{
		internal SqlTransaction(SqlConnection connection, IsolationLevel isolevel)
		{
			this.connection = connection;
			this.isolationLevel = isolevel;
			this.isOpen = true;
		}

		public new SqlConnection Connection
		{
			get
			{
				return this.connection;
			}
		}

		internal bool IsOpen
		{
			get
			{
				return this.isOpen;
			}
		}

		public override IsolationLevel IsolationLevel
		{
			get
			{
				if (!this.isOpen)
				{
					throw ExceptionHelper.TransactionNotUsable(base.GetType());
				}
				return this.isolationLevel;
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return this.Connection;
			}
		}

		public override void Commit()
		{
			if (!this.isOpen)
			{
				throw ExceptionHelper.TransactionNotUsable(base.GetType());
			}
			this.connection.Tds.Execute("COMMIT TRANSACTION");
			this.connection.Transaction = null;
			this.connection = null;
			this.isOpen = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing && this.isOpen)
				{
					this.Rollback();
				}
				this.disposed = true;
			}
		}

		public override void Rollback()
		{
			this.Rollback(string.Empty);
		}

		public void Rollback(string transactionName)
		{
			if (this.disposed)
			{
				return;
			}
			if (!this.isOpen)
			{
				throw ExceptionHelper.TransactionNotUsable(base.GetType());
			}
			this.connection.Tds.Execute(string.Format("IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION {0}", transactionName));
			this.isOpen = false;
			this.connection.Transaction = null;
			this.connection = null;
		}

		public void Save(string savePointName)
		{
			if (!this.isOpen)
			{
				throw ExceptionHelper.TransactionNotUsable(base.GetType());
			}
			this.connection.Tds.Execute(string.Format("SAVE TRANSACTION {0}", savePointName));
		}

		private bool disposed;

		private SqlConnection connection;

		private IsolationLevel isolationLevel;

		private bool isOpen;
	}
}
