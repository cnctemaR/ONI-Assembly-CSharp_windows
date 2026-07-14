using System;
using UnityEngine;

public class FilteredDragTool : DragTool
{
	protected bool IsActive
	{
		get
		{
			return this.active;
		}
	}

	private bool IsFilterOn(string name)
	{
		for (int i = 0; i < this.currentFilters.Length; i++)
		{
			if (this.currentFilters[i].name == name)
			{
				return this.currentFilters[i].IsOn;
			}
		}
		return false;
	}

	public bool IsActiveLayer(string layer)
	{
		return this.IsFilterOn(ToolParameterMenu.FILTERLAYERS.ALL) || this.IsFilterOn(layer.ToUpper());
	}

	public bool IsActiveLayer(ObjectLayer layer)
	{
		if (this.IsFilterOn(ToolParameterMenu.FILTERLAYERS.ALL))
		{
			return true;
		}
		for (int i = 0; i < this.currentFilters.Length; i++)
		{
			if (this.currentFilters[i].IsOn && this.GetObjectLayerFromFilterLayer(this.currentFilters[i].name) == layer)
			{
				return true;
			}
		}
		return false;
	}

	protected virtual void GetDefaultFilters(out ToolParameterMenu.ToggleData[] filters)
	{
		filters = new ToolParameterMenu.ToggleData[]
		{
			new ToolParameterMenu.ToggleData(ToolParameterMenu.FILTERLAYERS.ALL, ToolParameterMenu.ToggleState.On, false),
			new ToolParameterMenu.ToggleData(ToolParameterMenu.FILTERLAYERS.WIRES, ToolParameterMenu.ToggleState.Off, false),
			new ToolParameterMenu.ToggleData(ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, ToolParameterMenu.ToggleState.Off, false),
			new ToolParameterMenu.ToggleData(ToolParameterMenu.FILTERLAYERS.GASCONDUIT, ToolParameterMenu.ToggleState.Off, false),
			new ToolParameterMenu.ToggleData(ToolParameterMenu.FILTERLAYERS.SOLIDCONDUIT, ToolParameterMenu.ToggleState.Off, false),
			new ToolParameterMenu.ToggleData(ToolParameterMenu.FILTERLAYERS.BUILDINGS, ToolParameterMenu.ToggleState.Off, false),
			new ToolParameterMenu.ToggleData(ToolParameterMenu.FILTERLAYERS.LOGIC, ToolParameterMenu.ToggleState.Off, false),
			new ToolParameterMenu.ToggleData(ToolParameterMenu.FILTERLAYERS.BACKWALL, ToolParameterMenu.ToggleState.Off, false)
		};
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.ResetFilter();
		this.userSelectedFilters = this.CloneFilters(this.currentFilters);
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
		this.GetDefaultFilters(out this.currentFilters);
	}

	private ToolParameterMenu.ToggleData[] CloneFilters(ToolParameterMenu.ToggleData[] source)
	{
		ToolParameterMenu.ToggleData[] array = new ToolParameterMenu.ToggleData[source.Length];
		for (int i = 0; i < source.Length; i++)
		{
			array[i] = new ToolParameterMenu.ToggleData(source[i].name, source[i].state, source[i].isToggleInclusive);
		}
		return array;
	}

	private void SaveUserFilters()
	{
		this.userSelectedFilters = this.CloneFilters(this.currentFilters);
	}

	private void RestoreUserFilters()
	{
		if (this.userSelectedFilters != null)
		{
			this.currentFilters = this.CloneFilters(this.userSelectedFilters);
			return;
		}
		this.ResetFilter();
	}

	private void OnParametersChanged()
	{
		if (!this.isOverlayDriven)
		{
			this.SaveUserFilters();
		}
	}

	protected override void OnActivateTool()
	{
		this.active = true;
		base.OnActivateTool();
		ToolMenu.Instance.toolParameterMenu.onParametersChanged += this.OnParametersChanged;
		HashedString mode = OverlayScreen.Instance.mode;
		if (mode != this.lastAppliedOverlay)
		{
			this.OnOverlayChanged(mode);
			return;
		}
		ToolMenu.Instance.toolParameterMenu.PopulateMenu(this.currentFilters);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		this.active = false;
		ToolMenu.Instance.toolParameterMenu.onParametersChanged -= this.OnParametersChanged;
		ToolMenu.Instance.toolParameterMenu.ClearMenu();
		base.OnDeactivateTool(new_tool);
	}

	public virtual string GetFilterLayerFromGameObject(GameObject input)
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

