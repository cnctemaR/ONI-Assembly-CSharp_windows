using System;
using System.Collections;
using System.Reflection;

namespace YamlDotNet.Helpers
{
	internal sealed class GenericCollectionToNonGenericAdapter : IList, IEnumerable, ICollection
	{
		public GenericCollectionToNonGenericAdapter(object genericCollection, Type genericCollectionType, Type genericListType)
		{
			this.genericCollection = genericCollection;
			this.addMethod = genericCollectionType.GetPublicInstanceMethod("Add");
			this.countGetter = genericCollectionType.GetPublicProperty("Count").GetGetMethod();
			if (genericListType != null)
			{
				this.indexerSetter = genericListType.GetPublicProperty("Item").GetSetMethod();
			}
		}

		public int Add(object value)
		{
			int num = (int)this.countGetter.Invoke(this.genericCollection, null);
			this.addMethod.Invoke(this.genericCollection, new object[] { value });
			return num;
		}

		public void Clear()
		{
			throw new NotSupportedException();
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
				throw new NotSupportedException();
			}
		}

		public bool IsReadOnly
		{
			get
			{
				throw new NotSupportedException();
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
				throw new NotSupportedException();
			}
			set
			{
				this.indexerSetter.Invoke(this.genericCollection, new object[] { index, value });
			}
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		public int Count
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public bool IsSynchronized
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public object SyncRoot
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable)this.genericCollection).GetEnumerator();
		}

		private readonly object genericCollection;

		private readonly MethodInfo addMethod;

		private readonly MethodInfo indexerSetter;

		private readonly MethodInfo countGetter;
	}
}
