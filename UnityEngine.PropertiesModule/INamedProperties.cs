using System;

namespace Unity.Properties
{
	public interface INamedProperties<TContainer>
	{
		bool TryGetProperty(ref TContainer container, string name, out IProperty<TContainer> property);
	}
}
