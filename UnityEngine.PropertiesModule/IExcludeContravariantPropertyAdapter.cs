using System;

namespace Unity.Properties
{
	public interface IExcludeContravariantPropertyAdapter<TContainer, in TValue> : IPropertyVisitorAdapter
	{
		bool IsExcluded(in ExcludeContext<TContainer> context, ref TContainer container, TValue value);
	}
}
