using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq
{
	internal sealed class SystemCore_EnumerableDebugView<T>
	{
		public SystemCore_EnumerableDebugView(IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				throw Error.ArgumentNull("enumerable");
			}
			this._enumerable = enumerable;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				T[] array = this._enumerable.ToArray<T>();
				if (array.Length == 0)
				{
					throw new SystemCore_EnumerableDebugViewEmptyException();
				}
				return array;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IEnumerable<T> _enumerable;
	}
}
