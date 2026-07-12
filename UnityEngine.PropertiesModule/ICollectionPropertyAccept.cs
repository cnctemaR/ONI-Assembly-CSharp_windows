using System;

namespace Unity.Properties
{
	public interface ICollectionPropertyAccept<TCollection>
	{
		void Accept<TContainer>(ICollectionPropertyVisitor visitor, Property<TContainer, TCollection> property, ref TContainer container, ref TCollection collection);
	}
}
