using System;
using System.Collections.Generic;

namespace System.Xml.Xsl.IlGen
{
	internal class UniqueList<T>
	{
		public int Add(T value)
		{
			int num;
			if (!this.lookup.ContainsKey(value))
			{
				num = this.list.Count;
				this.lookup.Add(value, num);
				this.list.Add(value);
			}
			else
			{
				num = this.lookup[value];
			}
			return num;
		}

		public T[] ToArray()
		{
			return this.list.ToArray();
		}

		private Dictionary<T, int> lookup = new Dictionary<T, int>();

		private List<T> list = new List<T>();
	}
}
