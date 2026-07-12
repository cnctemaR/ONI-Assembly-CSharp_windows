using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public interface IDictionaryPropertyVisitor
	{
		void Visit<TContainer, TDictionary, TKey, TValue>(Property<TContainer, TDictionary> property, ref TContainer container, ref TDictionary dictionary) where TDictionary : IDictionary<TKey, TValue>;
	}
}
