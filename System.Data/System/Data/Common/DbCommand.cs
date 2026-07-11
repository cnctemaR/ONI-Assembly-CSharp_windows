using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Common
{
	public abstract class DbCommand : Component, IDbCommand, IDisposable
	{
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue("")]
		public abstract string CommandText { get; set; }

		public abstract int CommandTimeout { get; set; }

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(CommandType.Text)]
		public abstract CommandType CommandType { get; set; }

		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public DbConnection Connection
		{
			get
			{
				return this.DbConnection;
			}
			set
			{
				this.DbConnection = value;
			}
		}

		IDbConnection IDbCommand.Connection
		{
			get
			{
				return this.DbConnection;
			}
			set
			{
				this.DbConnection = (DbConnection)value;
			}
		}

		protected abstract DbConnection DbConnection { get; set; }

		protected abstract DbParameterCollection DbParameterCollection { get; }

		protected abstract DbTransaction DbTransaction { get; set; }

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignOnly(true)]
		[DefaultValue(true)]
		public abstract bool DesignTimeVisible { get; set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public DbParameterCollection Parameters
		{
			get
			{
				return this.DbParameterCollection;
			}
		}

		IDataParameterCollection IDbCommand.Parameters
		{
			get
			{
				return this.DbParameterCollection;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(null)]
		public DbTransaction Transaction
		{
			get
			{
				return this.DbTransaction;
			}
			set
			{
				this.DbTransaction = value;
			}
		}

		IDbTransaction IDbCommand.Transaction
		{
			get
			{
				return this.DbTransaction;
			}
			set
			{
				this.DbTransaction = (DbTransaction)value;
			}
		}

		[DefaultValue(UpdateRowSource.Both)]
		public abstract UpdateRowSource UpdatedRowSource { get; set; }

		internal void CancelIgnoreFailure()
		{
			try
			{
				this.Cancel();
			}
			catch (Exception)
			{
			}
		}

		public abstract void Cancel();

		public DbParameter CreateParameter()
		{
			return this.CreateDbParameter();
		}

		IDbDataParameter IDbCommand.CreateParameter()
		{
			return this.CreateDbParameter();
		}

		protected abstract DbParameter CreateDbParameter();

		protected abstract DbDataReader ExecuteDbDataReader(CommandBehavior behavior);

		public abstract int ExecuteNonQuery();

		public DbDataReader ExecuteReader()
		{
			return this.ExecuteDbDataReader(CommandBehavior.Default);
		}

		IDataReader IDbCommand.ExecuteReader()
		{
			return this.ExecuteDbDataReader(CommandBehavior.Default);
		}

		public DbDataReader ExecuteReader(CommandBehavior behavior)
		{
			return this.ExecuteDbDataReader(behavior);
		}

		IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
		{
			return this.ExecuteDbDataReader(behavior);
		}

		public Task<int> ExecuteNonQueryAsync()
		{
			return this.ExecuteNonQueryAsync(CancellationToken.None);
		}

		public virtual Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return ADP.CreatedTaskWithCancellation<int>();
			}
			CancellationTokenRegistration cancellationTokenRegistration = default(CancellationTokenRegistration);
			if (cancellationToken.CanBeCanceled)
			{
				cancellationTokenRegistration = cancellationToken.Register(delegate(object s)
				{
					((DbCommand)s).CancelIgnoreFailure();
				}, this);
			}
			Task<int> task;
			try
			{
				task = Task.FromResult<int>(this.ExecuteNonQuery());
			}
			catch (Exception ex)
			{
				task = Task.FromException<int>(ex);
			}
			finally
			{
				cancellationTokenRegistration.Dispose();
			}
			return task;
		}

		public Task<DbDataReader> ExecuteReaderAsync()
		{
			return this.ExecuteReaderAsync(CommandBehavior.Default, CancellationToken.None);
		}

		public Task<DbDataReader> ExecuteReaderAsync(CancellationToken cancellationToken)
		{
			return this.ExecuteReaderAsync(CommandBehavior.Default, cancellationToken);
		}

		public Task<DbDataReader> ExecuteReaderAsync(CommandBehavior behavior)
		{
			return this.ExecuteReaderAsync(behavior, CancellationToken.None);
		}

		public Task<DbDataReader> ExecuteReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
		{
			return this.ExecuteDbDataReaderAsync(behavior, cancellationToken);
		}

		protected virtual Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return ADP.CreatedTaskWithCancellation<DbDataReader>();
			}
			CancellationTokenRegistration cancellationTokenRegistration = default(CancellationTokenRegistration);
			if (cancellationToken.CanBeCanceled)
			{
				cancellationTokenRegistration = cancellationToken.Register(delegate(object s)
				{
					((DbCommand)s).CancelIgnoreFailure();
				}, this);
			}
			Task<DbDataReader> task;
			try
			{
				task = Task.FromResult<DbDataReader>(this.ExecuteReader(behavior));
			}
			catch (Exception ex)
			{
				task = Task.FromException<DbDataReader>(ex);
			}
			finally
			{
				cancellationTokenRegistration.Dispose();
			}
			return task;
		}

		public Task<object> ExecuteScalarAsync()
		{
			return this.ExecuteScalarAsync(CancellationToken.None);
		}

		public virtual Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return ADP.CreatedTaskWithCancellation<object>();
			}
			CancellationTokenRegistration cancellationTokenRegistration = default(CancellationTokenRegistration);
			if (cancellationToken.CanBeCanceled)
			{
				cancellationTokenRegistration = cancellationToken.Register(delegate(object s)
				{
					((DbCommand)s).CancelIgnoreFailure();
				}, this);
			}
			Task<object> task;
			try
			{
				task = Task.FromResult<object>(this.ExecuteScalar());
			}
			catch (Exception ex)
			{
				task = Task.FromException<object>(ex);
			}
			finally
			{
				cancellationTokenRegistration.Dispose();
			}
			return task;
		}

		public abstract object ExecuteScalar();

		public abstract void Prepare();
	}
}
