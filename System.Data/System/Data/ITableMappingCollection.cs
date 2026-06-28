using System;
using System.Collections;

namespace System.Data
{
	public interface ITableMappingCollection : IList, IEnumerable, ICollection
	{
		ITableMapping Add(string sourceTableName, string dataSetTableName);

		bool Contains(string sourceTableName);

		ITableMapping GetByDataSetTable(string dataSetTableName);

		int IndexOf(string sourceTableName);

		void RemoveAt(string sourceTableName);

		object this[string index] { get; set; }
	}
}
