using System;
using System.Diagnostics;

namespace System.Linq
{
	internal sealed class SystemLinq_LookupDebugView<TKey, TElement>
	{
		public SystemLinq_LookupDebugView(Lookup<TKey, TElement> lookup)
		{
			this._lookup = lookup;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public IGrouping<TKey, TElement>[] Groupings
		{
			get
			{
				IGrouping<TKey, TElement>[] array;
				if ((array = this._cachedGroupings) == null)
				{
					array = (this._cachedGroupings = this._lookup.ToArray<IGrouping<TKey, TElement>>());
				}
				return array;
			}
		}

		private readonly Lookup<TKey, TElement> _lookup;

		private IGrouping<TKey, TElement>[] _cachedGroupings;
	}
}
