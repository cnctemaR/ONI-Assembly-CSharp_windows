using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Collections.Concurrent
{
	internal sealed class IDictionaryDebugView<K, V>
	{
		public IDictionaryDebugView(IDictionary<K, V> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			this._dictionary = dictionary;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public KeyValuePair<K, V>[] Items
		{
			get
			{
				KeyValuePair<K, V>[] array = new KeyValuePair<K, V>[this._dictionary.Count];
				this._dictionary.CopyTo(array, 0);
				return array;
			}
		}

		private readonly IDictionary<K, V> _dictionary;
	}
}
