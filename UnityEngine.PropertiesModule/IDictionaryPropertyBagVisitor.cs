using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public interface IDictionaryPropertyBagVisitor
	{
		void Visit<TDictionary, TKey, TValue>(IDictionaryPropertyBag<TDictionary, TKey, TValue> properties, ref TDictionary container) where TDictionary : IDictionary<TKey, TValue>;
	}
}
