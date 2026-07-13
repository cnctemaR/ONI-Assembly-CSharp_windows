using System;

namespace Unity.Collections
{
	internal struct Pair<Key, Value>
	{
		public Pair(Key k, Value v)
		{
			this.key = k;
			this.value = v;
		}

		public override string ToString()
		{
			return string.Format("{0} = {1}", this.key, this.value);
		}

		public Key key;

		public Value value;
	}
}
