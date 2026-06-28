using System;

namespace Newtonsoft.Json.Utilities
{
	internal class PropertyNameTable
	{
		public PropertyNameTable()
		{
			this._entries = new PropertyNameTable.Entry[this._mask + 1];
		}

		public string Get(char[] key, int start, int length)
		{
			if (length == 0)
			{
				return string.Empty;
			}
			int num = length + PropertyNameTable.HashCodeRandomizer;
			num += (num << 7) ^ (int)key[start];
			int num2 = start + length;
			for (int i = start + 1; i < num2; i++)
			{
				num += (num << 7) ^ (int)key[i];
			}
			num -= num >> 17;
			num -= num >> 11;
			num -= num >> 5;
			for (PropertyNameTable.Entry entry = this._entries[num & this._mask]; entry != null; entry = entry.Next)
			{
				if (entry.HashCode == num && PropertyNameTable.TextEquals(entry.Value, key, start, length))
				{
					return entry.Value;
				}
			}
			return null;
		}

		public string Add(string key)
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
			int num = length + PropertyNameTable.HashCodeRandomizer;
			for (int i = 0; i < key.Length; i++)
			{
				num += (num << 7) ^ (int)key[i];
			}
			num -= num >> 17;
			num -= num >> 11;
			num -= num >> 5;
			for (PropertyNameTable.Entry entry = this._entries[num & this._mask]; entry != null; entry = entry.Next)
			{
				if (entry.HashCode == num && entry.Value.Equals(key))
				{
					return entry.Value;
				}
			}
			return this.AddEntry(key, num);
		}

		private string AddEntry(string str, int hashCode)
		{
			int num = hashCode & this._mask;
			PropertyNameTable.Entry entry = new PropertyNameTable.Entry(str, hashCode, this._entries[num]);
			this._entries[num] = entry;
			if (this._count++ == this._mask)
			{
				this.Grow();
			}
			return entry.Value;
		}

		private void Grow()
		{
			PropertyNameTable.Entry[] entries = this._entries;
			int num = this._mask * 2 + 1;
			PropertyNameTable.Entry[] array = new PropertyNameTable.Entry[num + 1];
			foreach (PropertyNameTable.Entry entry in entries)
			{
				while (entry != null)
				{
					int num2 = entry.HashCode & num;
					PropertyNameTable.Entry next = entry.Next;
					entry.Next = array[num2];
					array[num2] = entry;
					entry = next;
				}
			}
			this._entries = array;
			this._mask = num;
		}

		private static bool TextEquals(string str1, char[] str2, int str2Start, int str2Length)
		{
			if (str1.Length != str2Length)
			{
				return false;
			}
			for (int i = 0; i < str1.Length; i++)
			{
				if (str1[i] != str2[str2Start + i])
				{
					return false;
				}
			}
			return true;
		}

		private static readonly int HashCodeRandomizer = Environment.TickCount;

		private int _count;

		private PropertyNameTable.Entry[] _entries;

		private int _mask = 31;

		private class Entry
		{
			internal Entry(string value, int hashCode, PropertyNameTable.Entry next)
			{
				this.Value = value;
				this.HashCode = hashCode;
				this.Next = next;
			}

			internal readonly string Value;

			internal readonly int HashCode;

			internal PropertyNameTable.Entry Next;
		}
	}
}
