using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[DebuggerDisplay("{_value}", Name = "[{_key}]")]
	[Serializable]
	public struct DictionaryEntry
	{
		public DictionaryEntry(object key, object value)
		{
			this._key = key;
			this._value = value;
		}

		public object Key
		{
			get
			{
				return this._key;
			}
			set
			{
				this._key = value;
			}
		}

		public object Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		private object _key;

		private object _value;
	}
}
