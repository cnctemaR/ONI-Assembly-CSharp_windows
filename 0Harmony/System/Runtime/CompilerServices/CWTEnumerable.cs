using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Runtime.CompilerServices
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class CWTEnumerable<TKey, [Nullable(2)] TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable where TKey : class where TValue : class
	{
		public CWTEnumerable(ConditionalWeakTable<TKey, TValue> table)
		{
			this.cwt = table;
		}

		[return: Nullable(new byte[] { 1, 0, 1, 1 })]
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return this.cwt.GetEnumerator<TKey, TValue>();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly ConditionalWeakTable<TKey, TValue> cwt;
	}
}
