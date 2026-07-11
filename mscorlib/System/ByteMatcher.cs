using System;
using System.Collections;

namespace System
{
	internal class ByteMatcher
	{
		public void AddMapping(TermInfoStrings key, byte[] val)
		{
			if (val.Length == 0)
			{
				return;
			}
			this.map[val] = key;
			this.starts[(int)val[0]] = true;
		}

		public void Sort()
		{
		}

		public bool StartsWith(int c)
		{
			return this.starts[c] != null;
		}

		public TermInfoStrings Match(char[] buffer, int offset, int length, out int used)
		{
			foreach (object obj in this.map.Keys)
			{
				byte[] array = (byte[])obj;
				int num = 0;
				while (num < array.Length && num < length && (char)array[num] == buffer[offset + num])
				{
					if (array.Length - 1 == num)
					{
						used = array.Length;
						return (TermInfoStrings)this.map[array];
					}
					num++;
				}
			}
			used = 0;
			return (TermInfoStrings)(-1);
		}

		private Hashtable map = new Hashtable();

		private Hashtable starts = new Hashtable();
	}
}
