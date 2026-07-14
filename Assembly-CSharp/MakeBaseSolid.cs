using System;

public class MakeBaseSolid : GameStateMachine<MakeBaseSolid, MakeBaseSolid.Instance, IStateMachineTarget, MakeBaseSolid.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.Enter(new StateMachine<MakeBaseSolid, MakeBaseSolid.Instance, IStateMachineTarget, MakeBaseSolid.Def>.State.Callback(MakeBaseSolid.ConvertToSolid)).Exit(new StateMachine<MakeBaseSolid, MakeBaseSolid.Instance, IStateMachineTarget, MakeBaseSolid.Def>.State.Callback(MakeBaseSolid.ConvertToVacuum));
	}

	private static void ConvertToSolid(MakeBaseSolid.Instance smi)
	{
		if (smi.buildingComplete == null)
		{
			return;
		}
		int num = Grid.PosToCell(smi.gameObject);
		PrimaryElement component = smi.GetComponent<PrimaryElement>();
		Building component2 = smi.GetComponent<Building>();
		foreach (CellOffset cellOffset in smi.def.solidOffsets)
		{
			CellOffset rotatedOffset = component2.GetRotatedOffset(cellOffset);
			int num2 = Grid.OffsetCell(num, rotatedOffset);
			if (smi.def.occupyFoundationLayer)
			{
				SimMessages.ReplaceAndDisplaceElement(num2, component.ElementID, CellEventLogger.Instance.SimCellOccupierOnSpawn, component.Mass, component.Temperature, byte.MaxValue, 0, -1);
				Grid.Objects[num2, 9] = smi.gameObject;
			}
			else
			{
				SimMessages.ReplaceAndDisplaceElement(num2, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierOnSpawn, 0f, 0f, byte.MaxValue, 0, -1);
			}
			Grid.Foundation[num2] = true;
			Grid.SetSolid(num2, true, CellEventLogger.Instance.SimCellOccupierForceSolid);
			SimMessages.SetCellProperties(num2, 103);
			SimMessages.SetStrength(num2, 0, 1f);
			Grid.RenderedByWorld[num2] = false;
			World.Instance.OnSolidChanged(num2);
			GameScenePartitioner.Instance.TriggerEvent(num2, GameScenePartitioner.Instance.solidChangedLayer, null);
		}
	}

	private static void ConvertToVacuum(MakeBaseSolid.Instance smi)
	{
		if (smi.buildingComplete == null)
		{
			return;
		}
		int num = Grid.PosToCell(smi.gameObject);
		Building component = smi.GetComponent<Building>();
		foreach (CellOffset cellOffset in smi.def.solidOffsets)
		{
			CellOffset rotatedOffset = component.GetRotatedOffset(cellOffset);
			int num2 = Grid.OffsetCell(num, rotatedOffset);
			SimMessages.ReplaceAndDisplaceElement(num2, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierOnSpawn, 0f, -1f, byte.MaxValue, 0, -1);
			Grid.Objects[num2, 9] = null;
			Grid.Foundation[num2] = false;
			Grid.SetSolid(num2, false, CellEventLogger.Instance.SimCellOccupierDestroy);
			SimMessages.ClearCellProperties(num2, 103);
			SimMessages.SetStrength(num2, 1, 1f);
			Grid.RenderedByWorld[num2] = true;
			World.Instance.OnSolidChanged(num2);
			GameScenePartitioner.Instance.TriggerEvent(num2, GameScenePartitioner.Instance.solidChangedLayer, null);
		}
	}

	private const Sim.Cell.Properties floorCellProperties = (Sim.Cell.Properties)103;

	public class Def : StateMachine.BaseDef
	{
		public CellOffset[] solidOffsets;

		public bool occupyFoundationLayer = true;
	}

	public new class Instance : GameStateMachine<MakeBaseSolid, MakeBaseSolid.Instance, IStateMachineTarget, MakeBaseSolid.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, MakeBaseSolid.Def def)
			: base(master, def)
		{
		}

		[MyCmpGet]
		public BuildingComplete buildingComplete;
	}
}