	public string GetFilterLayerFromObjectLayer(ObjectLayer gamer_layer)
	{
		if (gamer_layer > ObjectLayer.FoundationTile)
		{
			switch (gamer_layer)
			{
			case ObjectLayer.GasConduit:
			case ObjectLayer.GasConduitConnection:
				return "GasPipes";
			case ObjectLayer.GasConduitTile:
			case ObjectLayer.ReplacementGasConduit:
			case ObjectLayer.LiquidConduitTile:
			case ObjectLayer.ReplacementLiquidConduit:
				goto IL_00AC;
			case ObjectLayer.LiquidConduit:
			case ObjectLayer.LiquidConduitConnection:
				return "LiquidPipes";
			case ObjectLayer.SolidConduit:
				break;
			default:
				switch (gamer_layer)
				{
				case ObjectLayer.SolidConduitConnection:
					break;
				case ObjectLayer.LadderTile:
				case ObjectLayer.ReplacementLadder:
				case ObjectLayer.WireTile:
				case ObjectLayer.ReplacementWire:
					goto IL_00AC;
				case ObjectLayer.Wire:
				case ObjectLayer.WireConnectors:
					return "Wires";
				case ObjectLayer.LogicGate:
				case ObjectLayer.LogicWire:
					return "Logic";
				default:
					if (gamer_layer == ObjectLayer.Gantry)
					{
						goto IL_007C;
					}
					goto IL_00AC;
				}
				break;
			}
			return "SolidConduits";
		}
		if (gamer_layer != ObjectLayer.Building)
		{
			if (gamer_layer == ObjectLayer.Backwall)
			{
				return "BackWall";
			}
			if (gamer_layer != ObjectLayer.FoundationTile)
			{
				goto IL_00AC;
			}
			return "Tiles";
		}
		IL_007C:
		return "Buildings";
		IL_00AC:
		return "Default";
	}

	private ObjectLayer GetObjectLayerFromFilterLayer(string filter_layer)
	{
		string text = filter_layer.ToLower();
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 2200975418U)
		{
			if (num <= 388608975U)
			{
				if (num != 25076977U)
				{
					if (num == 388608975U)
					{
						if (text == "solidconduits")
						{
							return ObjectLayer.SolidConduit;
						}
					}
				}
				else if (text == "wires")
				{
					return ObjectLayer.Wire;
				}
			}
			else if (num != 614364310U)
			{
				if (num == 2200975418U)
				{
					if (text == "backwall")
					{
						return ObjectLayer.Backwall;
					}
				}
			}
			else if (text == "liquidpipes")
			{
				return ObjectLayer.LiquidConduit;
			}
		}
		else if (num <= 2875565775U)
		{
			if (num != 2366751346U)
			{
				if (num == 2875565775U)
				{
					if (text == "gaspipes")
					{
						return ObjectLayer.GasConduit;
					}
				}
			}
			else if (text == "buildings")
			{
				return ObjectLayer.Building;
			}
		}
		else if (num != 3464443665U)
		{
			if (num == 4178729166U)
			{
				if (text == "tiles")
				{
					return ObjectLayer.FoundationTile;
				}
			}
		}
		else if (text == "logic")
		{
			return ObjectLayer.LogicWire;
		}
		throw new ArgumentException("Invalid filter layer: " + filter_layer);
	}

	protected virtual void OnOverlayChanged(HashedString overlay)
	{
		if (!this.active)
		{
			return;
		}
		if (GameUtil.IsCapturingTimeLapse())
		{
			return;
		}
		this.lastAppliedOverlay = overlay;
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
		if (text != null)
		{
			if (!this.isOverlayDriven)
			{
				this.SaveUserFilters();
			}
			this.isOverlayDriven = true;
			this.GetDefaultFilters(out this.currentFilters);
			for (int i = 0; i < this.currentFilters.Length; i++)
			{
				this.currentFilters[i].state = ((this.currentFilters[i].name == text) ? ToolParameterMenu.ToggleState.On : ToolParameterMenu.ToggleState.Disabled);
			}
		}
		else if (this.isOverlayDriven)
		{
			this.RestoreUserFilters();
			this.isOverlayDriven = false;
		}
		ToolMenu.Instance.toolParameterMenu.PopulateMenu(this.currentFilters);
	}

	protected ToolParameterMenu.ToggleData[] currentFilters = new ToolParameterMenu.ToggleData[0];

	private ToolParameterMenu.ToggleData[] userSelectedFilters;

	private bool active;

	private HashedString lastAppliedOverlay;

	private bool isOverlayDriven;
}
