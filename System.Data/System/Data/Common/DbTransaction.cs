using System;

namespace System.Data.Common
{
	public abstract class DbTransaction : MarshalByRefObject, IDbTransaction, IDisposable
	{
		public DbConnection Connection
		{
			get
			{
				throw null;
			}
		}

		protected abstract DbConnection DbConnection { get; }

		public abstract IsolationLevel IsolationLevel { get; }

		IDbConnection IDbTransaction.Connection
		{
			get
			{
				throw null;
			}
		}

		public abstract void Commit();

		public void Dispose()
		{
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public abstract void Rollback();
	}
}
