using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public interface IListPropertyBagVisitor
	{
		void Visit<TList, TElement>(IListPropertyBag<TList, TElement> properties, ref TList container) where TList : IList<TElement>;
	}
}
