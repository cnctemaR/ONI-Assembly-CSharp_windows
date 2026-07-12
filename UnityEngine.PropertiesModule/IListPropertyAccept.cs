using System;

namespace Unity.Properties
{
	public interface IListPropertyAccept<TList>
	{
		void Accept<TContainer>(IListPropertyVisitor visitor, Property<TContainer, TList> property, ref TContainer container, ref TList list);
	}
}
