using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class UxmlObjectAttributeDescription<T> where T : new()
	{
		public T defaultValue { get; set; }

		public virtual T GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			VisualTreeAsset visualTreeAsset = cc.visualTreeAsset;
			List<T> list = ((visualTreeAsset != null) ? visualTreeAsset.GetUxmlObjects<T>(bag, cc) : null);
			bool flag = list != null;
			if (flag)
			{
				using (List<T>.Enumerator enumerator = list.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						return enumerator.Current;
					}
				}
			}
			return this.defaultValue;
		}
	}
}
