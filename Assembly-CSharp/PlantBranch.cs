using System;
using STRINGS;
using UnityEngine;

public class PlantBranch : GameStateMachine<PlantBranch, PlantBranch.Instance, IStateMachineTarget, PlantBranch.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.root;
	}

	private StateMachine<PlantBranch, PlantBranch.Instance, IStateMachineTarget, PlantBranch.Def>.TargetParameter Trunk;

	public class Def : StateMachine.BaseDef
	{
		public Action<PlantBranchGrower.Instance, PlantBranch.Instance> animationSetupCallback;

		public Action<PlantBranch.Instance> onEarlySpawn;
	}

	public new class Instance : GameStateMachine<PlantBranch, PlantBranch.Instance, IStateMachineTarget, PlantBranch.Def>.GameInstance, IWiltCause
	{
		public bool HasTrunk
		{
			get
			{
				return this.trunk != null && !this.trunk.IsNullOrDestroyed() && !this.trunk.isMasterNull;
			}
		}

		public Instance(IStateMachineTarget master, PlantBranch.Def def)
			: base(master, def)
		{
			this.SetOccupyGridSpace(true);
			base.Subscribe(1272413801, new Action<object>(this.OnHarvest));
		}

		public override void StartSM()
		{
			base.StartSM();
			Action<PlantBranch.Instance> onEarlySpawn = base.def.onEarlySpawn;
			if (onEarlySpawn != null)
			{
				onEarlySpawn(this);
			}
			this.trunk = this.GetTrunk();
			if (!this.HasTrunk)
			{
				global::Debug.LogWarning("Tree Branch loaded with missing trunk reference. Destroying...");
				Util.KDestroyGameObject(base.gameObject);
				return;
			}
			this.SubscribeToTrunk();
			Action<PlantBranchGrower.Instance, PlantBranch.Instance> animationSetupCallback = base.def.animationSetupCallback;
			if (animationSetupCallback == null)
			{
				return;
			}
			animationSetupCallback(this.trunk, this);
		}

		private void OnHarvest(object data)
		{
			if (this.HasTrunk)
			{
				this.trunk.OnBrancHarvested(this);
			}
		}

		protected override void OnCleanUp()
		{
			this.UnsubscribeToTrunk();
			this.SetOccupyGridSpace(false);
			base.OnCleanUp();
		}

		private void SetOccupyGridSpace(bool active)
		{
			int num = Grid.PosToCell(base.gameObject);
			if (!active)
			{
				if (Grid.Objects[num, 5] == base.gameObject)
				{
					Grid.Objects[num, 5] = null;
				}
				return;
			}
			GameObject gameObject = Grid.Objects[num, 5];
			if (gameObject != null && gameObject != base.gameObject)
			{
				global::Debug.LogWarningFormat(base.gameObject, "PlantBranch.SetOccupyGridSpace already occupied by {0}", new object[] { gameObject });
				Util.KDestroyGameObject(base.gameObject);
				return;
			}
			Grid.Objects[num, 5] = base.gameObject;
		}

		public void SetTrunk(PlantBranchGrower.Instance trunk)
		{
			this.trunk = trunk;
			base.smi.sm.Trunk.Set(trunk.gameObject, this, false);
			this.SubscribeToTrunk();
			Action<PlantBranchGrower.Instance, PlantBranch.Instance> animationSetupCallback = base.def.animationSetupCallback;
			if (animationSetupCallback == null)
			{
				return;
			}
			animationSetupCallback(trunk, this);
		}

		public PlantBranchGrower.Instance GetTrunk()
		{
			if (base.smi.sm.Trunk.IsNull(this))
			{
				return null;
			}
			return base.sm.Trunk.Get(this).GetSMI<PlantBranchGrower.Instance>();
		}

		private void SubscribeToTrunk()
		{
			if (!this.HasTrunk)
			{
				return;
			}
			if (this.trunkWiltHandle == -1)
			{
				this.trunkWiltHandle = this.trunk.gameObject.Subscribe(-724860998, new Action<object>(this.OnTrunkWilt));
			}
			if (this.trunkWiltRecoverHandle == -1)
			{
				this.trunkWiltRecoverHandle = this.trunk.gameObject.Subscribe(712767498, new Action<object>(this.OnTrunkRecover));
			}
			base.Trigger(912965142, !this.trunk.GetComponent<WiltCondition>().IsWilting());
			ReceptacleMonitor component = base.GetComponent<ReceptacleMonitor>();
			PlantablePlot receptacle = this.trunk.GetComponent<ReceptacleMonitor>().GetReceptacle();
			component.SetReceptacle(receptacle);
			this.trunk.RefreshBranchZPositionOffset(base.gameObject);
			base.GetComponent<BudUprootedMonitor>().SetParentObject(this.trunk.GetComponent<KPrefabID>());
		}

		private void UnsubscribeToTrunk()
		{
			if (!this.HasTrunk)
			{
				return;
			}
			this.trunk.gameObject.Unsubscribe(this.trunkWiltHandle);
			this.trunk.gameObject.Unsubscribe(this.trunkWiltRecoverHandle);
			this.trunkWiltHandle = -1;
			this.trunkWiltRecoverHandle = -1;
			this.trunk.OnBranchRemoved(base.gameObject);
		}

		private void OnTrunkWilt(object data = null)
		{
			base.Trigger(912965142, false);
		}

		private void OnTrunkRecover(object data = null)
		{
			base.Trigger(912965142, true);
		}

		public string WiltStateString
		{
			get
			{
				return "    • " + DUPLICANTS.STATS.TRUNKHEALTH.NAME;
			}
		}

		public WiltCondition.Condition[] Conditions
		{
			get
			{
				return new WiltCondition.Condition[] { WiltCondition.Condition.UnhealthyRoot };
			}
		}

		public PlantBranchGrower.Instance trunk;

		private int trunkWiltHandle = -1;

		private int trunkWiltRecoverHandle = -1;
	}
}
