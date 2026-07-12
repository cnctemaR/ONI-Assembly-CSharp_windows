using System;

namespace Unity.Properties
{
	public interface IVisitPropertyAdapter : IPropertyVisitorAdapter
	{
		void Visit<TContainer, TValue>(in VisitContext<TContainer, TValue> context, ref TContainer container, ref TValue value);
	}
}
