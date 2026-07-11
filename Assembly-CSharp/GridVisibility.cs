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

	public static void Reveal(int baseX, int baseY, int radius, float innerRadius)
	{
		for (int i = -radius; i <= radius; i++)
		{
			for (int j = -radius; j <= radius; j++)
			{
				int num = baseY + i;
				int num2 = baseX + j;
				if (num >= 0 && Grid.HeightInCells - 1 >= num && num2 >= 0 && Grid.WidthInCells - 1 >= num2)
				{
					int num3 = num * Grid.WidthInCells + num2;
					if (Grid.Visible[num3] < 255)
					{
						Vector2 vector = new Vector2((float)j, (float)i);
						float num4 = Mathf.Lerp(1f, 0f, (vector.magnitude - innerRadius) / ((float)radius - innerRadius));
						Grid.Reveal(num3, (byte)(255f * num4));
					}
				}
			}
		}
		int num5 = Mathf.CeilToInt((float)radius);
		Game.Instance.UpdateGameActiveRegion(baseX - num5, baseY - num5, baseX + num5, baseY + num5);
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange));
	}

	public int radius = 18;

	public float innerRadius = 16.5f;
}
