using System;

namespace Unity.Properties
{
	public abstract class ConcreteTypeVisitor : IPropertyBagVisitor
	{
		protected abstract void VisitContainer<TContainer>(ref TContainer container);

		void IPropertyBagVisitor.Visit<TContainer>(IPropertyBag<TContainer> properties, ref TContainer container)
		{
			this.VisitContainer<TContainer>(ref container);
		}
	}
}
