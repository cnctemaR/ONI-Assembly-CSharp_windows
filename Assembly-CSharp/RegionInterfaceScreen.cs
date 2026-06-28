using System;
using System.Collections.Generic;
using UnityEngine;

public class RegionInterfaceScreen : KScreen
{
	public IList<Region> DragHighlightedRegions
	{
		get
		{
			return this.dragHighlightRegions;
		}
	}

	protected override void OnPrefabInit()
	{
		RegionInterfaceScreen.Instance = this;
		base.OnPrefabInit();
		this.dragHighlightRegions = new Region[0];
	}

	private void OnShow()
	{
	}

	public Color GetRegionColor(ushort regionID)
	{
		float num = 0.5f;
		float num2 = 0.75f;
		Region regionByID = Game.Instance.RegionManager.GetRegionByID(regionID);
		Color color = ((!(regionByID == null)) ? regionByID.OverlayColor : Color.black);
		if (regionByID == null)
		{
			return color;
		}
		KSelectable component = regionByID.gameObject.GetComponent<KSelectable>();
		if (component.IsSelected)
		{
			return this.AddHighlightToColor(color, num2);
		}
		if (regionByID == this.HoveredRegion)
		{
			return this.AddHighlightToColor(color, num);
		}
		if (this.dragHighlightRegions != null)
		{
			foreach (Region region in this.dragHighlightRegions)
			{
				if (regionByID == region)
				{
					return this.AddHighlightToColor(color, num);
				}
			}
			return color;
		}
		return color;
	}

	private Color AddHighlightToColor(Color baseColor, float amount)
	{
		Color color = baseColor;
		color = new Color(Mathf.Clamp(color.r + amount, 0f, 1f), Mathf.Clamp(color.g + amount, 0f, 1f), Mathf.Clamp(color.b + amount, 0f, 1f));
		return color;
	}

	private void UpdateHoverRegion()
	{
		Camera main = Camera.main;
		Vector3 vector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -main.transform.position.z);
		Vector3 vector2 = main.ScreenToWorldPoint(vector);
		Region intersectionRegion = Game.Instance.RegionManager.GetIntersectionRegion(vector2);
		if (intersectionRegion != null)
		{
			this.HoveredRegion = intersectionRegion;
		}
		else
		{
			this.HoveredRegion = null;
		}
	}

	public Region RegionUnderCursor()
	{
		Camera main = Camera.main;
		Vector3 vector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -main.transform.position.z);
		Vector3 vector2 = main.ScreenToWorldPoint(vector);
		return Game.Instance.RegionManager.GetIntersectionRegion(vector2);
	}

	private void Update()
	{
		if (SimDebugView.Instance.GetMode() == SimViewMode.Regions)
		{
			this.UpdateHoverRegion();
		}
	}

	public void SetDragOverlapRegions(IList<Region> regions)
	{
		this.dragHighlightRegions = regions;
	}

	public static RegionInterfaceScreen Instance;

	private Region HoveredRegion;

	private IList<Region> dragHighlightRegions;

	public Color[] NonSelectedRegionColors;

	public Color mergeRegionColor;
}
