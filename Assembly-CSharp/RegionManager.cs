using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class RegionManager
{
	public RegionManager(int numCells, RegionManager.RegionInfo[] regionInfo)
	{
		this.regionInfoList = new List<RegionManager.RegionInfo>(regionInfo);
		this.free_id = 0;
		this.idGrid = new ushort[numCells];
		for (int i = 0; i < numCells; i++)
		{
			this.idGrid[i] = ushort.MaxValue;
		}
		Assets.RegionPrefabs.ForEach(delegate(GameObject regionPref)
		{
			this.regionPrefabs.Add(regionPref.GetComponent<Region>());
		});
		this.regionInfoList.ForEach(delegate(RegionManager.RegionInfo info)
		{
			BuildingDef buildingDef = ScriptableObject.CreateInstance<BuildingDef>();
			buildingDef.BlockTileAtlas = Assets.GetTextureAtlas("walls_insulation");
			buildingDef.BlockTilePlaceAtlas = Assets.GetTextureAtlas("walls_insulation");
			buildingDef.BlockTileMaterial = Assets.GetMaterial(info.bgImg);
			buildingDef.ObjectLayer = ObjectLayer.Backwall;
			buildingDef.TileLayer = ObjectLayer.Backwall;
			buildingDef.SceneLayer = Grid.SceneLayer.Background;
			this.regionDefMap.Add(info.prefabID, buildingDef);
		});
	}

	public ushort FreeID
	{
		get
		{
			return this.free_id;
		}
	}

	public void SelectRegionPrefab(string regionName)
	{
		this.selectedRegionPrefab = this.regionPrefabs.Find((Region prefab) => prefab.RegionName == regionName);
	}

	public Region GetRegionPrefabByID(string id)
	{
		return this.regionPrefabs.Find((Region prefab) => prefab.PrefabID().Name == id);
	}

	public RegionManager.RegionInfo GetRegionInfoByID(string id)
	{
		return this.regionInfoList.Find((RegionManager.RegionInfo info) => info.prefabID == id);
	}

	public BuildingDef GetRegionDefByID(string id)
	{
		return this.regionDefMap[id];
	}

	public int GetRegionQueryLayerByID(string id)
	{
		return this.regionInfoList.IndexOf(this.GetRegionInfoByID(id));
	}

	public void AddRegion(Region region, bool fire_region_changed = true)
	{
		region.SetID(this.free_id);
		this.regions[this.free_id] = region;
		this.free_id += 1;
		if (fire_region_changed && this.OnRegionChanged != null)
		{
			this.OnRegionChanged();
		}
	}

	public Region GetRegionByID(ushort ID)
	{
		if (this.regions.ContainsKey(ID))
		{
			return this.regions[ID];
		}
		return null;
	}

	public void RemoveRegion(Region region, bool fire_region_changed = true)
	{
		this.regions.Remove(region.ID);
		Util.KDestroyGameObject(region.gameObject);
		if (fire_region_changed && this.OnRegionChanged != null)
		{
			this.OnRegionChanged();
		}
	}

	public ushort GetIntersectionRegionID(int cell)
	{
		DebugUtil.Assert(cell >= 0 && cell < this.idGrid.Length, "Assert!");
		return this.idGrid[cell];
	}

	public Region GetIntersectionRegion(Vector2 pt)
	{
		int num = Grid.PosToCell(pt);
		if (Grid.IsValidCell(num))
		{
			return this.GetIntersectionRegion(num);
		}
		return null;
	}

	public Region GetIntersectionRegion(int cell)
	{
		DebugUtil.Assert(cell >= 0 && cell < this.idGrid.Length, "Assert!");
		ushort num = this.idGrid[cell];
		if (num == 65535)
		{
			return null;
		}
		return this.regions[num];
	}

	public void SetCellOwner(ushort region_id, int cell)
	{
		DebugUtil.Assert(cell >= 0 && cell < this.idGrid.Length, "Assert!");
		this.idGrid[cell] = region_id;
	}

	public List<Region> regionPrefabs = new List<Region>();

	private Dictionary<ushort, Region> regions = new Dictionary<ushort, Region>();

	public global::System.Action OnRegionChanged;

	private ushort free_id;

	private ushort[] idGrid;

	public const ushort INVALID_ID = 65535;

	public Region selectedRegionPrefab;

	private List<RegionManager.RegionInfo> regionInfoList = new List<RegionManager.RegionInfo>();

	private Dictionary<string, BuildingDef> regionDefMap = new Dictionary<string, BuildingDef>();

	public class BuildingRequirement
	{
		public BuildingRequirement(string category, string building_tag)
		{
			this.category = category;
			this.buildingTag = TagManager.Create(building_tag, null);
		}

		public string category;

		public Tag buildingTag;
	}

	public class RegionInfo
	{
		public RegionInfo(string nameKey, string prefabID, bool useMinSize, Vector2I minSize, bool ignoreSolids, bool needsEnclosure, RegionManager.BuildingRequirement[] buildingRequirements, Color overlayColor, string buttonString, string hoverToolName, string iconName, bool isAssignable, REGIONS.RequiredComponent[] additionalCmps, global::Action action, string[] attributes, string bgImg)
		{
			this.name = nameKey;
			this.prefabID = prefabID;
			this.useMinSize = useMinSize;
			this.minSize = minSize;
			this.ignoreSolids = ignoreSolids;
			this.needsEnclosure = needsEnclosure;
			this.buildingRequirements = buildingRequirements;
			this.overlayColor = overlayColor;
			this.buttonString = buttonString;
			this.hoverToolName = hoverToolName;
			this.iconName = iconName;
			this.isAssignable = isAssignable;
			this.additionalCmps = additionalCmps;
			this.action = action;
			this.attributes = attributes;
			this.bgImg = bgImg;
		}

		public string name;

		public string prefabID;

		public bool useMinSize;

		public Vector2I minSize;

		public bool ignoreSolids;

		public bool needsEnclosure;

		public RegionManager.BuildingRequirement[] buildingRequirements;

		public Color overlayColor;

		public string buttonString;

		public string hoverToolName;

		public string iconName;

		public string[] attributes;

		public global::Action action;

		public string bgImg;

		public bool isAssignable;

		public REGIONS.RequiredComponent[] additionalCmps;

		public int queryLayer;
	}
}
