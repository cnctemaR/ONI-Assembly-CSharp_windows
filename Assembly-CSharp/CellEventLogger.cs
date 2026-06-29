using System;
using System.Collections.Generic;
using System.Diagnostics;

public class CellEventLogger : EventLogger<CellEventInstance, CellEvent>
{
	[Conditional("UNITY_EDITOR")]
	public void LogCallbackSend(int cell, int callback_id)
	{
		if (callback_id != -1)
		{
			this.CallbackToCellMap[callback_id] = cell;
		}
	}

	[Conditional("UNITY_EDITOR")]
	public void LogCallbackReceive(int callback_id)
	{
		int invalidCell = Grid.InvalidCell;
		if (this.CallbackToCellMap.TryGetValue(callback_id, out invalidCell))
		{
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		CellEventLogger.Instance = this;
		this.SimMessagesSolid = base.AddEvent(new CellSolidEvent("SimMessageSolid", "Sim Message", false, true)) as CellSolidEvent;
		this.SimCellOccupierDestroy = base.AddEvent(new CellSolidEvent("SimCellOccupierClearSolid", "Sim Cell Occupier Destroy", false, true)) as CellSolidEvent;
		this.SimCellOccupierForceSolid = base.AddEvent(new CellSolidEvent("SimCellOccupierForceSolid", "Sim Cell Occupier Force Solid", false, true)) as CellSolidEvent;
		this.SimCellOccupierSolidChanged = base.AddEvent(new CellSolidEvent("SimCellOccupierSolidChanged", "Sim Cell Occupier Solid Changed", false, true)) as CellSolidEvent;
		this.DoorOpen = base.AddEvent(new CellElementEvent("DoorOpen", "Door Open", true, true)) as CellElementEvent;
		this.DoorClose = base.AddEvent(new CellElementEvent("DoorClose", "Door Close", true, true)) as CellElementEvent;
		this.Excavator = base.AddEvent(new CellElementEvent("Excavator", "Excavator", true, true)) as CellElementEvent;
		this.DebugTool = base.AddEvent(new CellElementEvent("DebugTool", "Debug Tool", true, true)) as CellElementEvent;
		this.SandBoxTool = base.AddEvent(new CellElementEvent("SandBoxTool", "Sandbox Tool", true, true)) as CellElementEvent;
		this.TemplateLoader = base.AddEvent(new CellElementEvent("TemplateLoader", "Template Loader", true, true)) as CellElementEvent;
		this.Scenario = base.AddEvent(new CellElementEvent("Scenario", "Scenario", true, true)) as CellElementEvent;
		this.SimCellOccupierOnSpawn = base.AddEvent(new CellElementEvent("SimCellOccupierOnSpawn", "Sim Cell Occupier OnSpawn", true, true)) as CellElementEvent;
		this.SimCellOccupierDestroySelf = base.AddEvent(new CellElementEvent("SimCellOccupierDestroySelf", "Sim Cell Occupier Destroy Self", true, true)) as CellElementEvent;
		this.WorldGapManager = base.AddEvent(new CellElementEvent("WorldGapManager", "World Gap Manager", true, true)) as CellElementEvent;
		this.ReceiveElementChanged = base.AddEvent(new CellElementEvent("ReceiveElementChanged", "Sim Message", false, false)) as CellElementEvent;
		this.ObjectSetSimOnSpawn = base.AddEvent(new CellElementEvent("ObjectSetSimOnSpawn", "Object set sim on spawn", true, true)) as CellElementEvent;
		this.DecompositionDirtyWater = base.AddEvent(new CellElementEvent("DecompositionDirtyWater", "Decomposition dirty water", true, true)) as CellElementEvent;
		this.SendCallback = base.AddEvent(new CellCallbackEvent("SendCallback", true, true)) as CellCallbackEvent;
		this.ReceiveCallback = base.AddEvent(new CellCallbackEvent("ReceiveCallback", false, true)) as CellCallbackEvent;
		this.Dig = base.AddEvent(new CellDigEvent(true)) as CellDigEvent;
		this.WorldDamageDelayedSpawnFX = base.AddEvent(new CellAddRemoveSubstanceEvent("WorldDamageDelayedSpawnFX", "World Damage Delayed Spawn FX", false)) as CellAddRemoveSubstanceEvent;
		this.OxygenModifierSimUpdate = base.AddEvent(new CellAddRemoveSubstanceEvent("OxygenModifierSimUpdate", "Oxygen Modifier SimUpdate", false)) as CellAddRemoveSubstanceEvent;
		this.LiquidChunkOnStore = base.AddEvent(new CellAddRemoveSubstanceEvent("LiquidChunkOnStore", "Liquid Chunk On Store", false)) as CellAddRemoveSubstanceEvent;
		this.FallingWaterAddToSim = base.AddEvent(new CellAddRemoveSubstanceEvent("FallingWaterAddToSim", "Falling Water Add To Sim", false)) as CellAddRemoveSubstanceEvent;
		this.ExploderOnSpawn = base.AddEvent(new CellAddRemoveSubstanceEvent("ExploderOnSpawn", "Exploder OnSpawn", false)) as CellAddRemoveSubstanceEvent;
		this.ExhaustSimUpdate = base.AddEvent(new CellAddRemoveSubstanceEvent("ExhaustSimUpdate", "Exhaust SimUpdate", false)) as CellAddRemoveSubstanceEvent;
		this.ElementConsumerSimUpdate = base.AddEvent(new CellAddRemoveSubstanceEvent("ElementConsumerSimUpdate", "Element Consumer SimUpdate", false)) as CellAddRemoveSubstanceEvent;
		this.SublimatesEmit = base.AddEvent(new CellAddRemoveSubstanceEvent("SublimatesEmit", "Sublimates Emit", false)) as CellAddRemoveSubstanceEvent;
		this.Mop = base.AddEvent(new CellAddRemoveSubstanceEvent("Mop", "Mop", false)) as CellAddRemoveSubstanceEvent;
		this.OreMelted = base.AddEvent(new CellAddRemoveSubstanceEvent("OreMelted", "Ore Melted", false)) as CellAddRemoveSubstanceEvent;
		this.ConstructTile = base.AddEvent(new CellAddRemoveSubstanceEvent("ConstructTile", "ConstructTile", false)) as CellAddRemoveSubstanceEvent;
		this.Dumpable = base.AddEvent(new CellAddRemoveSubstanceEvent("Dympable", "Dumpable", false)) as CellAddRemoveSubstanceEvent;
		this.Cough = base.AddEvent(new CellAddRemoveSubstanceEvent("Cough", "Cough", false)) as CellAddRemoveSubstanceEvent;
		this.ElementChunkTransition = base.AddEvent(new CellAddRemoveSubstanceEvent("ElementChunkTransition", "Element Chunk Transition", false)) as CellAddRemoveSubstanceEvent;
		this.OxyrockEmit = base.AddEvent(new CellAddRemoveSubstanceEvent("OxyrockEmit", "Oxyrock Emit", false)) as CellAddRemoveSubstanceEvent;
		this.BleachstoneEmit = base.AddEvent(new CellAddRemoveSubstanceEvent("BleachstoneEmit", "Bleachstone Emit", false)) as CellAddRemoveSubstanceEvent;
		this.UnstableGround = base.AddEvent(new CellAddRemoveSubstanceEvent("UnstableGround", "Unstable Ground", false)) as CellAddRemoveSubstanceEvent;
		this.ConduitFlowEmptyConduit = base.AddEvent(new CellAddRemoveSubstanceEvent("ConduitFlowEmptyConduit", "Conduit Flow Empty Conduit", false)) as CellAddRemoveSubstanceEvent;
		this.ConduitConsumerWrongElement = base.AddEvent(new CellAddRemoveSubstanceEvent("ConduitConsumerWrongElement", "Conduit Consumer Wrong Element", false)) as CellAddRemoveSubstanceEvent;
		this.OverheatableMeltingDown = base.AddEvent(new CellAddRemoveSubstanceEvent("OverheatableMeltingDown", "Overheatable MeltingDown", false)) as CellAddRemoveSubstanceEvent;
		this.FabricatorProduceMelted = base.AddEvent(new CellAddRemoveSubstanceEvent("FabricatorProduceMelted", "Fabricator Produce Melted", false)) as CellAddRemoveSubstanceEvent;
		this.PumpSimUpdate = base.AddEvent(new CellAddRemoveSubstanceEvent("PumpSimUpdate", "Pump SimUpdate", false)) as CellAddRemoveSubstanceEvent;
		this.WallPumpSimUpdate = base.AddEvent(new CellAddRemoveSubstanceEvent("WallPumpSimUpdate", "Wall Pump SimUpdate", false)) as CellAddRemoveSubstanceEvent;
		this.Vomit = base.AddEvent(new CellAddRemoveSubstanceEvent("Vomit", "Vomit", false)) as CellAddRemoveSubstanceEvent;
		this.Tears = base.AddEvent(new CellAddRemoveSubstanceEvent("Tears", "Tears", false)) as CellAddRemoveSubstanceEvent;
		this.Pee = base.AddEvent(new CellAddRemoveSubstanceEvent("Pee", "Pee", false)) as CellAddRemoveSubstanceEvent;
		this.AlgaeHabitat = base.AddEvent(new CellAddRemoveSubstanceEvent("AlgaeHabitat", "AlgaeHabitat", false)) as CellAddRemoveSubstanceEvent;
		this.CO2FilterOxygen = base.AddEvent(new CellAddRemoveSubstanceEvent("CO2FilterOxygen", "CO2FilterOxygen", false)) as CellAddRemoveSubstanceEvent;
		this.ToiletEmit = base.AddEvent(new CellAddRemoveSubstanceEvent("ToiletEmit", "ToiletEmit", false)) as CellAddRemoveSubstanceEvent;
		this.ElementEmitted = base.AddEvent(new CellAddRemoveSubstanceEvent("ElementEmitted", "Element Emitted", false)) as CellAddRemoveSubstanceEvent;
		this.CO2ManagerFixedUpdate = base.AddEvent(new CellModifyMassEvent("CO2ManagerFixedUpdate", "CO2Manager FixedUpdate", false)) as CellModifyMassEvent;
		this.EnvironmentConsumerFixedUpdate = base.AddEvent(new CellModifyMassEvent("EnvironmentConsumerFixedUpdate", "EnvironmentConsumer FixedUpdate", false)) as CellModifyMassEvent;
		this.ExcavatorShockwave = base.AddEvent(new CellModifyMassEvent("ExcavatorShockwave", "Excavator Shockwave", false)) as CellModifyMassEvent;
		this.OxygenBreatherSimUpdate = base.AddEvent(new CellModifyMassEvent("OxygenBreatherSimUpdate", "Oxygen Breather SimUpdate", false)) as CellModifyMassEvent;
		this.CO2ScrubberSimUpdate = base.AddEvent(new CellModifyMassEvent("CO2ScrubberSimUpdate", "CO2Scrubber SimUpdate", false)) as CellModifyMassEvent;
		this.RiverSourceSimUpdate = base.AddEvent(new CellModifyMassEvent("RiverSourceSimUpdate", "RiverSource SimUpdate", false)) as CellModifyMassEvent;
		this.RiverTerminusSimUpdate = base.AddEvent(new CellModifyMassEvent("RiverTerminusSimUpdate", "RiverTerminus SimUpdate", false)) as CellModifyMassEvent;
		this.DebugToolModifyMass = base.AddEvent(new CellModifyMassEvent("DebugToolModifyMass", "DebugTool ModifyMass", false)) as CellModifyMassEvent;
		this.EnergyGeneratorModifyMass = base.AddEvent(new CellModifyMassEvent("EnergyGeneratorModifyMass", "EnergyGenerator ModifyMass", false)) as CellModifyMassEvent;
		this.SolidFilterEvent = base.AddEvent(new CellSolidFilterEvent("SolidFilterEvent", true)) as CellSolidFilterEvent;
	}

	public static CellEventLogger Instance;

	public CellSolidEvent SimMessagesSolid;

	public CellSolidEvent SimCellOccupierDestroy;

	public CellSolidEvent SimCellOccupierForceSolid;

	public CellSolidEvent SimCellOccupierSolidChanged;

	public CellElementEvent DoorOpen;

	public CellElementEvent DoorClose;

	public CellElementEvent Excavator;

	public CellElementEvent DebugTool;

	public CellElementEvent SandBoxTool;

	public CellElementEvent TemplateLoader;

	public CellElementEvent Scenario;

	public CellElementEvent SimCellOccupierOnSpawn;

	public CellElementEvent SimCellOccupierDestroySelf;

	public CellElementEvent WorldGapManager;

	public CellElementEvent ReceiveElementChanged;

	public CellElementEvent ObjectSetSimOnSpawn;

	public CellElementEvent DecompositionDirtyWater;

	public CellCallbackEvent SendCallback;

	public CellCallbackEvent ReceiveCallback;

	public CellDigEvent Dig;

	public CellAddRemoveSubstanceEvent WorldDamageDelayedSpawnFX;

	public CellAddRemoveSubstanceEvent SublimatesEmit;

	public CellAddRemoveSubstanceEvent OxygenModifierSimUpdate;

	public CellAddRemoveSubstanceEvent LiquidChunkOnStore;

	public CellAddRemoveSubstanceEvent FallingWaterAddToSim;

	public CellAddRemoveSubstanceEvent ExploderOnSpawn;

	public CellAddRemoveSubstanceEvent ExhaustSimUpdate;

	public CellAddRemoveSubstanceEvent ElementConsumerSimUpdate;

	public CellAddRemoveSubstanceEvent ElementChunkTransition;

	public CellAddRemoveSubstanceEvent OxyrockEmit;

	public CellAddRemoveSubstanceEvent BleachstoneEmit;

	public CellAddRemoveSubstanceEvent UnstableGround;

	public CellAddRemoveSubstanceEvent ConduitFlowEmptyConduit;

	public CellAddRemoveSubstanceEvent ConduitConsumerWrongElement;

	public CellAddRemoveSubstanceEvent OverheatableMeltingDown;

	public CellAddRemoveSubstanceEvent FabricatorProduceMelted;

	public CellAddRemoveSubstanceEvent PumpSimUpdate;

	public CellAddRemoveSubstanceEvent WallPumpSimUpdate;

	public CellAddRemoveSubstanceEvent Vomit;

	public CellAddRemoveSubstanceEvent Tears;

	public CellAddRemoveSubstanceEvent Pee;

	public CellAddRemoveSubstanceEvent AlgaeHabitat;

	public CellAddRemoveSubstanceEvent CO2FilterOxygen;

	public CellAddRemoveSubstanceEvent ToiletEmit;

	public CellAddRemoveSubstanceEvent ElementEmitted;

	public CellAddRemoveSubstanceEvent Mop;

	public CellAddRemoveSubstanceEvent OreMelted;

	public CellAddRemoveSubstanceEvent ConstructTile;

	public CellAddRemoveSubstanceEvent Dumpable;

	public CellAddRemoveSubstanceEvent Cough;

	public CellModifyMassEvent CO2ManagerFixedUpdate;

	public CellModifyMassEvent EnvironmentConsumerFixedUpdate;

	public CellModifyMassEvent ExcavatorShockwave;

	public CellModifyMassEvent OxygenBreatherSimUpdate;

	public CellModifyMassEvent CO2ScrubberSimUpdate;

	public CellModifyMassEvent RiverSourceSimUpdate;

	public CellModifyMassEvent RiverTerminusSimUpdate;

	public CellModifyMassEvent DebugToolModifyMass;

	public CellModifyMassEvent EnergyGeneratorModifyMass;

	public CellSolidFilterEvent SolidFilterEvent;

	public Dictionary<int, int> CallbackToCellMap = new Dictionary<int, int>();
}
