using System;

namespace System.Data.Common
{
	public abstract class DbTransaction : MarshalByRefObject, IDisposable, IDbTransaction
	{
		IDbConnection IDbTransaction.Connection
		{
			get
			{
				return this.Connection;
			}
		}

		public DbConnection Connection
		{
			get
			{
				return this.DbConnection;
			}
		}

		protected abstract DbConnection DbConnection { get; }

		public abstract IsolationLevel IsolationLevel { get; }

		public abstract void Commit();

		public abstract void Rollback();

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
