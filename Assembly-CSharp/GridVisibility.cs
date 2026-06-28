using System;
using UnityEngine;

public class GridVisibility : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		CellChangeMonitor.Instance.Add(this, new Action<int, int>(this.OnCellChange), false);
	}

	private void OnCellChange(int previous_cell, int new_cell)
	{
		if (base.gameObject.HasTag(GameTags.Dead))
		{
			return;
		}
		if (!Grid.Revealed[new_cell])
		{
			int num;
			int num2;
			Grid.PosToXY(this.transform.position, out num, out num2);
			GridVisibility.Reveal(num, num2, this.radius, this.innerRadius);
			Grid.Revealed[new_cell] = true;
		}
	}

	private void Update()
	{
		FogOfWarMask.ClearMask(Grid.PosToCell(this));
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
					byte b = Grid.Visible[num5];
					if (b < 255)
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
		CellChangeMonitor.Instance.Remove(this, new Action<int, int>(this.OnCellChange), false);
	}

	public float radius = 18f;

	public float innerRadius = 16.5f;
}
