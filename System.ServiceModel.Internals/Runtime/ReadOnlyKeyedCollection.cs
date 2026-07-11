using System;
using System.Collections.ObjectModel;

namespace System.Runtime
{
	internal class ReadOnlyKeyedCollection<TKey, TValue> : ReadOnlyCollection<TValue>
	{
		public ReadOnlyKeyedCollection(KeyedCollection<TKey, TValue> innerCollection)
			: base(innerCollection)
		{
			this.innerCollection = innerCollection;
		}

		public TValue this[TKey key]
		{
			get
			{
				return this.innerCollection[key];
			}
		}

		private KeyedCollection<TKey, TValue> innerCollection;
	}
}
