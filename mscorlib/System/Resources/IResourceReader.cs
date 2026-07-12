using System;
using System.Collections;

namespace System.Resources
{
	public interface IResourceReader : IEnumerable, IDisposable
	{
		void Close();

		IDictionaryEnumerator GetEnumerator();
	}
}
