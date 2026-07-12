using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class UxmlObjectListAttributeDescription<T> : UxmlObjectAttributeDescription<List<T>> where T : new()
	{
		public override List<T> GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			VisualTreeAsset visualTreeAsset = cc.visualTreeAsset;
			List<T> list = ((visualTreeAsset != null) ? visualTreeAsset.GetUxmlObjects<T>(bag, cc) : null);
			bool flag = list != null;
			List<T> list3;
			if (flag)
			{
				List<T> list2 = null;
				foreach (T t in list)
				{
					bool flag2 = list2 == null;
					if (flag2)
					{
						list2 = new List<T>();
					}
					list2.Add(t);
				}
				list3 = list2;
			}
			else
			{
				list3 = base.defaultValue;
			}
			return list3;
		}
	}
}
