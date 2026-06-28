using System;
using System.Collections;

namespace System.Data
{
	public interface IColumnMappingCollection : IList, IEnumerable, ICollection
	{
		IColumnMapping Add(string sourceColumnName, string dataSetColumnName);

		bool Contains(string sourceColumnName);

		IColumnMapping GetByDataSetColumn(string dataSetColumnName);

		int IndexOf(string sourceColumnName);

		void RemoveAt(string sourceColumnName);

		object this[string index] { get; set; }
	}
}
