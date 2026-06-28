using System;
using System.Collections;

namespace System.Data
{
	public interface IDataParameterCollection : IList, IEnumerable, ICollection
	{
		void RemoveAt(string parameterName);

		int IndexOf(string parameterName);

		bool Contains(string parameterName);

		object this[string parameterName] { get; set; }
	}
}
