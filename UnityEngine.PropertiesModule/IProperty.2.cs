using System;

namespace Unity.Properties
{
	public interface IProperty<TContainer> : IProperty, IPropertyAccept<TContainer>
	{
		object GetValue(ref TContainer container);

		void SetValue(ref TContainer container, object value);
	}
}
