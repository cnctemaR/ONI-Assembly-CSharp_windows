using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/GridVisibility")]
public class GridVisibility : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange), "GridVisibility.OnSpawn");
		this.OnCellChange();
	}

	private void OnCellChange()
	{
		if (base.gameObject.HasTag(GameTags.Dead))
		{
			return;
		}
		int num = Grid.PosToCell(this);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		if (!Grid.Revealed[num])
		{
			int num2;
			int num3;
			Grid.PosToXY(base.transform.GetPosition(), out num2, out num3);
			GridVisibility.Reveal(num2, num3, this.radius, this.innerRadius);
			Grid.Revealed[num] = true;
		}
		FogOfWarMask.ClearMask(num);
	}

	public static void Reveal(int baseX, int baseY, float radius, float innerRadius)
	{
		for (float num = -radius; num <= radius; num += 1f)
		{
			for (float num2 = -radius; num2 <= radius; num2 += 1f)
			{
				float num3 = (float)baseY + num;
				float num4 = (float)baseX + num2;
				if (num3 >= 0f && (float)(Grid.HeightInCells - 1) >= num3 && num4 >= 0f && (float)(Grid.WidthInCells - 1) >= num4)
				{
					int num5 = (int)(num3 * (float)Grid.WidthInCells + num4);
					if (Grid.Visible[num5] < 255)
					{
						Vector2 vector = new Vector2(num2, num);
						float num6 = Mathf.Lerp(1f, 0f, (vector.magnitude - innerRadius) / (radius - innerRadius));
						Grid.Reveal(num5, (byte)(255f * num6));
					}
				}
			}
		}
		int num7 = Mathf.CeilToInt(radius);
		Game.Instance.UpdateGameActiveRegion(baseX - num7, baseY - num7, baseX + num7, baseY + num7);
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange));
	}

	public float radius = 18f;

	public float innerRadius = 16.5f;
}
