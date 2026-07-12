using System;

namespace Unity.Properties
{
	public interface ISetPropertyAccept<TSet>
	{
		void Accept<TContainer>(ISetPropertyVisitor visitor, Property<TContainer, TSet> property, ref TContainer container, ref TSet set);
	}
}
