using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public interface IListPropertyVisitor
	{
		void Visit<TContainer, TList, TElement>(Property<TContainer, TList> property, ref TContainer container, ref TList list) where TList : IList<TElement>;
	}
}
