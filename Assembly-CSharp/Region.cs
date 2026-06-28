using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Klei.AI;
using KSerialization;
using Rendering;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[SerializationConfig(MemberSerialization.OptIn)]
public class Region : KMonoBehaviour, ISaveLoadableJson
{
	public event Action<BuildingComplete> OnBuildingAdded;

	public event Action<BuildingComplete> OnBuildingRemoved;

	public event Action<bool> OnValidStateChanged;

	public event global::System.Action OnRegionChanged;

	public ushort ID
	{
		get
		{
			return this.id;
		}
	}

	public IEnumerable<int> Cells
	{
		get
		{
			return this.cells;
		}
	}

	public string RegionName
	{
		get
		{
			return this.regionName;
		}
	}

	public Tag RegionTag
	{
		get
		{
			return this.regionTag;
		}
	}

	public bool NeedsEnclosure
	{
		get
		{
			return this.needsEnclosure;
		}
	}

	public int QueryLayer
	{
		get
		{
			return this.queryLayer;
		}
	}

	public string IconName
	{
		get
		{
			return this.iconName;
		}
	}

	public string ButtonStr
	{
		get
		{
			return this.buttonStr;
		}
	}

	public string HoverStr
	{
		get
		{
			return this.hoverToolStr;
		}
	}

	public global::Action Action
	{
		get
		{
			return this.action;
		}
	}

	public int MinWidth
	{
		get
		{
			return this.minSize.x;
		}
	}

	public int MinHeight
	{
		get
		{
			return this.minSize.y;
		}
	}

	public int UpperLeftCell { get; private set; }

	public int LowerLeftCell { get; private set; }

	public int UpperRightCell { get; private set; }

	public int LowerRightCell { get; private set; }

