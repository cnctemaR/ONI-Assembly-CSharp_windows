using System;

namespace Unity.Properties
{
	public interface IKeyedProperties<TContainer, TKey>
	{
		bool TryGetProperty(ref TContainer container, TKey key, out IProperty<TContainer> property);
	}
}
