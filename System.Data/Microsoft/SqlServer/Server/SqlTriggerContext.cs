using System;
using System.Data.SqlTypes;

namespace Microsoft.SqlServer.Server
{
	public sealed class SqlTriggerContext
	{
		internal SqlTriggerContext(TriggerAction triggerAction, bool[] columnsUpdated, SqlXml eventData)
		{
			this.triggerAction = triggerAction;
			this.columnsUpdated = columnsUpdated;
			this.eventData = eventData;
		}

		public int ColumnCount
		{
			get
			{
				return (this.columnsUpdated != null) ? this.columnsUpdated.Length : 0;
			}
		}

		public SqlXml EventData
		{
			get
			{
				return this.eventData;
			}
		}

		public TriggerAction TriggerAction
		{
			get
			{
				return this.triggerAction;
			}
		}

		public bool IsUpdatedColumn(int columnOrdinal)
		{
			if (this.columnsUpdated == null)
			{
				throw new IndexOutOfRangeException("The index specified does not exist");
			}
			return this.columnsUpdated[columnOrdinal];
		}

		private TriggerAction triggerAction;

		private bool[] columnsUpdated;

		private SqlXml eventData;
	}
}
