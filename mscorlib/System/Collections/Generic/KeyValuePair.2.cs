using System;

namespace System.Collections.Generic
{
	[Serializable]
	public readonly struct KeyValuePair<TKey, TValue>
	{
		public KeyValuePair(TKey key, TValue value)
		{
			this.key = key;
			this.value = value;
		}

		public TKey Key
		{
			get
			{
				return this.key;
			}
		}

		public TValue Value
		{
			get
			{
				return this.value;
			}
		}

		public override string ToString()
		{
			return KeyValuePair.PairToString(this.Key, this.Value);
		}

		public void Deconstruct(out TKey key, out TValue value)
		{
			key = this.Key;
			value = this.Value;
		}

		private readonly TKey key;

		private readonly TValue value;
	}
}
