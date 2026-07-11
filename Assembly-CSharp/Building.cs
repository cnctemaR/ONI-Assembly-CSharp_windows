using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using STRINGS;
using UnityEngine;

public class Building : KMonoBehaviour, IEffectDescriptor, IUniformGridObject, IApproachable
{
	public Orientation Orientation
	{
		get
		{
			return (!(this.rotatable != null)) ? Orientation.Neutral : this.rotatable.GetOrientation();
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
		if (this.extents.width == 0 || this.extents.height == 0)
		{
			this.RefreshCells();
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
			global::Debug.LogError("Missing building definition on object " + base.name, null);
		}
		KSelectable component = base.GetComponent<KSelectable>();
		if (component != null)
		{
			component.SetName(this.Def.Name);
			component.SetStatusIndicatorOffset(new Vector3(0f, -0.35f, 0f));
		}
		Prioritizable component2 = base.GetComponent<Prioritizable>();
		if (component2 != null)
		{
			component2.iconOffset.y = 0.3f;
		}
		KPrefabID component3 = base.GetComponent<KPrefabID>();
		if (component3.HasTag(RoomConstraints.ConstraintTags.IndustrialMachinery))
		{
			this.scenePartitionerEntry = GameScenePartitioner.Instance.Add(base.name, base.gameObject, this.GetExtents(), GameScenePartitioner.Instance.industrialBuildings, null);
		}
	}

	protected override void OnCleanUp()
	{
		if (this.scenePartitionerEntry != null)
		{
			this.scenePartitionerEntry.Release();
			this.scenePartitionerEntry = null;
		}
		base.OnCleanUp();
	}

	protected void RegisterBlockTileRenderer()
	{
		if (this.Def.BlockTileAtlas != null)
		{
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			if (component != null)
			{
				SimHashes visualizationElementID = this.GetVisualizationElementID(component);
				World.Instance.blockTileRenderer.AddBlock(base.gameObject.layer, this.Def, visualizationElementID, Grid.PosToCell(base.transform.GetPosition()));
			}
		}
	}

	public CellOffset GetRotatedOffset(CellOffset offset)
	{
		return (!(this.rotatable != null)) ? offset : this.rotatable.GetRotatedCellOffset(offset);
	}

	private int GetBottomLeftCell()
	{
		Vector3 position = base.transform.GetPosition();
		return Grid.PosToCell(position);
	}

	public int GetPowerInputCell()
	{
		CellOffset rotatedOffset = this.GetRotatedOffset(this.Def.PowerInputOffset);
		return Grid.OffsetCell(this.GetBottomLeftCell(), rotatedOffset);
	}

	public int GetPowerOutputCell()
	{
		CellOffset rotatedOffset = this.GetRotatedOffset(this.Def.PowerOutputOffset);
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
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			if (component != null)
			{
				SimHashes visualizationElementID = this.GetVisualizationElementID(component);
				World.Instance.blockTileRenderer.RemoveBlock(this.Def, visualizationElementID, Grid.PosToCell(base.transform.GetPosition()));
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

	public List<Descriptor> RequirementDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		if (def.RequiresPowerInput)
		{
			float wattsNeededWhenActive = base.GetComponent<IEnergyConsumer>().WattsNeededWhenActive;
			if (wattsNeededWhenActive > 0f)
			{
				string formattedWattage = GameUtil.GetFormattedWattage(wattsNeededWhenActive, GameUtil.WattageFormatterUnit.Automatic);
				Descriptor descriptor = new Descriptor(string.Format(UI.BUILDINGEFFECTS.REQUIRESPOWER, formattedWattage), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESPOWER, formattedWattage), Descriptor.DescriptorType.Requirement, false);
				list.Add(descriptor);
			}
		}
		if (def.InputConduitType == ConduitType.Liquid)
		{
			Descriptor descriptor2 = default(Descriptor);
			descriptor2.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESLIQUIDINPUT, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESLIQUIDINPUT, Descriptor.DescriptorType.Requirement);
			list.Add(descriptor2);
		}
		else if (def.InputConduitType == ConduitType.Gas)
		{
			Descriptor descriptor3 = default(Descriptor);
			descriptor3.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESGASINPUT, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESGASINPUT, Descriptor.DescriptorType.Requirement);
			list.Add(descriptor3);
		}
		if (def.OutputConduitType == ConduitType.Liquid)
		{
			Descriptor descriptor4 = default(Descriptor);
			descriptor4.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESLIQUIDOUTPUT, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESLIQUIDOUTPUT, Descriptor.DescriptorType.Requirement);
			list.Add(descriptor4);
		}
		else if (def.OutputConduitType == ConduitType.Gas)
		{
			Descriptor descriptor5 = default(Descriptor);
			descriptor5.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESGASOUTPUT, UI.BUILDINGEFFECTS.REQUIRESGASOUTPUT, Descriptor.DescriptorType.Requirement);
			list.Add(descriptor5);
		}
		if (component.isManuallyOperated)
		{
			Descriptor descriptor6 = default(Descriptor);
			descriptor6.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESMANUALOPERATION, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESMANUALOPERATION, Descriptor.DescriptorType.Requirement);
			list.Add(descriptor6);
		}
		if (component.isArtable)
		{
			Descriptor descriptor7 = default(Descriptor);
			descriptor7.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESCREATIVITY, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESCREATIVITY, Descriptor.DescriptorType.Requirement);
			list.Add(descriptor7);
		}
		return list;
	}

	public List<Descriptor> EffectDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (def.EffectDescription != null)
		{
			list.AddRange(def.EffectDescription);
		}
		if (def.GeneratorWattageRating > 0f && base.GetComponent<Battery>() == null)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ENERGYGENERATED, GameUtil.GetFormattedWattage(def.GeneratorWattageRating, GameUtil.WattageFormatterUnit.Automatic)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ENERGYGENERATED, GameUtil.GetFormattedWattage(def.GeneratorWattageRating, GameUtil.WattageFormatterUnit.Automatic)), Descriptor.DescriptorType.Effect);
			list.Add(descriptor);
		}
		if (def.ExhaustKilowattsWhenActive > 0f || def.SelfHeatKilowattsWhenActive > 0f)
		{
			Descriptor descriptor2 = default(Descriptor);
			descriptor2.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.HEATGENERATED, GameUtil.GetFormattedWattage(5f * (def.ExhaustKilowattsWhenActive + def.SelfHeatKilowattsWhenActive), GameUtil.WattageFormatterUnit.Automatic)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED, GameUtil.GetFormattedJoules(5f * (def.ExhaustKilowattsWhenActive + def.SelfHeatKilowattsWhenActive), "F1", GameUtil.TimeSlice.None)), Descriptor.DescriptorType.Effect);
			list.Add(descriptor2);
		}
		return list;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in this.RequirementDescriptors(def))
		{
			list.Add(descriptor);
		}
		foreach (Descriptor descriptor2 in this.EffectDescriptors(def))
		{
			list.Add(descriptor2);
		}
		return list;
	}

	public override Vector2 PosMin()
	{
		Extents extents = this.GetExtents();
		return new Vector2((float)extents.x, (float)extents.y);
	}

	public override Vector2 PosMax()
	{
		Extents extents = this.GetExtents();
		return new Vector2((float)(extents.x + extents.width), (float)(extents.y + extents.height));
	}

	public CellOffset[] GetOffsets()
	{
		return OffsetGroups.Use;
	}

	public int GetCell()
	{
		return Grid.PosToCell(this);
	}

	public bool ShouldPreferPrimaryCell()
	{
		return false;
	}

	public bool ShouldPreferUnreservedCell()
	{
		return false;
	}

	Transform IApproachable.get_transform()
	{
		return base.transform;
	}

	public BuildingDef Def;

	[MyCmpGet]
	private Rotatable rotatable;

	[MyCmpAdd]
	private StateMachineController stateMachineController;

	private int[] placementCells;

	private Extents extents;

	private GameScenePartitionerEntry scenePartitionerEntry;
}
