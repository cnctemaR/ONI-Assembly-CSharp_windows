using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace System.Data.Common
{
	public abstract class DbConnection : Component, IDbConnection, IDisposable, IAsyncDisposable
	{
		[DefaultValue("")]
		[SettingsBindable(true)]
		[RefreshProperties(RefreshProperties.All)]
		[RecommendedAsConfigurable(true)]
		public abstract string ConnectionString { get; set; }

		public virtual int ConnectionTimeout
		{
			get
			{
				return 15;
			}
		}

		public abstract string Database { get; }

		public abstract string DataSource { get; }

		protected virtual DbProviderFactory DbProviderFactory
		{
			get
			{
				return null;
			}
		}

		internal DbProviderFactory ProviderFactory
		{
			get
			{
				return this.DbProviderFactory;
			}
		}

		[Browsable(false)]
		public abstract string ServerVersion { get; }

		[Browsable(false)]
		public abstract ConnectionState State { get; }

		public virtual event StateChangeEventHandler StateChange;

		protected abstract DbTransaction BeginDbTransaction(IsolationLevel isolationLevel);

		public DbTransaction BeginTransaction()
		{
			return this.BeginDbTransaction(IsolationLevel.Unspecified);
		}

		public DbTransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			return this.BeginDbTransaction(isolationLevel);
		}

		IDbTransaction IDbConnection.BeginTransaction()
		{
			return this.BeginDbTransaction(IsolationLevel.Unspecified);
		}

		IDbTransaction IDbConnection.BeginTransaction(IsolationLevel isolationLevel)
		{
			return this.BeginDbTransaction(isolationLevel);
		}

		public abstract void Close();

		public abstract void ChangeDatabase(string databaseName);

		public DbCommand CreateCommand()
		{
			return this.CreateDbCommand();
		}

		IDbCommand IDbConnection.CreateCommand()
		{
			return this.CreateDbCommand();
		}

		protected abstract DbCommand CreateDbCommand();

		public virtual void EnlistTransaction(Transaction transaction)
		{
			throw ADP.NotSupported();
		}

		public virtual DataTable GetSchema()
		{
			throw ADP.NotSupported();
		}

		public virtual DataTable GetSchema(string collectionName)
		{
			throw ADP.NotSupported();
		}

		public virtual DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			throw ADP.NotSupported();
		}

		protected virtual void OnStateChange(StateChangeEventArgs stateChange)
		{
			if (this._suppressStateChangeForReconnection)
			{
				return;
			}
			StateChangeEventHandler stateChange2 = this.StateChange;
			if (stateChange2 == null)
			{
				return;
			}
			stateChange2(this, stateChange);
		}

		public abstract void Open();

		public Task OpenAsync()
		{
			return this.OpenAsync(CancellationToken.None);
		}

		public virtual Task OpenAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}
			Task task;
			try
			{
				this.Open();
				task = Task.CompletedTask;
			}
			catch (Exception ex)
			{
				task = Task.FromException(ex);
			}
			return task;
		}

		protected virtual ValueTask<DbTransaction> BeginDbTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<DbTransaction>(Task.FromCanceled<DbTransaction>(cancellationToken));
			}
			ValueTask<DbTransaction> valueTask;
			try
			{
				valueTask = new ValueTask<DbTransaction>(this.BeginDbTransaction(isolationLevel));
			}
			catch (Exception ex)
			{
				valueTask = new ValueTask<DbTransaction>(Task.FromException<DbTransaction>(ex));
			}
			return valueTask;
		}

		public ValueTask<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return this.BeginDbTransactionAsync(IsolationLevel.Unspecified, cancellationToken);
		}

		public ValueTask<DbTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default(CancellationToken))
		{
			return this.BeginDbTransactionAsync(isolationLevel, cancellationToken);
		}

		public virtual Task CloseAsync()
		{
			Task task;
			try
			{
				this.Close();
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
			base.Dispose();
			return default(ValueTask);
		}

		public virtual Task ChangeDatabaseAsync(string databaseName, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}
			Task task;
			try
			{
				this.ChangeDatabase(databaseName);
				task = Task.CompletedTask;
			}
			catch (Exception ex)
			{
				task = Task.FromException(ex);
			}
			return task;
		}

		internal bool _suppressStateChangeForReconnection;
	}
}
