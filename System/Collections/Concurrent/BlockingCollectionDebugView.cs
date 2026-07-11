using System;
using System.Diagnostics;

namespace System.Collections.Concurrent
{
	internal sealed class BlockingCollectionDebugView<T>
	{
		public BlockingCollectionDebugView(BlockingCollection<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			this._blockingCollection = collection;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				return this._blockingCollection.ToArray();
			}
		}

		private readonly BlockingCollection<T> _blockingCollection;
	}
}