	public bool RegionValid
	{
		get
		{
			return this.regionValid;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(-1503271301, new EventSystem.EventHandler(this.OnSelectObject));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Attributes attributes = this.GetAttributes();
		if (this.info != null && this.info.attributes != null)
		{
			foreach (string text in this.info.attributes)
			{
				attributes.Add(Db.Get().BuildingAttributes.Get(text));
			}
		}
		base.Subscribe(Game.Instance.gameObject, 1798162660, new EventSystem.EventHandler(this.OnOverlayChanged));
		this.RegionChanged();
	}

	[OnDeserialized]
	internal void OnDeserializedMethod()
	{
		RegionManager regionManager = Game.Instance.RegionManager;
		regionManager.AddRegion(this, true);
		foreach (int num in this.cells)
		{
			regionManager.SetCellOwner(this.id, num);
			string name = this.PrefabID().Name;
			World.Instance.regionTileRenderer.AddBlock(base.gameObject.layer, regionManager.GetRegionDefByID(name), regionManager.GetRegionQueryLayerByID(name), num);
		}
		RegionManager.RegionInfo regionInfoByID = regionManager.GetRegionInfoByID(this.PrefabID().Name);
		if (regionInfoByID != null)
		{
			this.SetInfo(regionInfoByID);
		}
	}

	protected override void OnCleanUp()
	{
		Game.Instance.RegionManager.RemoveRegion(this, true);
		base.Unsubscribe(Game.Instance.gameObject, 1798162660, new EventSystem.EventHandler(this.OnOverlayChanged));
		this.Unsubscribe(-1503271301, new EventSystem.EventHandler(this.OnSelectObject));
		SaveLoadRoot component = base.GetComponent<SaveLoadRoot>();
		SaveLoader.Instance.saveManager.Unregister(component);
		if (this.solidChangedEntry != null)
		{
			this.solidChangedEntry.Release();
		}
		if (this.foundationTileChangedEntry != null)
		{
			this.foundationTileChangedEntry.Release();
		}
	}

	public void SetInfo(RegionManager.RegionInfo info)
	{
		this.info = info;
		this.regionName = info.name;
		this.regionTag = TagManager.Create(info.prefabID, null);
		this.useMinSize = info.useMinSize;
		this.minSize = info.minSize;
		this.ignoreSolids = info.ignoreSolids;
		this.needsEnclosure = info.needsEnclosure;
		this.OverlayColor = info.overlayColor;
		this.buttonStr = info.buttonString;
		this.hoverToolStr = info.hoverToolName;
		this.iconName = info.iconName;
		this.action = info.action;
		this.queryLayer = info.queryLayer;
		foreach (RegionManager.BuildingRequirement buildingRequirement in info.buildingRequirements)
		{
			this.buildingRequirements.Add(new Region.BuildingRequirement(buildingRequirement));
		}
	}

	private void RegionChanged()
	{
		this.FindPivotCells();
		this.selectable.SetStatusIndicatorOffset(this.GetStatusIndicatorOffset());
		this.CollectBuildingsInRegion();
		this.regionValid = this.IsRegionValid();
		if (this.solidChangedEntry != null)
		{
			this.solidChangedEntry.Release();
		}
		if (this.foundationTileChangedEntry != null)
		{
			this.foundationTileChangedEntry.Release();
		}
		Extents extents = default(Extents);
		Grid.CellToXY(this.LowerLeftCell, out extents.x, out extents.y);
		extents.x--;
		extents.y--;
		extents.width = this.GetWidth() + 1;
		extents.height = this.GetHeight() + 1;
		this.solidChangedEntry = GameScenePartitioner.Instance.Add("Region.RegionChanged", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedMask.mask, new Action<object>(this.OnSolidChanged));
		this.foundationTileChangedEntry = GameScenePartitioner.Instance.Add("Region.TileChanged", base.gameObject, extents, GameScenePartitioner.Instance.objectLayerMasks[8].mask, new Action<object>(this.TileChanged));
		if (this.OnRegionChanged != null)
		{
			this.OnRegionChanged();
		}
	}

	private void OnSolidChanged(object data)
	{
		this.RegionChanged();
	}

	private void TileChanged(object data)
	{
		this.RegionChanged();
	}

	public void SetID(ushort id)
	{
		this.id = id;
	}

	public void AddCells(IList<int> cells, bool fire_region_changed)
	{
		this.CreateBlockedCellPool();
		string name = this.PrefabID().Name;
		RegionManager regionManager = Game.Instance.RegionManager;
		BuildingDef regionDefByID = regionManager.GetRegionDefByID(name);
		int regionQueryLayerByID = regionManager.GetRegionQueryLayerByID(name);
		foreach (int num in cells)
		{
			cells.Add(num);
			regionManager.SetCellOwner(this.id, num);
			World.Instance.regionTileRenderer.AddBlock(base.gameObject.layer, regionDefByID, regionQueryLayerByID, num);
		}
		this.RegionChanged();
	}

	public void AddCell(int cell, bool fire_region_changed)
	{
		this.CreateBlockedCellPool();
		RegionManager regionManager = Game.Instance.RegionManager;
		this.cells.Add(cell);
		regionManager.SetCellOwner(this.id, cell);
		string name = this.PrefabID().Name;
		World.Instance.regionTileRenderer.AddBlock(base.gameObject.layer, regionManager.GetRegionDefByID(name), regionManager.GetRegionQueryLayerByID(name), cell);
		if (fire_region_changed)
		{
			this.RegionChanged();
		}
	}

	private void CreateBlockedCellPool()
	{
		if (this.blockedCellPool == null)
		{
			this.blockedCellPool = new UIPool<Image>(Assets.UIPrefabs.RegionCellBlocked);
		}
	}

	private Vector2I GetDistanceFromUpperLeftCell(int cell)
	{
		return Grid.GetOffset(this.UpperLeftCell, cell).ToVector2I();
	}

	public void RemoveCell(Vector3 pos, bool fire_region_changed)
	{
		int num = Grid.PosToCell(pos);
		this.RemoveCell(num, fire_region_changed);
	}

	public bool RemoveCell(int cell, bool fire_region_changed)
	{
		this.cells.Remove(cell);
		RegionManager regionManager = Game.Instance.RegionManager;
		regionManager.SetCellOwner(ushort.MaxValue, cell);
		World.Instance.regionTileRenderer.RemoveBlock(regionManager.GetRegionDefByID(this.PrefabID().Name), SimHashes.Vacuum, cell);
		if (this.blockedCellIndicatorMap.ContainsKey(cell))
		{
			this.blockedCellPool.ClearElement(this.blockedCellIndicatorMap[cell]);
			this.blockedCellIndicatorMap.Remove(cell);
		}
		if (this.cellSpriteMap.ContainsKey(cell))
		{
			global::UnityEngine.Object.Destroy(this.cellSpriteMap[cell].gameObject);
			this.cellSpriteMap.Remove(cell);
		}
		if (this.cells.Count > 0)
		{
			this.RegionChanged();
		}
		return this.cells.Count == 0;
	}

	public void AddBuildings(List<BuildingComplete> buildings, bool fire_region_changed = false)
	{
		if (buildings == null || buildings.Count == 0)
		{
			return;
		}
		foreach (BuildingComplete buildingComplete in buildings)
		{
			this.AddBuilding(buildingComplete, fire_region_changed);
		}
	}

	public void AddBuilding(BuildingComplete building, bool fire_region_changed = true)
	{
		if (building == null)
		{
			return;
		}
		KPrefabID kpid = building.GetComponent<KPrefabID>();
		Region.BuildingRequirement buildingRequirement = this.buildingRequirements.Find((Region.BuildingRequirement br) => kpid.HasTag(br.buildingTag));
		if (this.ContainsCells(building.PlacementCells) && !this.ownedBuildings.Contains(building))
		{
			this.ownedBuildings.Add(building);
			Attributes attributes = this.GetAttributes();
			foreach (AttributeModifier attributeModifier in building.regionModifiers)
			{
				attributes.Add(building.GetComponent<KSelectable>().GetName(), attributeModifier);
			}
			if (buildingRequirement != null && buildingRequirement.AddBuilding(building))
			{
				if (fire_region_changed && this.OnBuildingAdded != null)
				{
					this.OnBuildingAdded(building);
				}
				RequiresRegion component = building.GetComponent<RequiresRegion>();
				if (component != null)
				{
					component.SetRegion(this, true);
				}
			}
		}
		if (fire_region_changed)
		{
			this.RegionChanged();
		}
	}

	public void RemoveBuildings(List<BuildingComplete> buildings, bool fire_region_changed = true)
	{
		if (buildings == null || buildings.Count == 0)
		{
			return;
		}
		for (int i = buildings.Count - 1; i >= 0; i--)
		{
			BuildingComplete buildingComplete = buildings[i];
			this.RemoveBuilding(buildingComplete, fire_region_changed);
		}
	}

	public void RemoveBuilding(BuildingComplete building, bool fire_region_changed = true)
	{
		if (building == null)
		{
			Debug.LogError("Can't remove a null building.");
			return;
		}
		KPrefabID kpid = building.GetComponent<KPrefabID>();
		Region.BuildingRequirement buildingRequirement = this.buildingRequirements.Find((Region.BuildingRequirement br) => kpid.HasTag(br.buildingTag));
		this.ownedBuildings.Remove(building);
		if (buildingRequirement == null)
		{
			return;
		}
		Attributes attributes = this.GetAttributes();
		foreach (AttributeModifier attributeModifier in building.regionModifiers)
		{
			attributes.Remove(attributeModifier);
		}
		if (buildingRequirement.RemoveBuilding(building))
		{
			if (fire_region_changed && this.OnBuildingRemoved != null)
			{
				this.OnBuildingRemoved(building);
			}
			RequiresRegion component = building.GetComponent<RequiresRegion>();
			if (component != null)
			{
				component.SetRegion(null, fire_region_changed);
			}
		}
		if (fire_region_changed)
		{
			this.RegionChanged();
		}
	}

	public void AddDoor(Door door)
	{
		if (!this.needsEnclosure)
		{
			return;
		}
		if (!this.ownedDoors.Contains(door))
		{
			this.ownedDoors.Add(door);
			this.RegionChanged();
		}
	}

	public void RemoveDoor(Door door)
	{
		if (!this.needsEnclosure || !this.ownedDoors.Contains(door))
		{
			return;
		}
		this.ownedDoors.Remove(door);
		this.RegionChanged();
	}

	private Vector3 GetStatusIndicatorOffset()
	{
		Vector3 vector = Vector3.zero;
		switch (Region.statusItemPos)
		{
		case Region.StatusItemPos.UpperLeft:
			vector = Grid.CellToPos(this.UpperLeftCell);
			break;
		case Region.StatusItemPos.LowerLeft:
			vector = Grid.CellToPos(this.LowerLeftCell);
			break;
		case Region.StatusItemPos.UpperRight:
			vector = Grid.CellToPos(this.UpperRightCell);
			break;
		default:
			vector = Grid.CellToPos(this.UpperLeftCell);
			break;
		}
		vector.x += Grid.CellSizeInMeters / 2f;
		return (this.transform.position - vector) * -1f;
	}

	private void OnSelectObject(object data)
	{
		ToolMenu.Instance.TurnLargeCollectionOff();
	}

	private void OnOverlayChanged(object data)
	{
		SimViewMode simViewMode = (SimViewMode)((int)data);
		if (simViewMode == SimViewMode.Regions)
		{
			foreach (KeyValuePair<int, Image> keyValuePair in this.blockedCellIndicatorMap)
			{
				Color overlayColor = this.OverlayColor;
				overlayColor.a = this.blockedRegionAlpha;
				keyValuePair.Value.color = overlayColor;
			}
			foreach (KeyValuePair<int, SpriteRenderer> keyValuePair2 in this.cellSpriteMap)
			{
				keyValuePair2.Value.gameObject.SetActive(false);
			}
		}
		else
		{
			foreach (KeyValuePair<int, SpriteRenderer> keyValuePair3 in this.cellSpriteMap)
			{
				keyValuePair3.Value.gameObject.SetActive(simViewMode == SimViewMode.None);
			}
			if (DetailsScreen.Instance.CompareTargetWith(base.gameObject))
			{
				DetailsScreen.Instance.DeselectAndClose();
			}
			foreach (KeyValuePair<int, Image> keyValuePair4 in this.blockedCellIndicatorMap)
			{
				Color overlayColor2 = this.OverlayColor;
				overlayColor2.a = this.blockedRegionNoOverlayAlpha;
				keyValuePair4.Value.color = overlayColor2;
			}
		}
	}

	private void FindPivotCells()
	{
		this.UpperLeftCell = this.FindUpperLeftCell();
		this.LowerLeftCell = this.FindLowerLeftCell();
		this.UpperRightCell = this.FindUpperRightCell();
		this.LowerRightCell = this.FindLowerRightCell();
	}

	private int FindUpperLeftCell()
	{
		if (this.cells.Count == 0)
		{
			return Grid.InvalidCell;
		}
		CellOffset cellOffset = default(CellOffset);
		bool flag = true;
		foreach (int num in this.cells)
		{
			CellOffset offset = Grid.GetOffset(num);
			if (flag || offset.x < cellOffset.x || (offset.x == cellOffset.x && offset.y > cellOffset.y))
			{
				cellOffset = offset;
				flag = false;
			}
		}
		return Grid.XYToCell(cellOffset.x, cellOffset.y);
	}

	private int FindLowerLeftCell()
	{
		if (this.cells.Count == 0)
		{
			return Grid.InvalidCell;
		}
		CellOffset cellOffset = default(CellOffset);
		bool flag = true;
		foreach (int num in this.cells)
		{
			CellOffset offset = Grid.GetOffset(num);
			if (flag || offset.x < cellOffset.x || (offset.x == cellOffset.x && offset.y < cellOffset.y))
			{
				cellOffset = offset;
				flag = false;
			}
		}
		return Grid.XYToCell(cellOffset.x, cellOffset.y);
	}

	private int FindUpperRightCell()
	{
		if (this.cells.Count == 0)
		{
			return Grid.InvalidCell;
		}
		CellOffset cellOffset = default(CellOffset);
		bool flag = true;
		foreach (int num in this.cells)
		{
			CellOffset offset = Grid.GetOffset(num);
			if (flag || offset.x > cellOffset.x || (offset.x == cellOffset.x && offset.y > cellOffset.y))
			{
				cellOffset = offset;
				flag = false;
			}
		}
		return Grid.XYToCell(cellOffset.x, cellOffset.y);
	}

	private int FindLowerRightCell()
	{
		if (this.cells.Count == 0)
		{
			return Grid.InvalidCell;
		}
		CellOffset cellOffset = default(CellOffset);
		bool flag = true;
		foreach (int num in this.cells)
		{
			CellOffset offset = Grid.GetOffset(num);
			if (flag || offset.x > cellOffset.x || (offset.x == cellOffset.x && offset.y < cellOffset.y))
			{
				cellOffset = offset;
				flag = false;
			}
		}
		return Grid.XYToCell(cellOffset.x, cellOffset.y);
	}

	public bool IsCellBlocked(int cell)
	{
		return this.blockedCellIndicatorMap.ContainsKey(cell);
	}

	public TagSet GetRequiredBuildingsPrefabTags()
	{
		TagSet tagSet = new TagSet();
		foreach (Region.BuildingRequirement buildingRequirement in this.buildingRequirements)
		{
			tagSet.Add(buildingRequirement.buildingTag);
		}
		return tagSet;
	}

	public bool ContainsCells(int[] containedCells)
	{
		bool flag = true;
		for (int i = 0; i < containedCells.Length; i++)
		{
			if (!this.cells.Contains(containedCells[i]))
			{
				flag = false;
				break;
			}
		}
		return flag;
	}

	public List<T> GetComponentsInOwnedBuildings<T>(bool refresh = false) where T : Component
	{
		if (refresh)
		{
			this.CollectBuildingsInRegion();
		}
		List<T> list = new List<T>();
		for (int i = 0; i < this.ownedBuildings.Count; i++)
		{
			T component = this.ownedBuildings[i].GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
		}
		return list;
	}

	private void AddEdgeComponentToList<T>(int cell, ref List<T> list) where T : Component
	{
		GameObject gameObject = Grid.Objects[cell, 3];
		if (gameObject != null)
		{
			T component = gameObject.GetComponent<T>();
			if (component != null && !list.Contains(component))
			{
				list.Add(component);
			}
		}
	}

	public List<T> GetComponentsInLeftEdge<T>() where T : Component
	{
		List<T> list = new List<T>();
		int num = this.LowerLeftCell;
		while (this.cells.Contains(num))
		{
			this.AddEdgeComponentToList<T>(Grid.CellLeft(num), ref list);
			num = Grid.CellAbove(num);
		}
		return list;
	}

	public List<T> GetComponentsInBelowEdge<T>() where T : Component
	{
		List<T> list = new List<T>();
		int num = this.LowerLeftCell;
		while (this.cells.Contains(num))
		{
			this.AddEdgeComponentToList<T>(Grid.CellBelow(num), ref list);
			num = Grid.CellRight(num);
		}
		return list;
	}

	public List<T> GetComponentsInAboveEdge<T>() where T : Component
	{
		List<T> list = new List<T>();
		int num = this.UpperLeftCell;
		while (this.cells.Contains(num))
		{
			this.AddEdgeComponentToList<T>(Grid.CellAbove(num), ref list);
			num = Grid.CellRight(num);
		}
		return list;
	}

	public List<T> GetComponentsInRightEdge<T>() where T : Component
	{
		List<T> list = new List<T>();
		int num = this.LowerRightCell;
		while (this.cells.Contains(num))
		{
			this.AddEdgeComponentToList<T>(Grid.CellRight(num), ref list);
			num = Grid.CellAbove(num);
		}
		return list;
	}

	public List<T> GetComponentsInAllEdges<T>() where T : Component
	{
		List<T> list = new List<T>();
		list.AddRange(this.GetComponentsInLeftEdge<T>());
		list.AddRange(this.GetComponentsInBelowEdge<T>());
		list.AddRange(this.GetComponentsInAboveEdge<T>());
		list.AddRange(this.GetComponentsInRightEdge<T>());
		return list;
	}

	private int GetDimension(Action<List<int>, int> dimensionMethod)
	{
		List<int> list = new List<int>();
		foreach (int num in this.cells)
		{
			dimensionMethod(list, num);
		}
		return list.Count;
	}

	public int GetHeight()
	{
		return this.GetDimension(delegate(List<int> list, int cell)
		{
			Vector2I vector2I = Grid.CellToXY(cell);
			if (!list.Contains(vector2I.y))
			{
				list.Add(vector2I.y);
			}
		});
	}

	public int GetWidth()
	{
		return this.GetDimension(delegate(List<int> list, int cell)
		{
			Vector2I vector2I = Grid.CellToXY(cell);
			if (!list.Contains(vector2I.x))
			{
				list.Add(vector2I.x);
			}
		});
	}

	public List<BuildingComplete> GetOwnedBuildings()
	{
		return this.ownedBuildings;
	}

	public List<BuildingComplete> GetOwnedBuildingsInCategory(string category)
	{
		List<BuildingComplete> list = new List<BuildingComplete>();
		Region.BuildingRequirement buildingRequirement = this.buildingRequirements.Find((Region.BuildingRequirement br) => br.category == category);
		if (buildingRequirement != null)
		{
			list.AddRange(buildingRequirement.buildingsOwned);
		}
		return list;
	}

	public bool OwnsBuilding(BuildingComplete building, bool forceRefresh = false)
	{
		if (building == null || this.ownedBuildings == null)
		{
			return false;
		}
		if (forceRefresh)
		{
			this.CollectBuildingsInRegion();
		}
		return this.ownedBuildings.Contains(building);
	}

	public string GetMissingRequirementsString()
	{
		string text = string.Empty;
		this.ownsDoor = false;
		bool flag = true;
		if (this.needsEnclosure)
		{
			flag = this.IsRegionEnclosed();
		}
		else
		{
			this.ownsDoor = true;
		}
		if (!this.CheckSolid())
		{
			text += REGIONS.MISSINGREQUIREMENTS.BLOCKED;
		}
		if (!flag)
		{
			text += REGIONS.MISSINGREQUIREMENTS.ENCLOSED;
		}
		if (!this.ownsDoor)
		{
			text += REGIONS.MISSINGREQUIREMENTS.DOOR;
		}
		if (this.GetHeight() < this.minSize.y)
		{
			text += string.Format(REGIONS.MISSINGREQUIREMENTS.HEIGHT, this.minSize.y.ToString());
		}
		if (this.GetWidth() < this.minSize.x)
		{
			text += string.Format(REGIONS.MISSINGREQUIREMENTS.WIDTH, this.minSize.x.ToString());
		}
		if (!this.CheckBuildingRequirements())
		{
			text += this.GetMissingBuildingRequirementsString();
		}
		return text;
	}

	public string GetMissingBuildingRequirementsString()
	{
		this.CheckBuildingRequirements();
		string text = string.Empty;
		for (int i = 0; i < this.missingRequirements.Count; i++)
		{
			Region.BuildingRequirement buildingRequirement = this.missingRequirements[i];
			text += string.Format(REGIONS.MISSINGREQUIREMENTS.BUILDINGTAG, buildingRequirement.buildingTag.ProperName());
			if (i < this.missingRequirements.Count - 1)
			{
				text += "\n";
			}
		}
		return text;
	}

	public void ClearCells()
	{
		if (this.cells.Count > 0)
		{
			RegionTileRenderer regionTileRenderer = World.Instance.regionTileRenderer;
			BuildingDef regionDefByID = Game.Instance.RegionManager.GetRegionDefByID(this.PrefabID().Name);
			foreach (int num in this.cells)
			{
				regionTileRenderer.RemoveBlock(regionDefByID, SimHashes.Vacuum, num);
			}
			this.cells.Clear();
		}
	}

	private void CollectBuildingsInRegion()
	{
		List<BuildingComplete> list = new List<BuildingComplete>();
		foreach (int num in this.cells)
		{
			GameObject gameObject = Grid.Objects[num, 3];
			if (!(gameObject == null))
			{
				BuildingComplete component = gameObject.GetComponent<BuildingComplete>();
				if (!(component == null) && !list.Contains(component))
				{
					list.Add(component);
				}
			}
		}
		List<BuildingComplete> newBuildings = new List<BuildingComplete>(list);
		List<BuildingComplete> list2 = new List<BuildingComplete>(this.ownedBuildings);
		list2.RemoveAll((BuildingComplete removedBuilding) => newBuildings.Contains(removedBuilding));
		newBuildings.RemoveAll((BuildingComplete newBuilding) => this.ownedBuildings.Contains(newBuilding));
		this.AddBuildings(newBuildings, false);
		this.RemoveBuildings(list2, true);
	}

	private bool IsRegionValid()
	{
		this.ownsDoor = false;
		bool flag = true;
		if (this.needsEnclosure)
		{
			flag = this.IsRegionEnclosed();
		}
		else
		{
			this.ownsDoor = true;
			this.IsRegionEnclosed();
		}
		this.selectable.ToggleStatusItem(Db.Get().MiscStatusItems.RegionNeedsClosure, !flag, this);
		this.selectable.ToggleStatusItem(Db.Get().MiscStatusItems.RegionNeedsDoor, !this.ownsDoor, this);
		bool flag2 = true;
		if (this.buildingRequirements.Count > 0)
		{
			flag2 = this.CheckBuildingRequirements();
		}
		this.selectable.ToggleStatusItem(Db.Get().MiscStatusItems.RegionNeedsFurniture, !flag2, this);
		bool flag3 = true;
		if (this.useMinSize)
		{
			flag3 = this.GetHeight() >= this.minSize.y && this.GetWidth() >= this.minSize.x;
		}
		this.selectable.ToggleStatusItem(Db.Get().MiscStatusItems.RegionNeedsSize, !flag3, this);
		bool flag4;
		if (this.ignoreSolids)
		{
			flag4 = true;
		}
		else
		{
			flag4 = this.CheckSolid();
			this.selectable.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked, !flag4, this);
		}
		bool flag5 = flag && this.ownsDoor && flag2 && flag3 && flag4;
		if (this.wasValid != flag5)
		{
			if (this.OnValidStateChanged != null)
			{
				this.OnValidStateChanged(flag5);
			}
			this.wasValid = flag5;
		}
		return flag5;
	}

