using System;

namespace Unity.Properties
{
	public interface ISetPropertyBagAccept<TContainer>
	{
		void Accept(ISetPropertyBagVisitor visitor, ref TContainer container);
	}
}
