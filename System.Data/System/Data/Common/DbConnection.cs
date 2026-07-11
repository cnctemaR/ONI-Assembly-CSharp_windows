using System;
using System.ComponentModel;
using System.Transactions;

namespace System.Data.Common
{
	public abstract class DbConnection : Component, IDbConnection, IDisposable
	{
		[DefaultValue("")]
		[RecommendedAsConfigurable(true)]
		[RefreshProperties(RefreshProperties.All)]
		public abstract string ConnectionString { get; set; }

		public virtual int ConnectionTimeout
		{
			get
			{
				throw null;
			}
		}

		public abstract string Database { get; }

		public abstract string DataSource { get; }

		protected internal virtual DbProviderFactory DbProviderFactory
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		public abstract string ServerVersion { get; }

		[Browsable(false)]
		public abstract ConnectionState State { get; }

		public virtual event StateChangeEventHandler StateChange
		{
			add
			{
			}
			remove
			{
			}
		}

		protected abstract DbTransaction BeginDbTransaction(IsolationLevel isolationLevel);

		public DbTransaction BeginTransaction()
		{
			throw null;
		}

		public DbTransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			throw null;
		}

		public abstract void ChangeDatabase(string databaseName);

		public abstract void Close();

		public DbCommand CreateCommand()
		{
			throw null;
		}

		protected abstract DbCommand CreateDbCommand();

		public virtual void EnlistTransaction(Transaction transaction)
		{
		}

		public virtual DataTable GetSchema()
		{
			throw null;
		}

		public virtual DataTable GetSchema(string collectionName)
		{
			throw null;
		}

		public virtual DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			throw null;
		}

		protected virtual void OnStateChange(StateChangeEventArgs stateChange)
		{
		}

		public abstract void Open();

		IDbTransaction IDbConnection.BeginTransaction()
		{
			throw null;
		}

		IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
		{
			throw null;
		}

		IDbCommand IDbConnection.CreateCommand()
		{
			throw null;
		}
	}
}
