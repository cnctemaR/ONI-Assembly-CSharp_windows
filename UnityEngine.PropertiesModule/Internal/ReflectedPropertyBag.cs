using System;
using UnityEngine;

namespace Unity.Properties.Internal
{
	[ReflectedPropertyBag]
	internal class ReflectedPropertyBag<TContainer> : ContainerPropertyBag<TContainer>
	{
		internal new void AddProperty<TValue>(Property<TContainer, TValue> property)
		{
			TContainer tcontainer = default(TContainer);
			IProperty<TContainer> property2;
			bool flag = base.TryGetProperty(ref tcontainer, property.Name, out property2);
			if (flag)
			{
				bool flag2 = property2.DeclaredValueType() == typeof(TValue);
				if (!flag2)
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						"Detected multiple return types for PropertyBag=[",
						TypeUtility.GetTypeDisplayName(typeof(TContainer)),
						"] Property=[",
						property.Name,
						"]. The property will use the most derived Type=[",
						TypeUtility.GetTypeDisplayName(property2.DeclaredValueType()),
						"] and IgnoreType=[",
						TypeUtility.GetTypeDisplayName(property.DeclaredValueType()),
						"]."
					}));
				}
			}
			else
			{
				base.AddProperty<TValue>(property);
			}
		}
	}
}