	private bool IsRegionEnclosed()
	{
		return this.CheckEnclosed();
	}

	public bool CheckBuildingRequirements()
	{
		this.missingRequirements = new List<Region.BuildingRequirement>(this.buildingRequirements);
		foreach (int num in this.cells)
		{
			GameObject gameObject = Grid.Objects[num, 3];
			if (!(gameObject == null))
			{
				KPrefabID prefabID = gameObject.GetComponent<KPrefabID>();
				if (prefabID == null)
				{
					Debug.LogError("Building Object is missing KPrefabID.");
					return false;
				}
				Region.BuildingRequirement buildingRequirement = this.buildingRequirements.Find((Region.BuildingRequirement req) => prefabID.HasTag(req.buildingTag));
				this.missingRequirements.Remove(buildingRequirement);
			}
		}
		return this.missingRequirements.Count == 0;
	}

	public string[] GetMissingBuildingsNames()
	{
		string[] array = new string[this.missingRequirements.Count];
		for (int i = 0; i < this.missingRequirements.Count; i++)
		{
			array[i] = string.Empty;
			GameObject prefab = Assets.GetPrefab(new Tag(this.missingRequirements[i].buildingTag));
			if (prefab == null)
			{
				Debug.LogError("There's no building with the given tag");
			}
			else
			{
				BuildingComplete component = prefab.GetComponent<BuildingComplete>();
				if (component == null)
				{
					Debug.LogError("The prefab retrieved does not contain a BuildingComplete component");
				}
				else
				{
					array[i] = component.Def.Name;
				}
			}
		}
		return array;
	}

