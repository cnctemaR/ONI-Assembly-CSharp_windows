using System;

namespace Unity.Properties
{
	public interface IVisitPropertyAdapter<TContainer, TValue> : IPropertyVisitorAdapter
	{
		void Visit(in VisitContext<TContainer, TValue> context, ref TContainer container, ref TValue value);
	}
}
