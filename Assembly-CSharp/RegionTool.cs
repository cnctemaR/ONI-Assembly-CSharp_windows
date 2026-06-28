using System;
using System.Collections.Generic;
using UnityEngine;

public class RegionTool : DragTool
{
	public static RegionTool Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		RegionTool.Instance = this;
		this.viewMode = SimViewMode.Regions;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
		base.SetMode(DragTool.Mode.Box);
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		if (base.Dragging)
		{
			List<Region> list = new List<Region>();
			int num;
			int num2;
			Grid.PosToXY(cursorPos, out num, out num2);
			int num3;
			int num4;
			Grid.PosToXY(this.downPos, out num3, out num4);
			int num5 = 0;
			for (int i = 0; i <= Mathf.Abs(num2 - num4); i++)
			{
				for (int j = 0; j <= Mathf.Abs(num - num3); j++)
				{
					num5++;
					int num6 = ((num <= num3) ? (-1) : 1);
					int num7 = ((num2 <= num4) ? (-1) : 1);
					int num8 = Grid.XYToCell(num3 + j * num6, num4 + i * num7);
					Region intersectionRegion = Game.Instance.RegionManager.GetIntersectionRegion(num8);
					if (intersectionRegion != null && !list.Contains(intersectionRegion))
					{
						list.Add(intersectionRegion);
					}
				}
			}
			RegionInterfaceScreen.Instance.SetDragOverlapRegions(list);
		}
		base.GetComponent<HoverTextConfiguration>().UpdateHoverElements(null);
		base.OnMouseMove(cursorPos);
	}

	private Region FindOverlappingRegionInCell(int cell)
	{
		RegionManager regionManager = Game.Instance.RegionManager;
		Region intersectionRegion = regionManager.GetIntersectionRegion(cell);
		if (intersectionRegion != null)
		{
			if (intersectionRegion.PrefabID().Name == regionManager.selectedRegionPrefab.PrefabID().Name)
			{
				return intersectionRegion;
			}
		}
		return null;
	}

	private List<Region> FindAdjacentRegions(Vector2I start, Vector2I end)
	{
		List<Region> list = new List<Region>();
		int num = end.y;
		for (int i = start.x; i <= end.x; i++)
		{
			int num2 = Grid.CellAbove(Grid.XYToCell(i, num));
			Region region = this.FindOverlappingRegionInCell(num2);
			if (region != null && !list.Contains(region))
			{
				list.Add(region);
			}
		}
		num = start.y;
		for (int j = start.x; j <= end.x; j++)
		{
			int num3 = Grid.CellBelow(Grid.XYToCell(j, num));
			Region region2 = this.FindOverlappingRegionInCell(num3);
			if (region2 != null && !list.Contains(region2))
			{
				list.Add(region2);
			}
		}
		int num4 = start.x;
		for (int k = start.y; k <= end.y; k++)
		{
			int num5 = Grid.CellLeft(Grid.XYToCell(num4, k));
			Region region3 = this.FindOverlappingRegionInCell(num5);
			if (region3 != null && !list.Contains(region3))
			{
				list.Add(region3);
			}
		}
		num4 = end.x;
		for (int l = start.y; l <= end.y; l++)
		{
			int num6 = Grid.CellRight(Grid.XYToCell(num4, l));
			Region region4 = this.FindOverlappingRegionInCell(num6);
			if (region4 != null && !list.Contains(region4))
			{
				list.Add(region4);
			}
		}
		return list;
	}

	private bool GetNewRegionInfo(Vector2I start, Vector2I end, ref List<int> newRegionCells, ref List<Region> overlappedRegions, ref Dictionary<Region, List<int>> overlappedRegionCells)
	{
		bool flag = false;
		for (int i = start.y; i <= end.y; i++)
		{
			for (int j = start.x; j <= end.x; j++)
			{
				int num = Grid.XYToCell(j, i);
				newRegionCells.Add(num);
				RegionManager regionManager = Game.Instance.RegionManager;
				Region intersectionRegion = regionManager.GetIntersectionRegion(num);
				if (intersectionRegion != null)
				{
					if (intersectionRegion.PrefabID().Name == regionManager.selectedRegionPrefab.PrefabID().Name)
					{
						if (!overlappedRegions.Contains(intersectionRegion))
						{
							overlappedRegions.Add(intersectionRegion);
						}
					}
					else
					{
						if (!overlappedRegionCells.ContainsKey(intersectionRegion))
						{
							overlappedRegionCells.Add(intersectionRegion, new List<int>());
						}
						overlappedRegionCells[intersectionRegion].Add(num);
					}
				}
				else
				{
					flag = true;
				}
			}
		}
		return flag;
	}

	private void DeleteOverlappingCells(ref Dictionary<Region, List<int>> overlappedRegionCells)
	{
		foreach (KeyValuePair<Region, List<int>> keyValuePair in overlappedRegionCells)
		{
			for (int i = 0; i < keyValuePair.Value.Count; i++)
			{
				if (keyValuePair.Key.RemoveCell(keyValuePair.Value[i], true))
				{
					global::UnityEngine.Object.DestroyImmediate(keyValuePair.Key.gameObject);
				}
			}
			if (keyValuePair.Key != null)
			{
				EraseRegionTool.Instance.CheckRegionAreaFragmented(keyValuePair.Key);
			}
		}
		overlappedRegionCells.Clear();
	}

	protected override void OnDragComplete(Vector3 downPos, Vector3 upPos)
	{
		this.overlappedRegions.Clear();
		this.newRegionCells.Clear();
		this.overlappedRegionCells.Clear();
		base.GetComponent<HoverTextConfiguration>().UpdateHoverElements(null);
		Vector3 vector = Vector3.Min(downPos, upPos);
		Vector3 vector2 = Vector3.Max(downPos, upPos);
		Vector2I vector2I;
		Grid.PosToXY(vector, out vector2I.x, out vector2I.y);
		Vector2I vector2I2;
		Grid.PosToXY(vector2, out vector2I2.x, out vector2I2.y);
		this.overlappedRegions.AddRange(this.FindAdjacentRegions(vector2I, vector2I2));
		bool newRegionInfo = this.GetNewRegionInfo(vector2I, vector2I2, ref this.newRegionCells, ref this.overlappedRegions, ref this.overlappedRegionCells);
		this.DeleteOverlappingCells(ref this.overlappedRegionCells);
		if (this.overlappedRegions.Count > 0)
		{
			if (this.overlappedRegions.Count != 1 || newRegionInfo)
			{
				this.mergedTags.Clear();
				this.overlappedRegionCells.Clear();
				foreach (Region region in this.overlappedRegions)
				{
					TreeFilterable component = region.gameObject.GetComponent<TreeFilterable>();
					if (component != null)
					{
						foreach (Tag tag in component.GetTags())
						{
							if (!this.mergedTags.Contains(tag))
							{
								this.mergedTags.Add(tag);
							}
						}
					}
					foreach (int num in region.Cells)
					{
						this.overlappingCells.Add(num);
					}
				}
				foreach (int num2 in this.newRegionCells)
				{
					if (!this.overlappingCells.Contains(num2))
					{
						this.overlappingCells.Add(num2);
					}
				}
				this.ReplaceRegions(this.overlappedRegions, this.overlappingCells, downPos, this.mergedTags);
			}
		}
		else
		{
			this.CreateRegion(this.newRegionCells, downPos, null);
		}
	}

	public void CreateRegion(Region region_prefab, IList<int> cells, Vector3 position, IList<Tag> storageTags = null)
	{
		GameObject gameObject = GameUtil.KInstantiate(region_prefab.gameObject, Grid.SceneLayer.Background, Folder.Regions, null, 0);
		gameObject.transform.position = position;
		Region component = gameObject.GetComponent<Region>();
		component.SetInfo(region_prefab.info);
		gameObject.SetActive(true);
		Game.Instance.RegionManager.AddRegion(component, true);
		for (int i = 0; i < cells.Count; i++)
		{
			component.AddCell(cells[i], false);
		}
		TreeFilterable component2 = gameObject.GetComponent<TreeFilterable>();
		if (storageTags != null && component2 != null)
		{
			List<Tag> list = new List<Tag>();
			foreach (Tag tag in storageTags)
			{
				list.Add(tag);
			}
			component2.UpdateFilters(list);
		}
	}

	public void CreateRegion(string prefabID, IList<int> cells, Vector3 position, IList<Tag> storageTags = null)
	{
		Region regionPrefabByID = Game.Instance.RegionManager.GetRegionPrefabByID(prefabID);
		if (regionPrefabByID != null)
		{
			this.CreateRegion(regionPrefabByID, cells, position, storageTags);
		}
		else
		{
			global::Debug.LogError("Tried selecting a Region prefab with an invalid id.", null);
		}
	}

	public void CreateRegion(IList<int> cells, Vector3 position, IList<Tag> storageTags = null)
	{
		Region selectedRegionPrefab = Game.Instance.RegionManager.selectedRegionPrefab;
		this.CreateRegion(selectedRegionPrefab, cells, position, storageTags);
	}

	public void ReplaceRegions(IList<Region> replaced_regions, IList<int> new_region_cells, Vector3 position, IList<Tag> storageTags = null)
	{
		Region selectedRegionPrefab = Game.Instance.RegionManager.selectedRegionPrefab;
		GameObject gameObject = GameUtil.KInstantiate(selectedRegionPrefab.gameObject, Grid.SceneLayer.Background, Folder.Regions, null, 0);
		gameObject.transform.position = position;
		Region component = gameObject.GetComponent<Region>();
		component.SetInfo(selectedRegionPrefab.info);
		foreach (Region region in replaced_regions)
		{
			List<BuildingComplete> ownedBuildings = region.GetOwnedBuildings();
			region.RemoveBuildings(ownedBuildings, false);
			component.AddBuildings(ownedBuildings, false);
		}
		Game.Instance.RegionManager.AddRegion(component, false);
		gameObject.SetActive(true);
		for (int i = 0; i < new_region_cells.Count; i++)
		{
			component.AddCell(new_region_cells[i], false);
		}
		TreeFilterable component2 = gameObject.GetComponent<TreeFilterable>();
		if (storageTags != null && component2 != null)
		{
			List<Tag> list = new List<Tag>();
			foreach (Tag tag in storageTags)
			{
				list.Add(tag);
			}
			component2.UpdateFilters(list);
		}
		for (int j = replaced_regions.Count - 1; j >= 0; j--)
		{
			Region region2 = replaced_regions[j];
			region2.ClearCells();
			Game.Instance.RegionManager.RemoveRegion(region2, false);
		}
	}

	[SerializeField]
	private GameObject placer;

	private List<Tag> mergedTags = new List<Tag>();

	private List<int> overlappingCells = new List<int>();

	private List<Region> overlappedRegions = new List<Region>();

	private List<int> newRegionCells = new List<int>();

	private Dictionary<Region, List<int>> overlappedRegionCells = new Dictionary<Region, List<int>>();
}
