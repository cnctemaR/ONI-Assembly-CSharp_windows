using System;
using System.Data.SqlTypes;

namespace Microsoft.SqlServer.Server
{
	public sealed class SqlTriggerContext
	{
		internal SqlTriggerContext()
		{
		}

		public int ColumnCount
		{
			get
			{
				throw null;
			}
		}

		public SqlXml EventData
		{
			get
			{
				throw null;
			}
		}

		public TriggerAction TriggerAction
		{
			get
			{
				throw null;
			}
		}

		public bool IsUpdatedColumn(int columnOrdinal)
		{
			throw null;
		}
	}
}
