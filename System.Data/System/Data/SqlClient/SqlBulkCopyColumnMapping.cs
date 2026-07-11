using System;

namespace System.Data.SqlClient
{
	public sealed class SqlBulkCopyColumnMapping
	{
		public SqlBulkCopyColumnMapping()
		{
		}

		public SqlBulkCopyColumnMapping(int sourceColumnOrdinal, int destinationOrdinal)
		{
		}

		public SqlBulkCopyColumnMapping(int sourceColumnOrdinal, string destinationColumn)
		{
		}

		public SqlBulkCopyColumnMapping(string sourceColumn, int destinationOrdinal)
		{
		}

		public SqlBulkCopyColumnMapping(string sourceColumn, string destinationColumn)
		{
		}

		public string DestinationColumn
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public int DestinationOrdinal
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public string SourceColumn
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public int SourceOrdinal
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}
	}
}
