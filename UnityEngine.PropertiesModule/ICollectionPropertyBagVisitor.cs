using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public interface ICollectionPropertyBagVisitor
	{
		void Visit<TCollection, TElement>(ICollectionPropertyBag<TCollection, TElement> properties, ref TCollection container) where TCollection : ICollection<TElement>;
	}
}
