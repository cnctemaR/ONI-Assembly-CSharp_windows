using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/GridVisibility")]
public class GridVisibility : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		this.cellChangeHandlerID = Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, GridVisibility.OnCellChangeDispatcher, this, "GridVisibility.OnSpawn");
		this.OnCellChange();
		WorldContainer myWorld = base.gameObject.GetMyWorld();
		if (myWorld != null && !base.gameObject.HasTag(GameTags.Stored))
		{
			myWorld.SetDiscovered(false);
		}
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
		int num = (int)Grid.WorldIdx[baseY * Grid.WidthInCells + baseX];
		for (int i = -radius; i <= radius; i++)
		{
			for (int j = -radius; j <= radius; j++)
			{
				int num2 = baseY + i;
				int num3 = baseX + j;
				if (num2 >= 0 && Grid.HeightInCells - 1 >= num2 && num3 >= 0 && Grid.WidthInCells - 1 >= num3)
				{
					int num4 = num2 * Grid.WidthInCells + num3;
					if (Grid.Visible[num4] < 255 && num == (int)Grid.WorldIdx[num4])
					{
						Vector2 vector = new Vector2((float)j, (float)i);
						float num5 = Mathf.Lerp(1f, 0f, (vector.magnitude - innerRadius) / ((float)radius - innerRadius));
						Grid.Reveal(num4, (byte)(255f * num5), false);
					}
				}
			}
		}
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(ref this.cellChangeHandlerID);
	}

	public int radius = 18;

	public float innerRadius = 16.5f;

	private ulong cellChangeHandlerID;

	private static readonly Action<object> OnCellChangeDispatcher = delegate(object obj)
	{
		Unsafe.As<GridVisibility>(obj).OnCellChange();
	};
}
