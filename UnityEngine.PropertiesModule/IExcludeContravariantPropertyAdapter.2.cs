using System;

namespace Unity.Properties
{
	public interface IExcludeContravariantPropertyAdapter<in TValue> : IPropertyVisitorAdapter
	{
		bool IsExcluded<TContainer>(in ExcludeContext<TContainer> context, ref TContainer container, TValue value);
	}
}
