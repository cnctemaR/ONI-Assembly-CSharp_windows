using System;

namespace Unity.Properties
{
	public interface IPropertyBagVisitor
	{
		void Visit<TContainer>(IPropertyBag<TContainer> properties, ref TContainer container);
	}
}
