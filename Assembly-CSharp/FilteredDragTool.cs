using System;
using System.Collections.Generic;
using UnityEngine;

public class FilteredDragTool : DragTool
{
	public bool IsActiveLayer(string layer)
	{
		return this.currentFilterTargets[FilteredDragTool.FILTERLAYERS.ALL] == ToolParameterMenu.ToggleState.On || (this.currentFilterTargets.ContainsKey(layer.ToUpper()) && this.currentFilterTargets[layer.ToUpper()] == ToolParameterMenu.ToggleState.On);
	}

	protected virtual void GetDefaultFilters(Dictionary<string, ToolParameterMenu.ToggleState> filters)
	{
		filters.Add(FilteredDragTool.FILTERLAYERS.ALL, ToolParameterMenu.ToggleState.On);
		filters.Add(FilteredDragTool.FILTERLAYERS.WIRES, ToolParameterMenu.ToggleState.Off);
		filters.Add(FilteredDragTool.FILTERLAYERS.LIQUIDCONDUIT, ToolParameterMenu.ToggleState.Off);
		filters.Add(FilteredDragTool.FILTERLAYERS.GASCONDUIT, ToolParameterMenu.ToggleState.Off);
		filters.Add(FilteredDragTool.FILTERLAYERS.BUILDINGS, ToolParameterMenu.ToggleState.Off);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.ResetFilter(this.filterTargets);
		OverlayScreen.OnOverlayChanged = (Action<SimViewMode>)Delegate.Combine(OverlayScreen.OnOverlayChanged, new Action<SimViewMode>(this.OnOverlayChanged));
	}

	public void ResetFilter()
	{
		this.ResetFilter(this.filterTargets);
	}

	protected void ResetFilter(Dictionary<string, ToolParameterMenu.ToggleState> filters)
	{
		filters.Clear();
		this.GetDefaultFilters(filters);
		this.currentFilterTargets = filters;
	}

	protected override void OnActivateTool()
	{
		this.active = true;
		base.OnActivateTool();
		this.OnOverlayChanged(OverlayScreen.Instance.mode);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		this.active = false;
		ToolMenu.Instance.toolParameterMenu.ClearMenu();
		base.OnDeactivateTool(new_tool);
	}

	protected string GetFilterLayerFromGameObject(GameObject input)
	{
		BuildingComplete component = input.GetComponent<BuildingComplete>();
		BuildingUnderConstruction component2 = input.GetComponent<BuildingUnderConstruction>();
		if (component)
		{
			return this.GetFilterLayerFromObjectLayer(component.Def.ObjectLayer);
		}
		if (component2)
		{
			return this.GetFilterLayerFromObjectLayer(component2.Def.ObjectLayer);
		}
		if (input.GetComponent<Clearable>() != null || input.GetComponent<Moppable>() != null)
		{
			return "CleanAndClear";
		}
		if (input.GetComponent<Diggable>() != null)
		{
			return "DigPlacer";
		}
		return "Default";
	}

	protected string GetFilterLayerFromObjectLayer(ObjectLayer input)
	{
		switch (input)
		{
		case ObjectLayer.FoundationTile:
			return "Tiles";
		default:
			if (input != ObjectLayer.Building)
			{
				return "Default";
			}
			return "Buildings";
		case ObjectLayer.GasConduit:
		case ObjectLayer.GasConduitConnection:
			return "GasPipes";
		case ObjectLayer.LiquidConduit:
		case ObjectLayer.LiquidConduitConnection:
			return "LiquidPipes";
		case ObjectLayer.Wire:
			return "Wires";
		}
	}

	private void OnOverlayChanged(SimViewMode overlay)
	{
		if (!this.active)
		{
			return;
		}
		string text = null;
		if (overlay != SimViewMode.LiquidVentMap)
		{
			if (overlay != SimViewMode.PowerMap)
			{
				if (overlay == SimViewMode.GasVentMap)
				{
					text = FilteredDragTool.FILTERLAYERS.GASCONDUIT;
				}
			}
			else
			{
				text = FilteredDragTool.FILTERLAYERS.WIRES;
			}
		}
		else
		{
			text = FilteredDragTool.FILTERLAYERS.LIQUIDCONDUIT;
		}
		this.currentFilterTargets = this.filterTargets;
		if (text != null)
		{
			List<string> list = new List<string>(this.filterTargets.Keys);
			foreach (string text2 in list)
			{
				this.filterTargets[text2] = ToolParameterMenu.ToggleState.Disabled;
				if (text2 == text)
				{
					this.filterTargets[text2] = ToolParameterMenu.ToggleState.On;
				}
			}
		}
		else
		{
			if (this.overlayFilterTargets.Count == 0)
			{
				this.ResetFilter(this.overlayFilterTargets);
			}
			this.currentFilterTargets = this.overlayFilterTargets;
		}
		ToolMenu.Instance.toolParameterMenu.PopulateMenu(this.currentFilterTargets);
	}

	private Dictionary<string, ToolParameterMenu.ToggleState> filterTargets = new Dictionary<string, ToolParameterMenu.ToggleState>();

	private Dictionary<string, ToolParameterMenu.ToggleState> overlayFilterTargets = new Dictionary<string, ToolParameterMenu.ToggleState>();

	private Dictionary<string, ToolParameterMenu.ToggleState> currentFilterTargets;

	private bool active;

	public class FILTERLAYERS
	{
		public static string BUILDINGS = "BUILDINGS";

		public static string TILES = "TILES";

		public static string WIRES = "WIRES";

		public static string LIQUIDCONDUIT = "LIQUIDPIPES";

		public static string GASCONDUIT = "GASPIPES";

		public static string CLEANANDCLEAR = "CLEANANDCLEAR";

		public static string DIGPLACER = "DIGPLACER";

		public static string ALL = "ALL";
	}
}
