using System;
using System.Diagnostics;

namespace System.Collections.Generic
{
	internal sealed class DictionaryValueCollectionDebugView<TKey, TValue>
	{
		public DictionaryValueCollectionDebugView(ICollection<TValue> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			this._collection = collection;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public TValue[] Items
		{
			get
			{
				TValue[] array = new TValue[this._collection.Count];
				this._collection.CopyTo(array, 0);
				return array;
			}
		}

		private readonly ICollection<TValue> _collection;
	}
}
