using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Common
{
	public abstract class DbTransaction : MarshalByRefObject, IDbTransaction, IDisposable, IAsyncDisposable
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

		public virtual Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}
			Task task;
			try
			{
				this.Commit();
				task = Task.CompletedTask;
			}
			catch (Exception ex)
			{
				task = Task.FromException(ex);
			}
			return task;
		}

		public virtual ValueTask DisposeAsync()
		{
			this.Dispose();
			return default(ValueTask);
		}

		public virtual Task RollbackAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}
			Task task;
			try
			{
				this.Rollback();
				task = Task.CompletedTask;
			}
			catch (Exception ex)
			{
				task = Task.FromException(ex);
			}
			return task;
		}
	}
}
