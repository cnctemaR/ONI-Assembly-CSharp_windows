using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public class DictionaryPropertyBag<TKey, TValue> : KeyValueCollectionPropertyBag<Dictionary<TKey, TValue>, TKey, TValue>
	{
		protected override InstantiationKind InstantiationKind
		{
			get
			{
				return InstantiationKind.PropertyBagOverride;
			}
		}

		protected override Dictionary<TKey, TValue> Instantiate()
		{
			return new Dictionary<TKey, TValue>();
		}
	}
}
