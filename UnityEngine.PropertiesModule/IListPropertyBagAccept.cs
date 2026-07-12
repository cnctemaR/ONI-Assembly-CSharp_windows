using System;

namespace Unity.Properties
{
	public interface IListPropertyBagAccept<TContainer>
	{
		void Accept(IListPropertyBagVisitor visitor, ref TContainer container);
	}
}
