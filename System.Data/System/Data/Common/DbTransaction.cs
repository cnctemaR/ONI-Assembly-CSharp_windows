using System;

namespace System.Data.Common
{
	public abstract class DbTransaction : MarshalByRefObject, IDbTransaction, IDisposable
	{
		public DbConnection Connection
		{
			get
			{
				return this.DbConnection;
			}
		}

		IDbConnection IDbTransaction.Connection
		{
			get
			{
				return this.DbConnection;
			}
		}

		protected abstract DbConnection DbConnection { get; }

		public abstract IsolationLevel IsolationLevel { get; }

		public abstract void Commit();

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public abstract void Rollback();
	}
}
