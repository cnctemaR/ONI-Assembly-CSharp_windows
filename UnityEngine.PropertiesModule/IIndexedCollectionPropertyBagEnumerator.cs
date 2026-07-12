using System;

namespace Unity.Properties
{
	internal interface IIndexedCollectionPropertyBagEnumerator<TContainer>
	{
		int GetCount(ref TContainer container);

		IProperty<TContainer> GetSharedProperty();

		IndexedCollectionSharedPropertyState GetSharedPropertyState();

		void SetSharedPropertyState(IndexedCollectionSharedPropertyState state);
	}
}
