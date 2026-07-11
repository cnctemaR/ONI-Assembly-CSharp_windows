using System;
using System.Diagnostics;

namespace System.Collections.Generic
{
	[DebuggerDisplay("{value}", Name = "[{key}]")]
	[Serializable]
	public struct KeyValuePair<TKey, TValue>
	{
		public KeyValuePair(TKey key, TValue value)
		{
			this.Key = key;
			this.Value = value;
		}

		public TKey Key
		{
			get
			{
				return this.key;
			}
			private set
			{
				this.key = value;
			}
		}

		public TValue Value
		{
			get
			{
				return this.value;
			}
			private set
			{
				this.value = value;
			}
		}

		public override string ToString()
		{
			string[] array = new string[5];
			array[0] = "[";
			int num = 1;
			string text;
			if (this.Key != null)
			{
				TKey tkey = this.Key;
				text = tkey.ToString();
			}
			else
			{
				text = string.Empty;
			}
			array[num] = text;
			array[2] = ", ";
			int num2 = 3;
			string text2;
			if (this.Value != null)
			{
				TValue tvalue = this.Value;
				text2 = tvalue.ToString();
			}
			else
			{
				text2 = string.Empty;
			}
			array[num2] = text2;
			array[4] = "]";
			return string.Concat(array);
		}

		private TKey key;

		private TValue value;
	}
}
