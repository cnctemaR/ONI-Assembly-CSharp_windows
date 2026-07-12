using System;

namespace Unity.Properties
{
	public interface IPropertyVisitor
	{
		void Visit<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container);
	}
}
