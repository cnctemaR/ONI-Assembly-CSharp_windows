using System;

namespace Unity.Properties
{
	public interface ICollectionPropertyBagAccept<TContainer>
	{
		void Accept(ICollectionPropertyBagVisitor visitor, ref TContainer container);
	}
}
