using System;
using System.Collections;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class ArrayNodeDeserializer : INodeDeserializer
	{
		bool INodeDeserializer.Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			if (!expectedType.IsArray)
			{
				value = false;
				return false;
			}
			Type elementType = expectedType.GetElementType();
			ArrayNodeDeserializer.ArrayList arrayList = new ArrayNodeDeserializer.ArrayList();
			CollectionNodeDeserializer.DeserializeHelper(elementType, parser, nestedObjectDeserializer, arrayList, true);
			Array array = Array.CreateInstance(elementType, arrayList.Count);
			arrayList.CopyTo(array, 0);
			value = array;
			return true;
		}

		private sealed class ArrayList : IList, IEnumerable, ICollection
		{
			public ArrayList()
			{
				this.Clear();
			}

			public int Add(object value)
			{
				if (this.count == this.data.Length)
				{
					Array.Resize<object>(ref this.data, this.data.Length * 2);
				}
				this.data[this.count] = value;
				int num = this.count;
				this.count = num + 1;
				return num;
			}

			public void Clear()
			{
				this.data = new object[10];
				this.count = 0;
			}

			public bool Contains(object value)
			{
				throw new NotSupportedException();
			}

			public int IndexOf(object value)
			{
				throw new NotSupportedException();
			}

			public void Insert(int index, object value)
			{
				throw new NotSupportedException();
			}

			public bool IsFixedSize
			{
				get
				{
					return false;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public void Remove(object value)
			{
				throw new NotSupportedException();
			}

			public void RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			public object this[int index]
			{
				get
				{
					return this.data[index];
				}
				set
				{
					this.data[index] = value;
				}
			}

			public void CopyTo(Array array, int index)
			{
				Array.Copy(this.data, 0, array, index, this.count);
			}

			public int Count
			{
				get
				{
					return this.count;
				}
			}

			public bool IsSynchronized
			{
				get
				{
					return false;
				}
			}

			public object SyncRoot
			{
				get
				{
					return this.data;
				}
			}

			public IEnumerator GetEnumerator()
			{
				int num;
				for (int i = 0; i < this.count; i = num)
				{
					yield return this.data[i];
					num = i + 1;
				}
				yield break;
			}

			private object[] data;

			private int count;
		}
	}
}
