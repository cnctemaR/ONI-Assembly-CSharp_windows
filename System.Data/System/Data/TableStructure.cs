using System;
using System.Collections;

namespace System.Data
{
	internal class TableStructure
	{
		public TableStructure(DataTable table)
		{
			this.Table = table;
		}

		public bool ContainsColumn(string name)
		{
			foreach (object obj in this.NonOrdinalColumns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				if (dataColumn.ColumnName == name)
				{
					return true;
				}
			}
			foreach (object obj2 in this.OrdinalColumns.Keys)
			{
				DataColumn dataColumn2 = (DataColumn)obj2;
				if (dataColumn2.ColumnName == name)
				{
					return true;
				}
			}
			return false;
		}

		public DataTable Table;

		public Hashtable OrdinalColumns = new Hashtable();

		public ArrayList NonOrdinalColumns = new ArrayList();

		public DataColumn PrimaryKey;
	}
}
