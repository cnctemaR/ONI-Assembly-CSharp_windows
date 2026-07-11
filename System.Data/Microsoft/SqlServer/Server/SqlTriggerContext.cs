using System;
using System.Data.Common;
using System.Data.SqlTypes;
using Unity;

namespace Microsoft.SqlServer.Server
{
	public sealed class SqlTriggerContext
	{
		internal SqlTriggerContext(TriggerAction triggerAction, bool[] columnsUpdated, SqlXml eventInstanceData)
		{
			this._triggerAction = triggerAction;
			this._columnsUpdated = columnsUpdated;
			this._eventInstanceData = eventInstanceData;
		}

		public int ColumnCount
		{
			get
			{
				int num = 0;
				if (this._columnsUpdated != null)
				{
					num = this._columnsUpdated.Length;
				}
				return num;
			}
		}

		public SqlXml EventData
		{
			get
			{
				return this._eventInstanceData;
			}
		}

		public TriggerAction TriggerAction
		{
			get
			{
				return this._triggerAction;
			}
		}

		public bool IsUpdatedColumn(int columnOrdinal)
		{
			if (this._columnsUpdated != null)
			{
				return this._columnsUpdated[columnOrdinal];
			}
			throw ADP.IndexOutOfRange(columnOrdinal);
		}

		internal SqlTriggerContext()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private TriggerAction _triggerAction;

		private bool[] _columnsUpdated;

		private SqlXml _eventInstanceData;
	}
}
