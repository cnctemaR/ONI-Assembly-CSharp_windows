using System;

namespace Unity.Properties
{
	public interface IExcludePropertyAdapter : IPropertyVisitorAdapter
	{
		bool IsExcluded<TContainer, TValue>(in ExcludeContext<TContainer, TValue> context, ref TContainer container, ref TValue value);
	}
}
