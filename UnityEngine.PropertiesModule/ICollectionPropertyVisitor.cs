using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public interface ICollectionPropertyVisitor
	{
		void Visit<TContainer, TCollection, TElement>(Property<TContainer, TCollection> property, ref TContainer container, ref TCollection collection) where TCollection : ICollection<TElement>;
	}
}
