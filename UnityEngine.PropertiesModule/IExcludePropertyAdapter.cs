using System;

namespace Unity.Properties
{
	public interface IExcludePropertyAdapter<TContainer, TValue> : IPropertyVisitorAdapter
	{
		bool IsExcluded(in ExcludeContext<TContainer, TValue> context, ref TContainer container, ref TValue value);
	}
}
