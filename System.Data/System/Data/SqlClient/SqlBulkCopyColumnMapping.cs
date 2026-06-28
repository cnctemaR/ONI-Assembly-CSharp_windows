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
			this.SourceOrdinal = sourceColumnOrdinal;
			this.DestinationOrdinal = destinationOrdinal;
		}

		public SqlBulkCopyColumnMapping(int sourceColumnOrdinal, string destinationColumn)
		{
			this.SourceOrdinal = sourceColumnOrdinal;
			this.DestinationColumn = destinationColumn;
		}

		public SqlBulkCopyColumnMapping(string sourceColumn, int destinationOrdinal)
		{
			this.SourceColumn = sourceColumn;
			this.DestinationOrdinal = destinationOrdinal;
		}

		public SqlBulkCopyColumnMapping(string sourceColumn, string destinationColumn)
		{
			this.SourceColumn = sourceColumn;
			this.DestinationColumn = destinationColumn;
		}

		public string DestinationColumn
		{
			get
			{
				if (this.destinationColumn != null)
				{
					return this.destinationColumn;
				}
				return string.Empty;
			}
			set
			{
				this.destinationOrdinal = -1;
				this.destinationColumn = value;
			}
		}

		public string SourceColumn
		{
			get
			{
				if (this.sourceColumn != null)
				{
					return this.sourceColumn;
				}
				return string.Empty;
			}
			set
			{
				this.sourceOrdinal = -1;
				this.sourceColumn = value;
			}
		}

		public int DestinationOrdinal
		{
			get
			{
				return this.destinationOrdinal;
			}
			set
			{
				if (value < 0)
				{
					throw new IndexOutOfRangeException();
				}
				this.destinationColumn = null;
				this.destinationOrdinal = value;
			}
		}

		public int SourceOrdinal
		{
			get
			{
				return this.sourceOrdinal;
			}
			set
			{
				if (value < 0)
				{
					throw new IndexOutOfRangeException();
				}
				this.sourceColumn = null;
				this.sourceOrdinal = value;
			}
		}

		private int sourceOrdinal = -1;

		private int destinationOrdinal = -1;

		private string sourceColumn;

		private string destinationColumn;
	}
}
