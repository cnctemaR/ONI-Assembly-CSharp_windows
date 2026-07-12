using System;

namespace Unity.Properties
{
	public interface IIndexedProperties<TContainer>
	{
		bool TryGetProperty(ref TContainer container, int index, out IProperty<TContainer> property);
	}
}
