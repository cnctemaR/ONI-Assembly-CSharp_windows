using System;
using System.Collections.Generic;
using UnityEngine;

public class EraseRegionTool : DragTool
{
	public static EraseRegionTool Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		EraseRegionTool.Instance = this;
		this.viewMode = SimViewMode.Regions;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
		base.SetMode(DragTool.Mode.Box);
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
	}

	protected override void OnDragComplete(Vector3 downPos, Vector3 upPos)
	{
		RegionManager regionManager = Game.Instance.RegionManager;
		HashSet<Region> hashSet = new HashSet<Region>();
		Vector3 vector = Vector3.Min(downPos, upPos);
		Vector3 vector2 = Vector3.Max(downPos, upPos);
		Vector2I vector2I;
		Grid.PosToXY(vector, out vector2I.x, out vector2I.y);
		Vector2I vector2I2;
		Grid.PosToXY(vector2, out vector2I2.x, out vector2I2.y);
		for (int i = vector2I.y; i <= vector2I2.y; i++)
		{
			for (int j = vector2I.x; j <= vector2I2.x; j++)
			{
				int num = Grid.XYToCell(j, i);
				Region intersectionRegion = regionManager.GetIntersectionRegion(num);
				if (intersectionRegion != null)
				{
					hashSet.Add(intersectionRegion);
					if (intersectionRegion.RemoveCell(num, false))
					{
						this.DestroyRegion(intersectionRegion);
					}
				}
			}
		}
		foreach (Region region in hashSet)
		{
			if (region != null)
			{
				this.CheckRegionAreaFragmented(region);
			}
		}
		RegionInterfaceScreen.Instance.SetDragOverlapRegions(null);
	}

	private void DestroyRegion(Region reg)
	{
		Game.Instance.RegionManager.RemoveRegion(reg, true);
	}

	public void CheckRegionAreaFragmented(Region region)
	{
		List<int> list = new List<int>();
		int num = 0;
		foreach (int num2 in region.Cells)
		{
			list.Add(num2);
			num++;
		}
		while (list.Count > 0)
		{
			int num3 = list[0];
			list.Remove(list[0]);
			List<int> list2 = new List<int>();
			List<int> list3 = new List<int>();
			List<int> list4 = new List<int>();
			List<int> list5 = new List<int>();
			List<int> list6 = new List<int>();
			list3.Add(num3);
			list5.Add(num3);
			while (list5.Count > 0)
			{
				foreach (int num4 in list5)
				{
					int num5 = Grid.CellAbove(num4);
					this.ProbeCellAndProcess(num5, list3, list2, list, list4, list6);
					int num6 = Grid.CellBelow(num4);
					this.ProbeCellAndProcess(num6, list3, list2, list, list4, list6);
					int num7 = Grid.CellLeft(num4);
					this.ProbeCellAndProcess(num7, list3, list2, list, list4, list6);
					int num8 = Grid.CellRight(num4);
					this.ProbeCellAndProcess(num8, list3, list2, list, list4, list6);
				}
				foreach (int num9 in list5)
				{
					list4.Add(num9);
				}
				list5.Clear();
				foreach (int num10 in list6)
				{
					list5.Add(num10);
				}
				list6.Clear();
				foreach (int num11 in list3)
				{
					list2.Add(num11);
				}
				list3.Clear();
			}
			if (list4.Count == num || list4.Count == 0)
			{
				return;
			}
			Tag[] array = null;
			TreeFilterable component = region.GetComponent<TreeFilterable>();
			if (component != null)
			{
				array = component.GetTags();
			}
			for (int i = list4.Count - 1; i >= 0; i--)
			{
				if (region.RemoveCell(list4[i], false))
				{
					this.DestroyRegion(region);
					break;
				}
			}
			RegionTool.Instance.CreateRegion(region.PrefabID().Name, list4.ToArray(), Grid.CellToPos(list4[0]), array);
		}
	}

	private bool ProbeCellAndProcess(int checkCell, List<int> newTouchList, List<int> allTouchedList, List<int> unknownCellList, List<int> workingRegion, List<int> newRegionAdditions)
	{
		if (!allTouchedList.Contains(checkCell))
		{
			if (!newTouchList.Contains(checkCell))
			{
				newTouchList.Add(checkCell);
			}
			if (unknownCellList.Contains(checkCell))
			{
				unknownCellList.Remove(checkCell);
				if (!workingRegion.Contains(checkCell) && !newRegionAdditions.Contains(checkCell))
				{
					newRegionAdditions.Add(checkCell);
				}
				return true;
			}
		}
		return false;
	}

	[SerializeField]
	private GameObject placer;
}
