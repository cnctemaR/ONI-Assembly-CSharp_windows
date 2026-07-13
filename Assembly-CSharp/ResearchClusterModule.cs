using System;
using UnityEngine;

public class ResearchClusterModule : GameStateMachine<ResearchClusterModule, ResearchClusterModule.Instance, IStateMachineTarget, ResearchClusterModule.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.grounded;
		this.root.EventHandler(GameHashes.RocketLanded, new StateMachine<ResearchClusterModule, ResearchClusterModule.Instance, IStateMachineTarget, ResearchClusterModule.Def>.State.Callback(ResearchClusterModule.DropInventory));
		this.grounded.TagTransition(GameTags.RocketNotOnGround, this.space, false);
		this.space.TagTransition(GameTags.RocketNotOnGround, this.grounded, true).DefaultState(this.space.idle);
		this.space.idle.EventHandlerTransition(GameHashes.OnStorageChange, this.space.full, new Func<ResearchClusterModule.Instance, object, bool>(ResearchClusterModule.IsStorageFull)).Target(this.ClusterCraft).EventHandlerTransition(GameHashes.TagsChanged, this.space.collecting, new Func<ResearchClusterModule.Instance, object, bool>(ResearchClusterModule.IsCollectingDatabanks));
		this.space.collecting.EventHandlerTransition(GameHashes.OnStorageChange, this.space.full, new Func<ResearchClusterModule.Instance, object, bool>(ResearchClusterModule.IsStorageFull)).Target(this.ClusterCraft).EventHandlerTransition(GameHashes.TagsChanged, this.space.collecting, new Func<ResearchClusterModule.Instance, object, bool>(ResearchClusterModule.IsNotCollectingDatabanks));
		this.space.full.EventHandlerTransition(GameHashes.OnStorageChange, this.space.idle, new Func<ResearchClusterModule.Instance, object, bool>(ResearchClusterModule.StorageIsNotFull));
	}

	public static void DropInventory(ResearchClusterModule.Instance smi)
	{
		smi.DropInventory();
	}

	public static bool IsNotCollectingDatabanks(ResearchClusterModule.Instance smi, object o)
	{
		return !smi.IsCollectingDatabanks;
	}

	public static bool IsCollectingDatabanks(ResearchClusterModule.Instance smi, object o)
	{
		return smi.IsCollectingDatabanks;
	}

	public static bool IsStorageFull(ResearchClusterModule.Instance smi, object o)
	{
		return smi.IsStorageFull;
	}

	public static bool StorageIsNotFull(ResearchClusterModule.Instance smi, object o)
	{
		return !smi.IsStorageFull;
	}

	public GameStateMachine<ResearchClusterModule, ResearchClusterModule.Instance, IStateMachineTarget, ResearchClusterModule.Def>.State grounded;

	public ResearchClusterModule.InSpaceStates space;

	public StateMachine<ResearchClusterModule, ResearchClusterModule.Instance, IStateMachineTarget, ResearchClusterModule.Def>.TargetParameter ClusterCraft;

	public class Def : StateMachine.BaseDef
	{
	}

	public class InSpaceStates : GameStateMachine<ResearchClusterModule, ResearchClusterModule.Instance, IStateMachineTarget, ResearchClusterModule.Def>.State
	{
		public GameStateMachine<ResearchClusterModule, ResearchClusterModule.Instance, IStateMachineTarget, ResearchClusterModule.Def>.State idle;

		public GameStateMachine<ResearchClusterModule, ResearchClusterModule.Instance, IStateMachineTarget, ResearchClusterModule.Def>.State collecting;

		public GameStateMachine<ResearchClusterModule, ResearchClusterModule.Instance, IStateMachineTarget, ResearchClusterModule.Def>.State full;
	}

	public new class Instance : GameStateMachine<ResearchClusterModule, ResearchClusterModule.Instance, IStateMachineTarget, ResearchClusterModule.Def>.GameInstance
	{
		public bool IsStorageFull
		{
			get
			{
				return this.storage != null && this.storage.IsFull();
			}
		}

		public bool IsCollectingDatabanks
		{
			get
			{
				return this.collector != null && this.collector.IsCollecting;
			}
		}

		public Instance(IStateMachineTarget master, ResearchClusterModule.Def def)
			: base(master, def)
		{
			this.storage = base.GetComponent<Storage>();
			this.collector = base.gameObject.GetSMI<RocketModuleHexCellCollector.Instance>();
		}

		public override void StartSM()
		{
			this.clustercraft = base.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			base.sm.ClusterCraft.Set(this.clustercraft.gameObject, this, false);
			base.StartSM();
		}

		public void DropInventory()
		{
			this.storage.DropAll(false, false, default(Vector3), true, null);
		}

		private Storage storage;

		private RocketModuleHexCellCollector.Instance collector;

		private Clustercraft clustercraft;
	}
}
