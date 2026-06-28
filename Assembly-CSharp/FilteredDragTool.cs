using System;
using System.Collections.Generic;
using UnityEngine;

public class FilteredDragTool : DragTool
{
	public bool IsActiveLayer(string layer)
	{
		return this.currentFilterTargets[ToolParameterMenu.FILTERLAYERS.ALL] == ToolParameterMenu.ToggleState.On || (this.currentFilterTargets.ContainsKey(layer.ToUpper()) && this.currentFilterTargets[layer.ToUpper()] == ToolParameterMenu.ToggleState.On);
	}

	protected virtual void GetDefaultFilters(Dictionary<string, ToolParameterMenu.ToggleState> filters)
	{
		filters.Add(ToolParameterMenu.FILTERLAYERS.ALL, ToolParameterMenu.ToggleState.On);
		filters.Add(ToolParameterMenu.FILTERLAYERS.WIRES, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.GASCONDUIT, ToolParameterMenu.ToggleState.Off);
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
		instance.OnOverlayChanged = (Action<SimViewMode>)Delegate.Combine(instance.OnOverlayChanged, new Action<SimViewMode>(this.OnOverlayChanged));
	}

	protected override void OnCleanUp()
	{
		OverlayScreen instance = OverlayScreen.Instance;
		instance.OnOverlayChanged = (Action<SimViewMode>)Delegate.Remove(instance.OnOverlayChanged, new Action<SimViewMode>(this.OnOverlayChanged));
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
		string text;
		if (component)
		{
			text = this.GetFilterLayerFromObjectLayer(component.Def.ObjectLayer);
		}
		else if (component2)
		{
			text = this.GetFilterLayerFromObjectLayer(component2.Def.ObjectLayer);
		}
		else if (input.GetComponent<Clearable>() != null || input.GetComponent<Moppable>() != null)
		{
			text = "CleanAndClear";
		}
		else if (input.GetComponent<Diggable>() != null)
		{
			text = "DigPlacer";
		}
		else
		{
			text = "Default";
		}
		return text;
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
		case ObjectLayer.Wire:
			return "Wires";
		case ObjectLayer.LogicGates:
		case ObjectLayer.LogicWires:
			return "Logic";
		}
		IL_0083:
		return "GasPipes";
	}

	private void OnOverlayChanged(SimViewMode overlay)
	{
		if (this.active)
		{
			string text = null;
			if (overlay != SimViewMode.LiquidVentMap)
			{
				if (overlay != SimViewMode.PowerMap)
				{
					if (overlay != SimViewMode.GasVentMap)
					{
						if (overlay == SimViewMode.Logic)
						{
							text = ToolParameterMenu.FILTERLAYERS.LOGIC;
						}
					}
					else
					{
						text = ToolParameterMenu.FILTERLAYERS.GASCONDUIT;
					}
				}
				else
				{
					text = ToolParameterMenu.FILTERLAYERS.WIRES;
				}
			}
			else
			{
				text = ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT;
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
	}

	private Dictionary<string, ToolParameterMenu.ToggleState> filterTargets = new Dictionary<string, ToolParameterMenu.ToggleState>();

	private Dictionary<string, ToolParameterMenu.ToggleState> overlayFilterTargets = new Dictionary<string, ToolParameterMenu.ToggleState>();

	private Dictionary<string, ToolParameterMenu.ToggleState> currentFilterTargets;

	private bool active = false;
}
