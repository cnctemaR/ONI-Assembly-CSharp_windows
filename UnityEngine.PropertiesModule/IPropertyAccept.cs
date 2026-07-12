using System;

namespace Unity.Properties
{
	public interface IPropertyAccept<TContainer>
	{
		void Accept(IPropertyVisitor visitor, ref TContainer container);
	}
}
