using System;
using System.Collections.Generic;
using UnityEngine;

public class FilteredDragTool : DragTool
{
	public bool IsActiveLayer(string layer)
	{
		return this.currentFilterTargets[ToolParameterMenu.FILTERLAYERS.ALL] == ToolParameterMenu.ToggleState.On || (this.currentFilterTargets.ContainsKey(layer.ToUpper()) && this.currentFilterTargets[layer.ToUpper()] == ToolParameterMenu.ToggleState.On);
	}

	public bool IsActiveLayer(ObjectLayer layer)
	{
		if (this.currentFilterTargets.ContainsKey(ToolParameterMenu.FILTERLAYERS.ALL) && this.currentFilterTargets[ToolParameterMenu.FILTERLAYERS.ALL] == ToolParameterMenu.ToggleState.On)
		{
			return true;
		}
		bool flag = false;
		foreach (KeyValuePair<string, ToolParameterMenu.ToggleState> keyValuePair in this.currentFilterTargets)
		{
			if (keyValuePair.Value == ToolParameterMenu.ToggleState.On)
			{
				ObjectLayer objectLayerFromFilterLayer = this.GetObjectLayerFromFilterLayer(keyValuePair.Key);
				if (objectLayerFromFilterLayer == layer)
				{
					flag = true;
					break;
				}
			}
		}
		return flag;
	}

	protected virtual void GetDefaultFilters(Dictionary<string, ToolParameterMenu.ToggleState> filters)
	{
		filters.Add(ToolParameterMenu.FILTERLAYERS.ALL, ToolParameterMenu.ToggleState.On);
		filters.Add(ToolParameterMenu.FILTERLAYERS.WIRES, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.GASCONDUIT, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.SOLIDCONDUIT, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.BUILDINGS, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.LOGIC, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.BACKWALL, ToolParameterMenu.ToggleState.Off);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.ResetFilter(this.filterTargets);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		OverlayScreen instance = OverlayScreen.Instance;
		instance.OnOverlayChanged = (Action<HashedString>)Delegate.Combine(instance.OnOverlayChanged, new Action<HashedString>(this.OnOverlayChanged));
	}

	protected override void OnCleanUp()
	{
		OverlayScreen instance = OverlayScreen.Instance;
		instance.OnOverlayChanged = (Action<HashedString>)Delegate.Remove(instance.OnOverlayChanged, new Action<HashedString>(this.OnOverlayChanged));
		base.OnCleanUp();
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

	protected string GetFilterLayerFromObjectLayer(ObjectLayer gamer_layer)
	{
		switch (gamer_layer)
		{
		case ObjectLayer.GasConduitConnection:
			break;
		case ObjectLayer.LiquidConduit:
		case ObjectLayer.LiquidConduitConnection:
			return "LiquidPipes";
		default:
			if (gamer_layer == ObjectLayer.Building)
			{
				return "Buildings";
			}
			if (gamer_layer != ObjectLayer.Backwall)
			{
				switch (gamer_layer)
				{
				case ObjectLayer.FoundationTile:
					return "Tiles";
				case ObjectLayer.GasConduit:
					goto IL_0083;
				}
				return "Default";
			}
			return "BackWall";
		case ObjectLayer.SolidConduit:
		case ObjectLayer.SolidConduitConnection:
			return "SolidConduits";
		case ObjectLayer.Wire:
			return "Wires";
		case ObjectLayer.LogicGates:
		case ObjectLayer.LogicWires:
			return "Logic";
		}
		IL_0083:
		return "GasPipes";
	}

	private ObjectLayer GetObjectLayerFromFilterLayer(string filter_layer)
	{
		string text = filter_layer.ToLower();
		if (text != null)
		{
			if (FilteredDragTool.<>f__switch$map0 == null)
			{
				FilteredDragTool.<>f__switch$map0 = new Dictionary<string, int>(8)
				{
					{ "buildings", 0 },
					{ "wires", 1 },
					{ "liquidpipes", 2 },
					{ "gaspipes", 3 },
					{ "solidconduits", 4 },
					{ "tiles", 5 },
					{ "logic", 6 },
					{ "backwall", 7 }
				};
			}
			int num;
			if (FilteredDragTool.<>f__switch$map0.TryGetValue(text, out num))
			{
				ObjectLayer objectLayer;
				switch (num)
				{
				case 0:
					objectLayer = ObjectLayer.Building;
					break;
				case 1:
					objectLayer = ObjectLayer.Wire;
					break;
				case 2:
					objectLayer = ObjectLayer.LiquidConduit;
					break;
				case 3:
					objectLayer = ObjectLayer.GasConduit;
					break;
				case 4:
					objectLayer = ObjectLayer.SolidConduit;
					break;
				case 5:
					objectLayer = ObjectLayer.FoundationTile;
					break;
				case 6:
					objectLayer = ObjectLayer.LogicWires;
					break;
				case 7:
					objectLayer = ObjectLayer.Backwall;
					break;
				case 8:
					goto IL_0106;
				default:
					goto IL_0106;
				}
				return objectLayer;
			}
		}
		IL_0106:
		throw new ArgumentException("Invalid filter layer: " + filter_layer);
	}

	private void OnOverlayChanged(HashedString overlay)
	{
		if (!this.active)
		{
			return;
		}
		string text = null;
		if (overlay == OverlayModes.Power.ID)
		{
			text = ToolParameterMenu.FILTERLAYERS.WIRES;
		}
		else if (overlay == OverlayModes.LiquidConduits.ID)
		{
			text = ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT;
		}
		else if (overlay == OverlayModes.GasConduits.ID)
		{
			text = ToolParameterMenu.FILTERLAYERS.GASCONDUIT;
		}
		else if (overlay == OverlayModes.SolidConveyor.ID)
		{
			text = ToolParameterMenu.FILTERLAYERS.SOLIDCONDUIT;
		}
		else if (overlay == OverlayModes.Logic.ID)
		{
			text = ToolParameterMenu.FILTERLAYERS.LOGIC;
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
}
