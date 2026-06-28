using System;
using System.Data.Common;

namespace System.Data.OleDb
{
	public sealed class OleDbTransaction : DbTransaction, IDisposable, IDbTransaction
	{
		internal OleDbTransaction(OleDbConnection connection, int depth)
			: this(connection, depth, IsolationLevel.ReadCommitted)
		{
		}

		internal OleDbTransaction(OleDbConnection connection)
			: this(connection, 1)
		{
		}

		internal OleDbTransaction(OleDbConnection connection, int depth, IsolationLevel isolevel)
		{
			this.connection = connection;
			this.gdaTransaction = libgda.gda_transaction_new(depth.ToString());
			if (isolevel != IsolationLevel.ReadUncommitted)
			{
				if (isolevel != IsolationLevel.ReadCommitted)
				{
					if (isolevel != IsolationLevel.RepeatableRead)
					{
						if (isolevel == IsolationLevel.Serializable)
						{
							libgda.gda_transaction_set_isolation_level(this.gdaTransaction, GdaTransactionIsolation.Serializable);
						}
					}
					else
					{
						libgda.gda_transaction_set_isolation_level(this.gdaTransaction, GdaTransactionIsolation.RepeatableRead);
					}
				}
				else
				{
					libgda.gda_transaction_set_isolation_level(this.gdaTransaction, GdaTransactionIsolation.ReadCommitted);
				}
			}
			else
			{
				libgda.gda_transaction_set_isolation_level(this.gdaTransaction, GdaTransactionIsolation.ReadUncommitted);
			}
			libgda.gda_connection_begin_transaction(connection.GdaConnection, this.gdaTransaction);
			this.isOpen = true;
		}

		internal OleDbTransaction(OleDbConnection connection, IsolationLevel isolevel)
			: this(connection, 1, isolevel)
		{
		}

		public new OleDbConnection Connection
		{
			get
			{
				return this.connection;
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return this.connection;
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
				switch (libgda.gda_transaction_get_isolation_level(this.gdaTransaction))
				{
				case GdaTransactionIsolation.ReadCommitted:
					return IsolationLevel.ReadCommitted;
				case GdaTransactionIsolation.ReadUncommitted:
					return IsolationLevel.ReadUncommitted;
				case GdaTransactionIsolation.RepeatableRead:
					return IsolationLevel.RepeatableRead;
				case GdaTransactionIsolation.Serializable:
					return IsolationLevel.Serializable;
				default:
					return IsolationLevel.Unspecified;
				}
			}
		}

		public OleDbTransaction Begin()
		{
			if (!this.isOpen)
			{
				throw ExceptionHelper.TransactionNotUsable(base.GetType());
			}
			return new OleDbTransaction(this.connection, this.depth + 1);
		}

		public OleDbTransaction Begin(IsolationLevel isolevel)
		{
			if (!this.isOpen)
			{
				throw ExceptionHelper.TransactionNotUsable(base.GetType());
			}
			return new OleDbTransaction(this.connection, this.depth + 1, isolevel);
		}

		public override void Commit()
		{
			if (!this.isOpen)
			{
				throw ExceptionHelper.TransactionNotUsable(base.GetType());
			}
			if (!libgda.gda_connection_commit_transaction(this.connection.GdaConnection, this.gdaTransaction))
			{
				throw new InvalidOperationException();
			}
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
			base.Dispose(disposing);
		}

		public override void Rollback()
		{
			if (!this.isOpen)
			{
				throw ExceptionHelper.TransactionNotUsable(base.GetType());
			}
			if (!libgda.gda_connection_rollback_transaction(this.connection.GdaConnection, this.gdaTransaction))
			{
				throw new InvalidOperationException();
			}
			this.connection = null;
			this.isOpen = false;
		}

		private bool disposed;

		private OleDbConnection connection;

		private IntPtr gdaTransaction;

		private int depth;

		private bool isOpen;
	}
}
