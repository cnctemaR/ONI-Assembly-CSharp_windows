using System;
using System.Diagnostics;

namespace System.Collections.Concurrent
{
	internal sealed class IProducerConsumerCollectionDebugView<T>
	{
		public IProducerConsumerCollectionDebugView(IProducerConsumerCollection<T> collection)
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
				return this._collection.ToArray();
			}
		}

		private readonly IProducerConsumerCollection<T> _collection;
	}
}
