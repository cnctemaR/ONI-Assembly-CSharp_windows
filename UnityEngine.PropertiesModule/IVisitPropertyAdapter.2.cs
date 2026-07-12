using System;

namespace Unity.Properties
{
	public interface IVisitPropertyAdapter<TValue> : IPropertyVisitorAdapter
	{
		void Visit<TContainer>(in VisitContext<TContainer, TValue> context, ref TContainer container, ref TValue value);
	}
}
