using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	public interface IDictionaryEnumerator : IEnumerator
	{
		object Key { get; }

		object Value { get; }

		DictionaryEntry Entry { get; }
	}
}