	private bool CheckSolid()
	{
		if (this.blockedCellPool == null)
		{
			this.blockedCellPool = new UIPool<Image>(Assets.UIPrefabs.RegionCellBlocked);
		}
		bool flag = true;
		foreach (int num in this.cells)
		{
			if (Grid.Solid[num])
			{
				if (!this.blockedCellIndicatorMap.ContainsKey(num))
				{
					Image freeElement = this.blockedCellPool.GetFreeElement(GameScreenManager.Instance.worldSpaceCanvas, true);
					freeElement.transform.position = Grid.CellToPos(num) + Vector3.one * (Grid.CellSizeInMeters / 2f);
					this.blockedCellIndicatorMap.Add(num, freeElement);
				}
				Color overlayColor = this.OverlayColor;
				overlayColor.a = ((SimDebugView.Instance.GetMode() != SimViewMode.Regions) ? this.blockedRegionNoOverlayAlpha : this.blockedRegionAlpha);
				this.blockedCellIndicatorMap[num].color = overlayColor;
				flag = false;
			}
			else if (this.blockedCellIndicatorMap.ContainsKey(num))
			{
				this.blockedCellPool.ClearElement(this.blockedCellIndicatorMap[num]);
				this.blockedCellIndicatorMap.Remove(num);
			}
		}
		return flag;
	}

