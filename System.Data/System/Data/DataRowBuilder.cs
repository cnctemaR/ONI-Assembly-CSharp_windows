using System;
using Unity;

namespace System.Data
{
	public sealed class DataRowBuilder
	{
		internal DataRowBuilder(DataTable table, int record)
		{
			this._table = table;
			this._record = record;
		}

		internal DataRowBuilder()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		internal readonly DataTable _table;

		internal int _record;
	}
}
