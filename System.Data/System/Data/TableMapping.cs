using System;
using System.Collections;

namespace System.Data
{
	internal class TableMapping
	{
		public TableMapping(string name, string ns)
		{
			this.Table = new DataTable(name);
			this.Table.Namespace = ns;
		}

		public TableMapping(DataTable dt)
		{
			this.existsInDataSet = true;
			this.Table = dt;
			foreach (object obj in dt.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				switch (dataColumn.ColumnMapping)
				{
				case MappingType.Element:
					this.Elements.Add(dataColumn);
					break;
				case MappingType.Attribute:
					this.Attributes.Add(dataColumn);
					break;
				case MappingType.SimpleContent:
					this.SimpleContent = dataColumn;
					break;
				}
			}
			this.PrimaryKey = ((dt.PrimaryKey.Length <= 0) ? null : dt.PrimaryKey[0]);
		}

		public bool ExistsInDataSet
		{
			get
			{
				return this.existsInDataSet;
			}
		}

		public bool ContainsColumn(string name)
		{
			return this.GetColumn(name) != null;
		}

		public DataColumn GetColumn(string name)
		{
			foreach (object obj in this.Elements)
			{
				DataColumn dataColumn = (DataColumn)obj;
				if (dataColumn.ColumnName == name)
				{
					return dataColumn;
				}
			}
			foreach (object obj2 in this.Attributes)
			{
				DataColumn dataColumn2 = (DataColumn)obj2;
				if (dataColumn2.ColumnName == name)
				{
					return dataColumn2;
				}
			}
			if (this.SimpleContent != null && name == this.SimpleContent.ColumnName)
			{
				return this.SimpleContent;
			}
			if (this.PrimaryKey != null && name == this.PrimaryKey.ColumnName)
			{
				return this.PrimaryKey;
			}
			return null;
		}

		public void RemoveElementColumn(string name)
		{
			foreach (object obj in this.Elements)
			{
				DataColumn dataColumn = (DataColumn)obj;
				if (dataColumn.ColumnName == name)
				{
					this.Elements.Remove(dataColumn);
					break;
				}
			}
		}

		private bool existsInDataSet;

		public DataTable Table;

		public ArrayList Elements = new ArrayList();

		public ArrayList Attributes = new ArrayList();

		public DataColumn SimpleContent;

		public DataColumn PrimaryKey;

		public DataColumn ReferenceKey;

		public int lastElementIndex = -1;

		public TableMapping ParentTable;

		public TableMappingCollection ChildTables = new TableMappingCollection();
	}
}
