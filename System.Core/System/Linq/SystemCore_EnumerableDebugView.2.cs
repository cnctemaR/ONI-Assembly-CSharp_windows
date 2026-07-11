using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq
{
	internal sealed class SystemCore_EnumerableDebugView
	{
		public SystemCore_EnumerableDebugView(IEnumerable enumerable)
		{
			if (enumerable == null)
			{
				throw Error.ArgumentNull("enumerable");
			}
			this._enumerable = enumerable;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public object[] Items
		{
			get
			{
				List<object> list = new List<object>();
				foreach (object obj in this._enumerable)
				{
					list.Add(obj);
				}
				if (list.Count == 0)
				{
					throw new SystemCore_EnumerableDebugViewEmptyException();
				}
				return list.ToArray();
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IEnumerable _enumerable;
	}
}
