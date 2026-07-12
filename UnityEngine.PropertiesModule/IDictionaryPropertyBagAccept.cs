using System;

namespace Unity.Properties
{
	public interface IDictionaryPropertyBagAccept<TContainer>
	{
		void Accept(IDictionaryPropertyBagVisitor visitor, ref TContainer container);
	}
}
