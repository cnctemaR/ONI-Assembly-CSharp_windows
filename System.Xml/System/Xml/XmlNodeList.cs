using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Xml
{
	public abstract class XmlNodeList : IEnumerable, IDisposable
	{
		public abstract XmlNode Item(int index);

		public abstract int Count { get; }

		public abstract IEnumerator GetEnumerator();

		[IndexerName("ItemOf")]
		public virtual XmlNode this[int i]
		{
			get
			{
				return this.Item(i);
			}
		}

		void IDisposable.Dispose()
		{
			this.PrivateDisposeNodeList();
		}

		protected virtual void PrivateDisposeNodeList()
		{
		}
	}
}
