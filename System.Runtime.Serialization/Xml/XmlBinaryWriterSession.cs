using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace System.Xml
{
	public class XmlBinaryWriterSession
	{
		public XmlBinaryWriterSession()
		{
			this.nextKey = 0;
			this.maps = new XmlBinaryWriterSession.PriorityDictionary<IXmlDictionary, XmlBinaryWriterSession.IntArray>();
			this.strings = new XmlBinaryWriterSession.PriorityDictionary<string, int>();
		}

		public virtual bool TryAdd(XmlDictionaryString value, out int key)
		{
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
			}
			XmlBinaryWriterSession.IntArray intArray;
			if (!this.maps.TryGetValue(value.Dictionary, out intArray))
			{
				key = this.Add(value.Value);
				intArray = this.AddKeys(value.Dictionary, value.Key + 1);
				intArray[value.Key] = key + 1;
				return true;
			}
			key = intArray[value.Key] - 1;
			if (key != -1)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("The specified key already exists in the dictionary.")));
			}
			key = this.Add(value.Value);
			intArray[value.Key] = key + 1;
			return true;
		}

		private int Add(string s)
		{
			int num = this.nextKey;
			this.nextKey = num + 1;
			int num2 = num;
			this.strings.Add(s, num2);
			return num2;
		}

		private XmlBinaryWriterSession.IntArray AddKeys(IXmlDictionary dictionary, int minCount)
		{
			XmlBinaryWriterSession.IntArray intArray = new XmlBinaryWriterSession.IntArray(Math.Max(minCount, 16));
			this.maps.Add(dictionary, intArray);
			return intArray;
		}

		public void Reset()
		{
			this.nextKey = 0;
			this.maps.Clear();
			this.strings.Clear();
		}

		internal bool TryLookup(XmlDictionaryString s, out int key)
		{
			XmlBinaryWriterSession.IntArray intArray;
			if (this.maps.TryGetValue(s.Dictionary, out intArray))
			{
				key = intArray[s.Key] - 1;
				if (key != -1)
				{
					return true;
				}
			}
			if (this.strings.TryGetValue(s.Value, out key))
			{
				if (intArray == null)
				{
					intArray = this.AddKeys(s.Dictionary, s.Key + 1);
				}
				intArray[s.Key] = key + 1;
				return true;
			}
			key = -1;
			return false;
		}

		private XmlBinaryWriterSession.PriorityDictionary<string, int> strings;

		private XmlBinaryWriterSession.PriorityDictionary<IXmlDictionary, XmlBinaryWriterSession.IntArray> maps;

		private int nextKey;

		private class PriorityDictionary<K, V> where K : class
		{
			public PriorityDictionary()
			{
				this.list = new XmlBinaryWriterSession.PriorityDictionary<K, V>.Entry[16];
			}

			public void Clear()
			{
				this.now = 0;
				this.listCount = 0;
				Array.Clear(this.list, 0, this.list.Length);
				if (this.dictionary != null)
				{
					this.dictionary.Clear();
				}
			}

			public bool TryGetValue(K key, out V value)
			{
				for (int i = 0; i < this.listCount; i++)
				{
					if (this.list[i].Key == key)
					{
						value = this.list[i].Value;
						this.list[i].Time = this.Now;
						return true;
					}
				}
				for (int j = 0; j < this.listCount; j++)
				{
					if (this.list[j].Key.Equals(key))
					{
						value = this.list[j].Value;
						this.list[j].Time = this.Now;
						return true;
					}
				}
				if (this.dictionary == null)
				{
					value = default(V);
					return false;
				}
				if (!this.dictionary.TryGetValue(key, out value))
				{
					return false;
				}
				int num = 0;
				int num2 = this.list[0].Time;
				for (int k = 1; k < this.listCount; k++)
				{
					if (this.list[k].Time < num2)
					{
						num = k;
						num2 = this.list[k].Time;
					}
				}
				this.list[num].Key = key;
				this.list[num].Value = value;
				this.list[num].Time = this.Now;
				return true;
			}

			public void Add(K key, V value)
			{
				if (this.listCount < this.list.Length)
				{
					this.list[this.listCount].Key = key;
					this.list[this.listCount].Value = value;
					this.listCount++;
					return;
				}
				if (this.dictionary == null)
				{
					this.dictionary = new Dictionary<K, V>();
					for (int i = 0; i < this.listCount; i++)
					{
						this.dictionary.Add(this.list[i].Key, this.list[i].Value);
					}
				}
				this.dictionary.Add(key, value);
			}

			private int Now
			{
				get
				{
					int num = this.now + 1;
					this.now = num;
					if (num == 2147483647)
					{
						this.DecreaseAll();
					}
					return this.now;
				}
			}

			private void DecreaseAll()
			{
				for (int i = 0; i < this.listCount; i++)
				{
					XmlBinaryWriterSession.PriorityDictionary<K, V>.Entry[] array = this.list;
					int num = i;
					array[num].Time = array[num].Time / 2;
				}
				this.now /= 2;
			}

			private Dictionary<K, V> dictionary;

			private XmlBinaryWriterSession.PriorityDictionary<K, V>.Entry[] list;

			private int listCount;

			private int now;

			private struct Entry
			{
				public K Key;

				public V Value;

				public int Time;
			}
		}

		private class IntArray
		{
			public IntArray(int size)
			{
				this.array = new int[size];
			}

			public int this[int index]
			{
				get
				{
					if (index >= this.array.Length)
					{
						return 0;
					}
					return this.array[index];
				}
				set
				{
					if (index >= this.array.Length)
					{
						int[] array = new int[Math.Max(index + 1, this.array.Length * 2)];
						Array.Copy(this.array, array, this.array.Length);
						this.array = array;
					}
					this.array[index] = value;
				}
			}

			private int[] array;
		}
	}
}
