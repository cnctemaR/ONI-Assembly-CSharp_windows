using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Building : KMonoBehaviour, ISaveLoadableJson, IEffectDescriptor
{
	public int DescriptionOrder { get; set; }

	public Orientation Orientation
	{
		get
		{
			return (!(this.rotatable != null)) ? Orientation.None : this.rotatable.GetOrientation();
		}
	}

	public int[] PlacementCells
	{
		get
		{
			if (this.placementCells == null)
			{
				this.RefreshCells();
			}
			return this.placementCells;
		}
	}

	public Extents GetExtents()
	{
		if (this.PlacementCells != null)
		{
			return this.extents;
		}
		return this.extents;
	}

	public Extents GetValidPlacementExtents()
	{
		Extents extents = this.GetExtents();
		if (this.Def.BuildLocationRule != BuildLocationRule.OnCeiling)
		{
			extents.y--;
		}
		extents.height++;
		return extents;
	}

	public void RefreshCells()
	{
		this.placementCells = new int[this.Def.PlacementOffsets.Length];
		int num = Grid.PosToCell(this);
		Orientation orientation = this.Orientation;
		for (int i = 0; i < this.Def.PlacementOffsets.Length; i++)
		{
			CellOffset cellOffset = this.Def.PlacementOffsets[i];
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(cellOffset, orientation);
			int num2 = Grid.OffsetCell(num, rotatedCellOffset);
			this.placementCells[i] = num2;
		}
		int num3 = 0;
		int num4 = 0;
		Grid.CellToXY(this.placementCells[0], out num3, out num4);
		int num5 = num3;
		int num6 = num4;
		foreach (int num7 in this.placementCells)
		{
			int num8 = 0;
			int num9 = 0;
			Grid.CellToXY(num7, out num8, out num9);
			num3 = Math.Min(num3, num8);
			num4 = Math.Min(num4, num9);
			num5 = Math.Max(num5, num8);
			num6 = Math.Max(num6, num9);
		}
		this.extents.x = num3;
		this.extents.y = num4;
		this.extents.width = num5 - num3 + 1;
		this.extents.height = num6 - num4 + 1;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	[OnDeserialized]
	internal void OnDeserialized()
	{
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		if (component != null && component.Temperature == 0f)
		{
			if (component.Element == null)
			{
				DeserializeWarnings.Instance.PrimaryElementHasNoElement.Warn(base.name + " primary element has no element.", base.gameObject);
			}
			else if (!(this is BuildingUnderConstruction))
			{
				DeserializeWarnings.Instance.BuildingTemeperatureIsZeroKelvin.Warn(base.name + " is at zero degrees kelvin. Resetting temperature.", null);
				component.Temperature = component.Element.defaultValues.temperature;
			}
		}
	}

	protected override void OnSpawn()
	{
		if (this.Def == null)
		{
			Debug.LogError("Missing building definition on object " + base.name);
		}
		KSelectable component = base.GetComponent<KSelectable>();
		if (component != null)
		{
			component.SetName(this.Def.Name);
		}
	}

	protected void RegisterBlockTileRenderer()
	{
		if (this.Def.BlockTileAtlas != null)
		{
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			if (component != null)
			{
				SimHashes visualizationElementID = this.GetVisualizationElementID(component);
				World.Instance.blockTileRenderer.AddBlock(base.gameObject.layer, this.Def, visualizationElementID, Grid.PosToCell(this.transform.position));
			}
		}
	}

	public bool IsValidBuildLocation(Vector3 pos, out string reason)
	{
		reason = null;
		int num = Grid.PosToCell(pos);
		bool flag = true;
		if (this.Def.BuildLocationRule == BuildLocationRule.OnFloor || this.Def.BuildLocationRule == BuildLocationRule.OnCeiling)
		{
			int num2 = -(this.Def.WidthInCells - 1) / 2;
			int num3 = this.Def.WidthInCells / 2;
			for (int i = num2; i <= num3; i++)
			{
				int num4 = ((this.Def.BuildLocationRule != BuildLocationRule.OnFloor) ? this.Def.HeightInCells : (-1));
				int num5 = Grid.OffsetCell(num, i, num4);
				if (!Grid.IsValidCell(num5))
				{
					reason = "Foundation is not too near edge of world";
					return false;
				}
				if (Grid.Objects[num, 22] != null)
				{
					reason = "Location occupied by plant";
					return false;
				}
				flag = flag && Grid.Solid[num5];
				if (!flag)
				{
					reason = "Foundation is not solid";
					return false;
				}
			}
		}
		else
		{
			if (this.Def.BuildLocationRule == BuildLocationRule.Anywhere)
			{
				return true;
			}
			if (this.Def.BuildLocationRule == BuildLocationRule.Tile)
			{
				flag = Grid.Solid[num];
				if (!flag)
				{
					reason = "Tile Solid should be covering cell but isn't";
					return false;
				}
			}
		}
		if (!flag)
		{
			reason = "Unknown reason.";
		}
		return flag;
	}

	public bool IsBuildingProneToCollapse(Vector3 pos)
	{
		int num = Grid.PosToCell(pos);
		bool flag = true;
		if (this.Def.BuildLocationRule == BuildLocationRule.OnFloor)
		{
			int num2 = -(this.Def.WidthInCells - 1) / 2;
			int num3 = this.Def.WidthInCells / 2;
			for (int i = num2; i <= num3; i++)
			{
				int num4 = Grid.OffsetCell(num, i, -1);
				if (!Grid.IsValidCell(num4))
				{
					flag = true;
					break;
				}
				if (Grid.Solid[num4])
				{
					flag = false;
					break;
				}
				if (Grid.Objects[num4, 3] != null)
				{
					flag = false;
					break;
				}
			}
		}
		else if (this.Def.BuildLocationRule == BuildLocationRule.Anywhere)
		{
			flag = false;
		}
		else if (this.Def.BuildLocationRule == BuildLocationRule.Tile)
		{
			flag = !Grid.Solid[num];
		}
		return flag;
	}

	public CellOffset GetRotatedOffset(CellOffset offset)
	{
		return (!(this.rotatable != null)) ? offset : this.rotatable.GetRotatedCellOffset(offset);
	}

	private int GetBottomLeftCell()
	{
		Vector3 position = this.transform.position;
		return Grid.PosToCell(position);
	}

	public int GetPowerInputCell()
	{
		CellOffset rotatedOffset = this.GetRotatedOffset(this.Def.PowerInputOffset);
		return Grid.OffsetCell(this.GetBottomLeftCell(), rotatedOffset);
	}

	public int GetUtilityInputCell()
	{
		CellOffset rotatedOffset = this.GetRotatedOffset(this.Def.UtilityInputOffset);
		return Grid.OffsetCell(this.GetBottomLeftCell(), rotatedOffset);
	}

	public int GetUtilityOutputCell()
	{
		CellOffset rotatedOffset = this.GetRotatedOffset(this.Def.UtilityOutputOffset);
		return Grid.OffsetCell(this.GetBottomLeftCell(), rotatedOffset);
	}

	public CellOffset GetUtilityInputOffset()
	{
		return this.GetRotatedOffset(this.Def.UtilityInputOffset);
	}

	public CellOffset GetUtilityOutputOffset()
	{
		return this.GetRotatedOffset(this.Def.UtilityOutputOffset);
	}

	protected void UnregisterBlockTileRenderer()
	{
		if (this.Def.BlockTileAtlas != null)
		{
			int num = Grid.PosToCell(this.transform.position);
			this.Def.UnmarkArea(num, this.Orientation, this.Def.TileLayer, base.gameObject);
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			if (component != null)
			{
				SimHashes visualizationElementID = this.GetVisualizationElementID(component);
				World.Instance.blockTileRenderer.RemoveBlock(this.Def, visualizationElementID, Grid.PosToCell(this.transform.position));
			}
		}
	}

	private SimHashes GetVisualizationElementID(PrimaryElement pe)
	{
		return (!(this is BuildingComplete)) ? SimHashes.Void : pe.ElementID;
	}

	public void RunOnArea(Action<int> callback)
	{
		this.Def.RunOnArea(Grid.PosToCell(this), this.Orientation, callback);
	}

	public List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		if (def.RequiresPower)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, string.Format(UI.BUILDINGEFFECTS.REQUIRESPOWER, GameUtil.GetFormattedWattage(base.GetComponent<EnergyConsumer>().WattsNeededWhenActive))), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESPOWER, GameUtil.GetFormattedWattage(base.GetComponent<EnergyConsumer>().WattsNeededWhenActive)));
			list.Add(descriptor);
		}
		if (def.InputConduitType == ConduitType.Liquid)
		{
			Descriptor descriptor2 = default(Descriptor);
			descriptor2.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, UI.BUILDINGEFFECTS.REQUIRESLIQUIDINPUT), UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESLIQUIDINPUT);
			list.Add(descriptor2);
		}
		if (def.InputConduitType == ConduitType.Gas)
		{
			Descriptor descriptor3 = default(Descriptor);
			descriptor3.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, UI.BUILDINGEFFECTS.REQUIRESGASINPUT), UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESGASINPUT);
			list.Add(descriptor3);
		}
		if (def.OutputConduitType == ConduitType.Liquid)
		{
			Descriptor descriptor4 = default(Descriptor);
			descriptor4.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, UI.BUILDINGEFFECTS.REQUIRESLIQUIDOUTPUT), UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESLIQUIDOUTPUT);
			list.Add(descriptor4);
		}
		if (def.OutputConduitType == ConduitType.Gas)
		{
			Descriptor descriptor5 = default(Descriptor);
			descriptor5.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, UI.BUILDINGEFFECTS.REQUIRESGASOUTPUT), UI.BUILDINGEFFECTS.REQUIRESGASOUTPUT);
			list.Add(descriptor5);
		}
		if (component.isManuallyOperated)
		{
			Descriptor descriptor6 = default(Descriptor);
			descriptor6.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, UI.BUILDINGEFFECTS.REQUIRESMANUALOPERATION), UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESMANUALOPERATION);
			list.Add(descriptor6);
		}
		return list;
	}

	public List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (def.GeneratorWattageRating > 0f && base.GetComponent<Battery>() == null)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, string.Format(UI.BUILDINGEFFECTS.ENERGYGENERATED, GameUtil.GetFormattedWattage(def.GeneratorWattageRating))), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ENERGYGENERATED, GameUtil.GetFormattedWattage(def.GeneratorWattageRating)));
			list.Add(descriptor);
		}
		if (def.TemperatureModificationWhenActive > 0f)
		{
			Descriptor descriptor2 = default(Descriptor);
			descriptor2.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, string.Format(UI.BUILDINGEFFECTS.HEATGENERATED, GameUtil.GetFormattedWattage(def.TemperatureModificationWhenActive))), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED, GameUtil.GetFormattedWattage(def.TemperatureModificationWhenActive)));
			list.Add(descriptor2);
		}
		return list;
	}

	public BuildingDef Def;

	[MyCmpGet]
	private Rotatable rotatable;

	[MyCmpAdd]
	private Approachable approachable;

	[MyCmpAdd]
	private StateMachineController stateMachineController;

	private int[] placementCells;

	private Extents extents;
}