	private bool CheckEnclosed()
	{
		int[] array = new int[4];
		foreach (int num in this.cells)
		{
			array[0] = Grid.CellAbove(num);
			array[1] = Grid.CellRight(num);
			array[2] = Grid.CellBelow(num);
			array[3] = Grid.CellLeft(num);
			foreach (int num2 in array)
			{
				if (!Grid.Solid[num2])
				{
					if (!this.cells.Contains(num2))
					{
						if (!Grid.HasDoor[num2])
						{
							return false;
						}
						this.ownsDoor = true;
					}
				}
			}
		}
		return true;
	}

	private ushort id;

	[Serialize]
	private HashSet<int> cells = new HashSet<int>();

	[MyCmpGet]
	private KSelectable selectable;

	private string regionName;

	private Tag regionTag;

	private bool useMinSize = true;

	private Vector2I minSize;

	[HideInInspector]
	public bool ignoreSolids;

	[HideInInspector]
	public bool needsEnclosure;

	private bool[] visitedCells;

	private bool ownsDoor;

	private bool wasValid;

	private List<Door> ownedDoors = new List<Door>();

	private List<BuildingComplete> ownedBuildings = new List<BuildingComplete>();

	private List<Region.BuildingRequirement> missingRequirements = new List<Region.BuildingRequirement>();

