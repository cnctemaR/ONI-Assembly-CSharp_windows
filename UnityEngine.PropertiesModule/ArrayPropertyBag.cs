using System;

namespace Unity.Properties
{
	public sealed class ArrayPropertyBag<TElement> : IndexedCollectionPropertyBag<TElement[], TElement>
	{
		protected override InstantiationKind InstantiationKind
		{
			get
			{
				return InstantiationKind.PropertyBagOverride;
			}
		}

		protected override TElement[] InstantiateWithCount(int count)
		{
			return new TElement[count];
		}

		protected override TElement[] Instantiate()
		{
			return Array.Empty<TElement>();
		}
	}
}
