using System;
using System.Collections.Generic;

namespace System.Runtime.CompilerServices
{
	[NullableContext(1)]
	internal interface ICWTEnumerable<[Nullable(2)] T>
	{
		IEnumerable<T> SelfEnumerable { get; }

		IEnumerator<T> GetEnumerator();
	}
}