	private Dictionary<int, Image> blockedCellIndicatorMap = new Dictionary<int, Image>();

	private Dictionary<int, SpriteRenderer> cellSpriteMap = new Dictionary<int, SpriteRenderer>();

	private string iconName;

	private string buttonStr;

	private string hoverToolStr;

	private string bgImg;

	private global::Action action;

	private static Region.StatusItemPos statusItemPos;

	[HideInInspector]
	public Color OverlayColor;

	[HideInInspector]
	private List<Region.BuildingRequirement> buildingRequirements = new List<Region.BuildingRequirement>();

	private UIPool<Image> blockedCellPool;

	private float blockedRegionAlpha = 0.35f;

	private float blockedRegionNoOverlayAlpha = 0.15f;

	public RegionManager.RegionInfo info;

	private int queryLayer;

	private bool regionValid;

	private GameScenePartitionerEntry solidChangedEntry;

	private GameScenePartitionerEntry foundationTileChangedEntry;

	[Serializable]
	public class BuildingRequirement
	{
		public BuildingRequirement(RegionManager.BuildingRequirement requirement)
		{
			if (requirement == null)
			{
				return;
			}
			this.category = requirement.category;
			this.buildingTag = requirement.buildingTag;
		}

		public bool AddBuilding(BuildingComplete newBuilding)
		{
			if (newBuilding == null)
			{
				return false;
			}
			if (!this.buildingsOwned.Contains(newBuilding))
			{
				this.buildingsOwned.Add(newBuilding);
				return true;
			}
			return false;
		}

		public bool RemoveBuilding(BuildingComplete building)
		{
			if (building == null)
			{
				return false;
			}
			if (this.buildingsOwned.Contains(building))
			{
				this.buildingsOwned.Remove(building);
				return true;
			}
			return false;
		}

		public string category;

		public Tag buildingTag;

		[HideInInspector]
		public List<BuildingComplete> buildingsOwned = new List<BuildingComplete>();
	}

	public enum StatusItemPos
	{
		UpperLeft,
		LowerLeft,
		UpperRight,
		LowerRight
	}
}
