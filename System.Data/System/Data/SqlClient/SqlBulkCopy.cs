using System;

namespace System.Data.SqlClient
{
	public sealed class SqlBulkCopy : IDisposable
	{
		public SqlBulkCopy(SqlConnection connection)
		{
		}

		[MonoTODO]
		public SqlBulkCopy(SqlConnection connection, SqlBulkCopyOptions copyOptions, SqlTransaction externalTransaction)
		{
		}

		public SqlBulkCopy(string connectionString)
		{
		}

		[MonoTODO]
		public SqlBulkCopy(string connectionString, SqlBulkCopyOptions copyOptions)
		{
		}

		public int BatchSize
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public int BulkCopyTimeout
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public SqlBulkCopyColumnMappingCollection ColumnMappings
		{
			get
			{
				throw null;
			}
		}

		public string DestinationTableName
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public int NotifyAfter
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public event SqlRowsCopiedEventHandler SqlRowsCopied
		{
			add
			{
			}
			remove
			{
			}
		}

		public void Close()
		{
		}

		void IDisposable.Dispose()
		{
		}

		public void WriteToServer(DataRow[] rows)
		{
		}

		public void WriteToServer(DataTable table)
		{
		}

		public void WriteToServer(DataTable table, DataRowState rowState)
		{
		}

		public void WriteToServer(IDataReader reader)
		{
		}
	}
}
