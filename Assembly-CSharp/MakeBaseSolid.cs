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
		int num = Grid.PosToCell(smi.gameObject);
		PrimaryElement component = smi.GetComponent<PrimaryElement>();
		SimMessages.ReplaceAndDisplaceElement(num, component.ElementID, CellEventLogger.Instance.SimCellOccupierOnSpawn, component.Mass, component.Temperature, byte.MaxValue, 0, -1);
		Grid.Objects[num, 9] = smi.gameObject;
		Grid.Foundation[num] = true;
		Grid.SetSolid(num, true, CellEventLogger.Instance.SimCellOccupierForceSolid);
		Grid.RenderedByWorld[num] = false;
		World.Instance.OnSolidChanged(num);
		GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, null);
	}

	private static void ConvertToVacuum(MakeBaseSolid.Instance smi)
	{
		int num = Grid.PosToCell(smi.gameObject);
		SimMessages.ReplaceAndDisplaceElement(num, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierOnSpawn, 0f, -1f, byte.MaxValue, 0, -1);
		Grid.Objects[num, 9] = null;
		Grid.Foundation[num] = false;
		Grid.SetSolid(num, false, CellEventLogger.Instance.SimCellOccupierDestroy);
		Grid.RenderedByWorld[num] = true;
		World.Instance.OnSolidChanged(num);
		GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, null);
	}

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<MakeBaseSolid, MakeBaseSolid.Instance, IStateMachineTarget, MakeBaseSolid.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, MakeBaseSolid.Def def)
			: base(master, def)
		{
		}
	}
}
