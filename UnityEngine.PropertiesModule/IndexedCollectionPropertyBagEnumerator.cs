using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.Properties
{
	internal struct IndexedCollectionPropertyBagEnumerator<TContainer> : IEnumerator<IProperty<TContainer>>, IEnumerator, IDisposable
	{
		internal IndexedCollectionPropertyBagEnumerator(IIndexedCollectionPropertyBagEnumerator<TContainer> impl, TContainer container)
		{
			this.m_Impl = impl;
			this.m_Container = container;
			this.m_Previous = impl.GetSharedPropertyState();
			this.m_Position = -1;
		}

		public IProperty<TContainer> Current
		{
			get
			{
				return this.m_Impl.GetSharedProperty();
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		public bool MoveNext()
		{
			this.m_Position++;
			bool flag = this.m_Position < this.m_Impl.GetCount(ref this.m_Container);
			bool flag2;
			if (flag)
			{
				this.m_Impl.SetSharedPropertyState(new IndexedCollectionSharedPropertyState
				{
					Index = this.m_Position,
					IsReadOnly = false
				});
				flag2 = true;
			}
			else
			{
				this.m_Impl.SetSharedPropertyState(this.m_Previous);
				flag2 = false;
			}
			return flag2;
		}

		public void Reset()
		{
			this.m_Position = -1;
			this.m_Impl.SetSharedPropertyState(this.m_Previous);
		}

		public void Dispose()
		{
		}

		private readonly IIndexedCollectionPropertyBagEnumerator<TContainer> m_Impl;

		private readonly IndexedCollectionSharedPropertyState m_Previous;

		private TContainer m_Container;

		private int m_Position;
	}
}
