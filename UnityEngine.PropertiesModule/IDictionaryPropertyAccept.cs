using System;

namespace Unity.Properties
{
	public interface IDictionaryPropertyAccept<TDictionary>
	{
		void Accept<TContainer>(IDictionaryPropertyVisitor visitor, Property<TContainer, TDictionary> property, ref TContainer container, ref TDictionary dictionary);
	}
}
