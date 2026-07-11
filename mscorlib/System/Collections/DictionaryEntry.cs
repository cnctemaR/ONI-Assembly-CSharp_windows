using System;

namespace System.Collections
{
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

		public void Deconstruct(out object key, out object value)
		{
			key = this.Key;
			value = this.Value;
		}

		private object _key;

		private object _value;
	}
}
