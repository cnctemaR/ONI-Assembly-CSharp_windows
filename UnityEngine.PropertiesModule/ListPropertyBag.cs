using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public class ListPropertyBag<TElement> : IndexedCollectionPropertyBag<List<TElement>, TElement>
	{
		protected override InstantiationKind InstantiationKind
		{
			get
			{
				return InstantiationKind.PropertyBagOverride;
			}
		}

		protected override List<TElement> InstantiateWithCount(int count)
		{
			return new List<TElement>(count);
		}

		protected override List<TElement> Instantiate()
		{
			return new List<TElement>();
		}
	}
}
