using System;
using System.ComponentModel;

namespace System.Data.Common
{
	public abstract class DbCommand : Component, IDbCommand, IDisposable
	{
		[DefaultValue("")]
		[RefreshProperties(RefreshProperties.All)]
		public abstract string CommandText { get; set; }

		public abstract int CommandTimeout { get; set; }

		[DefaultValue(CommandType.Text)]
		[RefreshProperties(RefreshProperties.All)]
		public abstract CommandType CommandType { get; set; }

		[Browsable(false)]
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DbConnection Connection
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		protected abstract DbConnection DbConnection { get; set; }

		protected abstract DbParameterCollection DbParameterCollection { get; }

		protected abstract DbTransaction DbTransaction { get; set; }

		[Browsable(false)]
		[DefaultValue(true)]
		[DesignOnly(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract bool DesignTimeVisible { get; set; }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DbParameterCollection Parameters
		{
			get
			{
				throw null;
			}
		}

		IDbConnection IDbCommand.Connection
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		IDataParameterCollection IDbCommand.Parameters
		{
			get
			{
				throw null;
			}
		}

		IDbTransaction IDbCommand.Transaction
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DbTransaction Transaction
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(UpdateRowSource.Both)]
		public abstract UpdateRowSource UpdatedRowSource { get; set; }

		public abstract void Cancel();

		protected abstract DbParameter CreateDbParameter();

		public DbParameter CreateParameter()
		{
			throw null;
		}

		protected abstract DbDataReader ExecuteDbDataReader(CommandBehavior behavior);

		public abstract int ExecuteNonQuery();

		public DbDataReader ExecuteReader()
		{
			throw null;
		}

		public DbDataReader ExecuteReader(CommandBehavior behavior)
		{
			throw null;
		}

		public abstract object ExecuteScalar();

		public abstract void Prepare();

		IDbDataParameter IDbCommand.CreateParameter()
		{
			throw null;
		}

		IDataReader IDbCommand.ExecuteReader()
		{
			throw null;
		}

		IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
		{
			throw null;
		}
	}
}
