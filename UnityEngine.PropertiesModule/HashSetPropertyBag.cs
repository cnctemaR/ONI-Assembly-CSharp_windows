using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public class HashSetPropertyBag<TElement> : SetPropertyBagBase<HashSet<TElement>, TElement>
	{
		protected override InstantiationKind InstantiationKind
		{
			get
			{
				return InstantiationKind.PropertyBagOverride;
			}
		}

		protected override HashSet<TElement> Instantiate()
		{
			return new HashSet<TElement>();
		}
	}
}
