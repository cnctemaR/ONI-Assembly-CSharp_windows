using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Effects/Outline", 15)]
	public class Outline : Shadow
	{
		protected Outline()
		{
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (this.IsActive())
			{
				List<UIVertex> list = ListPool<UIVertex>.Get();
				vh.GetUIVertexStream(list);
				int num = list.Count * 5;
				if (list.Capacity < num)
				{
					list.Capacity = num;
				}
				int num2 = 0;
				int num3 = list.Count;
				base.ApplyShadowZeroAlloc(list, base.effectColor, num2, list.Count, base.effectDistance.x, base.effectDistance.y);
				num2 = num3;
				num3 = list.Count;
				base.ApplyShadowZeroAlloc(list, base.effectColor, num2, list.Count, base.effectDistance.x, -base.effectDistance.y);
				num2 = num3;
				num3 = list.Count;
				base.ApplyShadowZeroAlloc(list, base.effectColor, num2, list.Count, -base.effectDistance.x, base.effectDistance.y);
				num2 = num3;
				num3 = list.Count;
				base.ApplyShadowZeroAlloc(list, base.effectColor, num2, list.Count, -base.effectDistance.x, -base.effectDistance.y);
				vh.Clear();
				vh.AddUIVertexTriangleStream(list);
				ListPool<UIVertex>.Release(list);
			}
		}
	}
}
