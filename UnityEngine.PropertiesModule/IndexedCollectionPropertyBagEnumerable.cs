using System;

namespace Unity.Properties
{
	internal readonly struct IndexedCollectionPropertyBagEnumerable<TContainer>
	{
		public IndexedCollectionPropertyBagEnumerable(IIndexedCollectionPropertyBagEnumerator<TContainer> impl, TContainer container)
		{
			this.m_Impl = impl;
			this.m_Container = container;
		}

		public IndexedCollectionPropertyBagEnumerator<TContainer> GetEnumerator()
		{
			return new IndexedCollectionPropertyBagEnumerator<TContainer>(this.m_Impl, this.m_Container);
		}

		private readonly IIndexedCollectionPropertyBagEnumerator<TContainer> m_Impl;

		private readonly TContainer m_Container;
	}
}
