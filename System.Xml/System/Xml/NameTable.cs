using System;

namespace System.Xml
{
	public class NameTable : XmlNameTable
	{
		public override string Add(char[] key, int start, int len)
		{
			if ((0 > start && start >= key.Length) || (0 > len && len >= key.Length - len))
			{
				throw new IndexOutOfRangeException("The Index is out of range.");
			}
			if (len == 0)
			{
				return string.Empty;
			}
			int num = 0;
			int num2 = start + len;
			for (int i = start; i < num2; i++)
			{
				num = (num << 5) - num + (int)key[i];
			}
			num &= int.MaxValue;
			for (NameTable.Entry entry = this.buckets[num % this.count]; entry != null; entry = entry.next)
			{
				if (entry.hash == num && entry.len == len && NameTable.StrEqArray(entry.str, key, start))
				{
					return entry.str;
				}
			}
			return this.AddEntry(new string(key, start, len), num);
		}

		public override string Add(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int length = key.Length;
			if (length == 0)
			{
				return string.Empty;
			}
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				num = (num << 5) - num + (int)key[i];
			}
			num &= int.MaxValue;
			for (NameTable.Entry entry = this.buckets[num % this.count]; entry != null; entry = entry.next)
			{
				if (entry.hash == num && entry.len == key.Length && entry.str == key)
				{
					return entry.str;
				}
			}
			return this.AddEntry(key, num);
		}

		public override string Get(char[] key, int start, int len)
		{
			if ((0 > start && start >= key.Length) || (0 > len && len >= key.Length - len))
			{
				throw new IndexOutOfRangeException("The Index is out of range.");
			}
			if (len == 0)
			{
				return string.Empty;
			}
			int num = 0;
			int num2 = start + len;
			for (int i = start; i < num2; i++)
			{
				num = (num << 5) - num + (int)key[i];
			}
			num &= int.MaxValue;
			for (NameTable.Entry entry = this.buckets[num % this.count]; entry != null; entry = entry.next)
			{
				if (entry.hash == num && entry.len == len && NameTable.StrEqArray(entry.str, key, start))
				{
					return entry.str;
				}
			}
			return null;
		}

		public override string Get(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			int length = value.Length;
			if (length == 0)
			{
				return string.Empty;
			}
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				num = (num << 5) - num + (int)value[i];
			}
			num &= int.MaxValue;
			for (NameTable.Entry entry = this.buckets[num % this.count]; entry != null; entry = entry.next)
			{
				if (entry.hash == num && entry.len == value.Length && entry.str == value)
				{
					return entry.str;
				}
			}
			return null;
		}

		private string AddEntry(string str, int hash)
		{
			int num = hash % this.count;
			this.buckets[num] = new NameTable.Entry(str, hash, this.buckets[num]);
			if (this.size++ == this.count)
			{
				this.count <<= 1;
				int num2 = this.count - 1;
				NameTable.Entry[] array = new NameTable.Entry[this.count];
				for (int i = 0; i < this.buckets.Length; i++)
				{
					NameTable.Entry entry = this.buckets[i];
					NameTable.Entry next;
					for (NameTable.Entry entry2 = entry; entry2 != null; entry2 = next)
					{
						int num3 = entry2.hash & num2;
						next = entry2.next;
						entry2.next = array[num3];
						array[num3] = entry2;
					}
				}
				this.buckets = array;
			}
			return str;
		}

		private static bool StrEqArray(string str, char[] str2, int start)
		{
			int num = str.Length;
			num--;
			start += num;
			while (str[num] == str2[start])
			{
				num--;
				start--;
				if (num < 0)
				{
					return true;
				}
			}
			return false;
		}

		private const int INITIAL_BUCKETS = 128;

		private int count = 128;

		private NameTable.Entry[] buckets = new NameTable.Entry[128];

		private int size;

		private class Entry
		{
			public Entry(string str, int hash, NameTable.Entry next)
			{
				this.str = str;
				this.len = str.Length;
				this.hash = hash;
				this.next = next;
			}

			public string str;

			public int hash;

			public int len;

			public NameTable.Entry next;
		}
	}
}
