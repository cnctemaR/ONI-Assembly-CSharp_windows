using System;

namespace Unity.Properties
{
	public interface IExcludePropertyAdapter<TValue> : IPropertyVisitorAdapter
	{
		bool IsExcluded<TContainer>(in ExcludeContext<TContainer, TValue> context, ref TContainer container, ref TValue value);
	}
}
