using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[Guid("496B0ABF-CDEE-11d3-88E8-00902754C43A")]
	[ComVisible(true)]
	public interface IEnumerator
	{
		bool MoveNext();

		object Current { get; }

		void Reset();
	}
}
