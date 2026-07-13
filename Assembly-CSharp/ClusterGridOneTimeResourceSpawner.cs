using System;
using System.Collections.Generic;

public class ClusterGridOneTimeResourceSpawner : GameStateMachine<ClusterGridOneTimeResourceSpawner, ClusterGridOneTimeResourceSpawner.Instance, IStateMachineTarget, ClusterGridOneTimeResourceSpawner.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.enter;
		this.enter.ParamTransition<bool>(this.HasSpawnedResources, this.spawned, GameStateMachine<ClusterGridOneTimeResourceSpawner, ClusterGridOneTimeResourceSpawner.Instance, IStateMachineTarget, ClusterGridOneTimeResourceSpawner.Def>.IsTrue).ParamTransition<bool>(this.HasSpawnedResources, this.spawning, GameStateMachine<ClusterGridOneTimeResourceSpawner, ClusterGridOneTimeResourceSpawner.Instance, IStateMachineTarget, ClusterGridOneTimeResourceSpawner.Def>.IsFalse);
		this.spawning.ParamTransition<bool>(this.HasSpawnedResources, this.spawned, GameStateMachine<ClusterGridOneTimeResourceSpawner, ClusterGridOneTimeResourceSpawner.Instance, IStateMachineTarget, ClusterGridOneTimeResourceSpawner.Def>.IsTrue).Enter(new StateMachine<ClusterGridOneTimeResourceSpawner, ClusterGridOneTimeResourceSpawner.Instance, IStateMachineTarget, ClusterGridOneTimeResourceSpawner.Def>.State.Callback(ClusterGridOneTimeResourceSpawner.SpawnResources));
		this.spawned.DoNothing();
	}

	public static void SpawnResources(ClusterGridOneTimeResourceSpawner.Instance smi)
	{
		smi.SpawnResources();
	}

	public GameStateMachine<ClusterGridOneTimeResourceSpawner, ClusterGridOneTimeResourceSpawner.Instance, IStateMachineTarget, ClusterGridOneTimeResourceSpawner.Def>.State enter;

	public GameStateMachine<ClusterGridOneTimeResourceSpawner, ClusterGridOneTimeResourceSpawner.Instance, IStateMachineTarget, ClusterGridOneTimeResourceSpawner.Def>.State spawning;

	public GameStateMachine<ClusterGridOneTimeResourceSpawner, ClusterGridOneTimeResourceSpawner.Instance, IStateMachineTarget, ClusterGridOneTimeResourceSpawner.Def>.State spawned;

	public StateMachine<ClusterGridOneTimeResourceSpawner, ClusterGridOneTimeResourceSpawner.Instance, IStateMachineTarget, ClusterGridOneTimeResourceSpawner.Def>.BoolParameter HasSpawnedResources;

	public struct Data
	{
		public Tag itemID;

		public float mass;
	}

	public class Def : StateMachine.BaseDef
	{
		public List<ClusterGridOneTimeResourceSpawner.Data> thingsToSpawn;
	}

	public new class Instance : GameStateMachine<ClusterGridOneTimeResourceSpawner, ClusterGridOneTimeResourceSpawner.Instance, IStateMachineTarget, ClusterGridOneTimeResourceSpawner.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, ClusterGridOneTimeResourceSpawner.Def def)
			: base(master, def)
		{
		}

		public void SpawnResources()
		{
			StarmapHexCellInventory hexCellInventory = this.GetHexCellInventory();
			foreach (ClusterGridOneTimeResourceSpawner.Data data in base.def.thingsToSpawn)
			{
				hexCellInventory.AddItem(data.itemID, data.mass, Element.State.Vacuum).RecalculateState();
			}
			base.sm.HasSpawnedResources.Set(true, this, false);
		}

		public StarmapHexCellInventory GetHexCellInventory()
		{
			ClusterGridEntity component = base.GetComponent<ClusterGridEntity>();
			return ClusterGrid.Instance.AddOrGetHexCellInventory(component.Location);
		}
	}
}
