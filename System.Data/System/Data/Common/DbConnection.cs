using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace System.Data.Common
{
	public abstract class DbConnection : Component, IDbConnection, IDisposable
	{
		[RecommendedAsConfigurable(true)]
		[SettingsBindable(true)]
		[DefaultValue("")]
		[RefreshProperties(RefreshProperties.All)]
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

		internal DbProviderFactory ProviderFactory
		{
			get
			{
				return this.DbProviderFactory;
			}
		}

		internal bool _suppressStateChangeForReconnection;
	}
}
