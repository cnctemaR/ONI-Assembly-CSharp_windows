using System;
using System.Collections;

namespace System.Data
{
	internal class TableMappingCollection : CollectionBase
	{
		public void Add(TableMapping map)
		{
			base.List.Add(map);
		}

		public TableMapping this[string name]
		{
			get
			{
				foreach (object obj in base.List)
				{
					TableMapping tableMapping = (TableMapping)obj;
					if (tableMapping.Table.TableName == name)
					{
						return tableMapping;
					}
				}
				return null;
			}
		}
	}
}
