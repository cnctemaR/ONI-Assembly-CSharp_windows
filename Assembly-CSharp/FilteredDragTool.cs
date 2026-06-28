using System;
using System.Collections.Generic;
using UnityEngine;

public class FilteredDragTool : DragTool
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.filterTargets = new Dictionary<string, bool>();
	}

	public virtual void ResetFilter()
	{
		this.filterTargets.Clear();
		this.filterTargets.Add(FilteredDragTool.FILTERLAYERS.ALL, true);
		this.filterTargets.Add(FilteredDragTool.FILTERLAYERS.WIRES, false);
		this.filterTargets.Add(FilteredDragTool.FILTERLAYERS.LIQUIDCONDUIT, false);
		this.filterTargets.Add(FilteredDragTool.FILTERLAYERS.GASCONDUIT, false);
		this.filterTargets.Add(FilteredDragTool.FILTERLAYERS.BUILDINGS, false);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		this.ResetFilter();
		ToolMenu.Instance.toolParameterMenu.PopulateMenu(this.filterTargets);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
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
		case ObjectLayer.Building:
			return "Buildings";
		case ObjectLayer.Wire:
			return "Wires";
		case ObjectLayer.FoundationTile:
			return "Tiles";
		case ObjectLayer.GasConduit:
			return "GasPipes";
		case ObjectLayer.LiquidConduit:
			return "LiquidPipes";
		}
		return "Default";
	}

	protected Dictionary<string, bool> filterTargets;

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
