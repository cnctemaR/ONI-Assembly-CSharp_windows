using System;
using System.Data.Common;

namespace System.Data.OleDb
{
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbCommand : DbCommand, IDbCommand, IDisposable, ICloneable
	{
		public OleDbCommand()
		{
		}

		public OleDbCommand(string cmdText)
		{
			throw ADP.OleDb();
		}

		public OleDbCommand(string cmdText, OleDbConnection connection)
		{
			throw ADP.OleDb();
		}

		public OleDbCommand(string cmdText, OleDbConnection connection, OleDbTransaction transaction)
		{
			throw ADP.OleDb();
		}

		public override string CommandText
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public override int CommandTimeout
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public override CommandType CommandType
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public new OleDbConnection Connection
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		protected override DbParameterCollection DbParameterCollection
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		protected override DbTransaction DbTransaction
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public override bool DesignTimeVisible
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public new OleDbParameterCollection Parameters
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public new OleDbTransaction Transaction
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public override UpdateRowSource UpdatedRowSource
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public override void Cancel()
		{
		}

		public OleDbCommand Clone()
		{
			throw ADP.OleDb();
		}

		protected override DbParameter CreateDbParameter()
		{
			throw ADP.OleDb();
		}

		public new OleDbParameter CreateParameter()
		{
			throw ADP.OleDb();
		}

		protected override void Dispose(bool disposing)
		{
			throw ADP.OleDb();
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			throw ADP.OleDb();
		}

		public override int ExecuteNonQuery()
		{
			throw ADP.OleDb();
		}

		public new OleDbDataReader ExecuteReader()
		{
			throw ADP.OleDb();
		}

		public new OleDbDataReader ExecuteReader(CommandBehavior behavior)
		{
			throw ADP.OleDb();
		}

		public override object ExecuteScalar()
		{
			throw ADP.OleDb();
		}

		public override void Prepare()
		{
			throw ADP.OleDb();
		}

		public void ResetCommandTimeout()
		{
			throw ADP.OleDb();
		}

		IDataReader IDbCommand.ExecuteReader()
		{
			throw ADP.OleDb();
		}

		IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
		{
			throw ADP.OleDb();
		}

		object ICloneable.Clone()
		{
			throw ADP.OleDb();
		}
	}
}
