using System;
using UnityEngine;

public class RegionSelectTool : SelectTool
{
	protected override void OnPrefabInit()
	{
		this.defaultLayerMask = 1 | LayerMask.GetMask(new string[] { "World", "Pickupable", "Place", "PlaceWithDepth", "BlockSelection", "Construction", "Selection" });
		this.layerMask = this.defaultLayerMask;
		this.selectMarker = Util.KInstantiateUI<SelectMarker>(EntityPrefabs.Instance.SelectMarker, GameScreenManager.Instance.worldSpaceCanvas, false);
		this.selectMarker.gameObject.SetActive(false);
		RegionSelectTool.Instance = this;
		base.IncludeRegions = true;
		this.viewMode = SimViewMode.Regions;
	}

	public new static RegionSelectTool Instance;
}
