using System;
using System.Diagnostics;

namespace System.Collections.Generic
{
	internal sealed class CollectionDebugView<T>
	{
		public CollectionDebugView(ICollection<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			this._collection = collection;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				T[] array = new T[this._collection.Count];
				this._collection.CopyTo(array, 0);
				return array;
			}
		}

		private readonly ICollection<T> _collection;
	}
}
