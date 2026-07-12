using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public interface ICollectionPropertyBag<TCollection, TElement> : IPropertyBag<TCollection>, IPropertyBag, ICollectionPropertyBagAccept<TCollection> where TCollection : ICollection<TElement>
	{
	}
}
