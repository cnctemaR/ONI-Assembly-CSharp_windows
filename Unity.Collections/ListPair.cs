using System;
using System.Collections;

namespace Unity.Collections
{
	internal struct ListPair<Key, Value> where Value : IList
	{
		public ListPair(Key k, Value v)
		{
			this.key = k;
			this.value = v;
		}

		public override string ToString()
		{
			string text = string.Format("{0} = [", this.key);
			for (int i = 0; i < this.value.Count; i++)
			{
				string text2 = text;
				object obj = this.value[i];
				text = text2 + ((obj != null) ? obj.ToString() : null);
				if (i < this.value.Count - 1)
				{
					text += ", ";
				}
			}
			return text + "]";
		}

		public Key key;

		public Value value;
	}
}
