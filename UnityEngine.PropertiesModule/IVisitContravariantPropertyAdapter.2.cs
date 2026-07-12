using System;

namespace Unity.Properties
{
	public interface IVisitContravariantPropertyAdapter<in TValue> : IPropertyVisitorAdapter
	{
		void Visit<TContainer>(in VisitContext<TContainer> context, ref TContainer container, TValue value);
	}
}
