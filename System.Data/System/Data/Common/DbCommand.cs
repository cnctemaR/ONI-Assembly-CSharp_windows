using System;
using System.ComponentModel;

namespace System.Data.Common
{
	public abstract class DbCommand : Component, IDisposable, IDbCommand
	{
		IDbConnection IDbCommand.Connection
		{
			get
			{
				return this.Connection;
			}
			set
			{
				this.Connection = (DbConnection)value;
			}
		}

		IDataParameterCollection IDbCommand.Parameters
		{
			get
			{
				return this.Parameters;
			}
		}

		IDbTransaction IDbCommand.Transaction
		{
			get
			{
				return this.Transaction;
			}
			set
			{
				this.Transaction = (DbTransaction)value;
			}
		}

		IDbDataParameter IDbCommand.CreateParameter()
		{
			return this.CreateParameter();
		}

		IDataReader IDbCommand.ExecuteReader()
		{
			return this.ExecuteReader();
		}

		IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
		{
			return this.ExecuteReader(behavior);
		}

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

		protected abstract DbConnection DbConnection { get; set; }

		protected abstract DbParameterCollection DbParameterCollection { get; }

		protected abstract DbTransaction DbTransaction { get; set; }

		[DefaultValue(true)]
		[Browsable(false)]
		[DesignOnly(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(null)]
		[Browsable(false)]
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

		[DefaultValue(UpdateRowSource.Both)]
		public abstract UpdateRowSource UpdatedRowSource { get; set; }

		public abstract void Cancel();

		protected abstract DbParameter CreateDbParameter();

		public DbParameter CreateParameter()
		{
			return this.CreateDbParameter();
		}

		protected abstract DbDataReader ExecuteDbDataReader(CommandBehavior behavior);

		public abstract int ExecuteNonQuery();

		public DbDataReader ExecuteReader()
		{
			return this.ExecuteDbDataReader(CommandBehavior.Default);
		}

		public DbDataReader ExecuteReader(CommandBehavior behavior)
		{
			return this.ExecuteDbDataReader(behavior);
		}

		public abstract object ExecuteScalar();

		public abstract void Prepare();
	}
}
