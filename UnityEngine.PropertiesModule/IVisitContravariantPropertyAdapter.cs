using System;

namespace Unity.Properties
{
	public interface IVisitContravariantPropertyAdapter<TContainer, in TValue> : IPropertyVisitorAdapter
	{
		void Visit(in VisitContext<TContainer> context, ref TContainer container, TValue value);
	}
}
