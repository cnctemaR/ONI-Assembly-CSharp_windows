using System;
using System.Diagnostics;

namespace System.Collections
{
	[DebuggerDisplay("{_value}", Name = "[{_key}]")]
	internal class KeyValuePairs
	{
		public KeyValuePairs(object key, object value)
		{
			this._value = value;
			this._key = key;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly object _key;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly object _value;
	}
}
