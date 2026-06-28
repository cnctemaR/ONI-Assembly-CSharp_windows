using System;
using System.Collections;

namespace System.Data
{
	internal class TableStructureCollection : CollectionBase
	{
		public void Add(TableStructure table)
		{
			base.List.Add(table);
		}

		public TableStructure this[int i]
		{
			get
			{
				return base.List[i] as TableStructure;
			}
		}

		public TableStructure this[string name]
		{
			get
			{
				foreach (object obj in base.List)
				{
					TableStructure tableStructure = (TableStructure)obj;
					if (tableStructure.Table.TableName == name)
					{
						return tableStructure;
					}
				}
				return null;
			}
		}
	}
}
