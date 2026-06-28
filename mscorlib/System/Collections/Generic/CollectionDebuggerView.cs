using System;
using System.Diagnostics;

namespace System.Collections.Generic
{
	internal sealed class CollectionDebuggerView<T>
	{
		public CollectionDebuggerView(ICollection<T> col)
		{
			this.c = col;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				T[] array = new T[this.c.Count];
				this.c.CopyTo(array, 0);
				return array;
			}
		}

		private readonly ICollection<T> c;
	}
}
