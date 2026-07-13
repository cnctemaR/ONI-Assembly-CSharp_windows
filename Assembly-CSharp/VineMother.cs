using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class VineMother : PlantBranchGrowerBase<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.growing;
		this.growing.InitializeStates(this.masterTarget, this.dead).DefaultState(this.growing.growing);
		this.growing.growing.ParamTransition<bool>(this.IsGrown, this.grown, GameStateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.IsTrue).PlayAnim("grow", KAnim.PlayMode.Once).OnAnimQueueComplete(this.growing.growing_pst);
		this.growing.growing_pst.Enter(new StateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.State.Callback(VineMother.MarkAsGrown)).PlayAnim("grow_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.grown);
		this.grown.InitializeStates(this.masterTarget, this.dead).DefaultState(this.grown.growingBranches);
		this.grown.growingBranches.EventTransition(GameHashes.Wilt, this.grown.wilt, (VineMother.Instance smi) => smi.IsWilting).ParamTransition<GameObject>(this.LeftBranch, this.grown.idle, (VineMother.Instance smi, GameObject b) => VineMother.HasGrownAllBranches(smi)).ParamTransition<GameObject>(this.RightBranch, this.grown.idle, (VineMother.Instance smi, GameObject b) => VineMother.HasGrownAllBranches(smi))
			.PlayAnim("idle_full", KAnim.PlayMode.Loop)
			.Enter(new StateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.State.Callback(VineMother.SpawnBranchesIfNewGameSpawn))
			.Update(new Action<VineMother.Instance, float>(VineMother.AttemptToSpawnBranches), UpdateRate.SIM_4000ms, false)
			.DefaultState(this.grown.growingBranches.growing);
		this.grown.growingBranches.growing.ParamTransition<GameObject>(this.LeftBranch, this.grown.growingBranches.blocked, (VineMother.Instance smi, GameObject b) => VineMother.HasNoBranches(smi)).ParamTransition<GameObject>(this.RightBranch, this.grown.growingBranches.blocked, (VineMother.Instance smi, GameObject b) => VineMother.HasNoBranches(smi));
		this.grown.growingBranches.blocked.ParamTransition<GameObject>(this.LeftBranch, this.grown.growingBranches.growing, GameStateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.IsNotNull).ParamTransition<GameObject>(this.RightBranch, this.grown.growingBranches.growing, GameStateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.IsNotNull);
		this.grown.idle.EventTransition(GameHashes.Wilt, this.grown.wilt, (VineMother.Instance smi) => smi.IsWilting).ParamTransition<GameObject>(this.LeftBranch, this.grown.growingBranches, GameStateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.IsNull).ParamTransition<GameObject>(this.RightBranch, this.grown.growingBranches, GameStateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.IsNull)
			.PlayAnim("idle_full", KAnim.PlayMode.Loop);
		this.grown.wilt.EventTransition(GameHashes.WiltRecover, this.grown.idle, (VineMother.Instance smi) => !smi.IsWilting).PlayAnim("wilt3", KAnim.PlayMode.Loop);
		this.dead.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead, null).Enter(delegate(VineMother.Instance smi)
		{
			if (!smi.IsWild && !smi.GetComponent<KPrefabID>().HasTag(GameTags.Uprooted))
			{
				Notifier notifier = smi.gameObject.AddOrGet<Notifier>();
				Notification notification = VineMother.CreateDeathNotification(smi);
				notifier.Add(notification, "");
			}
			GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
			smi.Trigger(1623392196, null);
			smi.DestroySelf(null);
		});
	}

	private static void MarkAsGrown(VineMother.Instance smi)
	{
		smi.sm.IsGrown.Set(true, smi, false);
	}

	private static bool HasNoBranches(VineMother.Instance smi)
	{
		return smi.LeftBranch == null && smi.RightBranch == null;
	}

	private static bool HasGrownAllBranches(VineMother.Instance smi)
	{
		return smi.HasGrownAllBranches;
	}

	private static void SpawnBranchesIfNewGameSpawn(VineMother.Instance smi)
	{
		if (smi.IsNewGameSpawned)
		{
			VineMother.AttemptToSpawnBranches(smi);
		}
	}

	private static void AttemptToSpawnBranches(VineMother.Instance smi, float dt)
	{
		VineMother.AttemptToSpawnBranches(smi);
	}

	private static void AttemptToSpawnBranches(VineMother.Instance smi)
	{
		smi.AttemptToSpawnBranches();
	}

	public static Notification CreateDeathNotification(VineMother.Instance smi)
	{
		return new Notification(CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION, NotificationType.Bad, (List<Notification> notificationList, object data) => CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), "/t• " + smi.gameObject.GetProperName(), true, 0f, null, null, null, true, false, false);
	}

	private const string GROW_ANIM_NAME = "grow";

	private const string GROW_PST_ANIM_NAME = "grow_pst";

	private const string IDLE_ANIM_NAME = "idle_full";

	private const string WILT_ANIM_NAME = "wilt3";

	public GameStateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.State dead;

	public VineMother.GrowingStates growing;

	public VineMother.GrownStates grown;

	public StateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.BoolParameter IsGrown;

	public StateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.TargetParameter LeftBranch;

	public StateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.TargetParameter RightBranch;

	public class Def : PlantBranchGrowerBase<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.PlantBranchGrowerBaseDef
	{
	}

	public class GrowingBranchesStates : GameStateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.State
	{
		public GameStateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.State growing;

		public GameStateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.State blocked;
	}

	public class GrownStates : GameStateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.PlantAliveSubState
	{
		public VineMother.GrowingBranchesStates growingBranches;

		public GameStateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.State idle;

		public GameStateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.State wilt;
	}

	public class GrowingStates : GameStateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.PlantAliveSubState
	{
		public GameStateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.State growing;

		public GameStateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.State growing_pst;
	}

	public new class Instance : GameStateMachine<VineMother, VineMother.Instance, IStateMachineTarget, VineMother.Def>.GameInstance
	{
		public GameObject LeftBranch
		{
			get
			{
				return base.sm.LeftBranch.Get(this);
			}
		}

		public GameObject RightBranch
		{
			get
			{
				return base.sm.RightBranch.Get(this);
			}
		}

		public bool HasGrownAllBranches
		{
			get
			{
				return this.LeftBranch != null && this.RightBranch != null;
			}
		}

		public bool IsGrown
		{
			get
			{
				return this.growing.IsGrown();
			}
		}

		public bool IsWild
		{
			get
			{
				return !this.receptacleMonitor.Replanted;
			}
		}

		public bool IsOnPlanterBox
		{
			get
			{
				return !this.IsWild && this.receptacleMonitor.smi.ReceptacleObject != null && this.receptacleMonitor.smi.ReceptacleObject is PlantablePlot && (this.receptacleMonitor.smi.ReceptacleObject as PlantablePlot).IsOffGround;
			}
		}

		public int PlanterboxCell
		{
			get
			{
				if (!this.IsWild)
				{
					return Grid.PosToCell(this.receptacleMonitor.smi.ReceptacleObject);
				}
				return Grid.InvalidCell;
			}
		}

		public bool IsWilting
		{
			get
			{
				return this.wiltCondition.IsWilting();
			}
		}

		public Instance(IStateMachineTarget master, VineMother.Def def)
			: base(master, def)
		{
			this.growing = base.GetComponent<Growing>();
			this.receptacleMonitor = base.GetComponent<ReceptacleMonitor>();
			this.wiltCondition = base.GetComponent<WiltCondition>();
			base.Subscribe(1119167081, new Action<object>(this.OnSpawnedByDiscovered));
			base.Subscribe(-266953818, delegate(object obj)
			{
				this.UpdateAutoHarvestValue();
			});
		}

		public void AttemptToSpawnBranches()
		{
			int num = Grid.PosToCell(base.gameObject);
			if (this.LeftBranch == null)
			{
				int num2 = Grid.OffsetCell(num, CellOffset.left);
				if (VineBranch.IsCellAvailable(base.gameObject, num2, null))
				{
					GameObject gameObject = this.SpawnBranchOnCell(num2);
					base.sm.LeftBranch.Set(gameObject, this, false);
					if (this.IsNewGameSpawned)
					{
						gameObject.Trigger(1119167081, null);
					}
				}
			}
			if (this.RightBranch == null)
			{
				int num3 = Grid.OffsetCell(num, CellOffset.right);
				if (VineBranch.IsCellAvailable(base.gameObject, num3, null))
				{
					GameObject gameObject2 = this.SpawnBranchOnCell(num3);
					base.sm.RightBranch.Set(gameObject2, this, false);
					if (this.IsNewGameSpawned)
					{
						gameObject2.Trigger(1119167081, null);
					}
				}
			}
			if (this.IsNewGameSpawned)
			{
				this.IsNewGameSpawned = false;
			}
		}

		public void DestroySelf(object o)
		{
			CreatureHelpers.DeselectCreature(base.gameObject);
			Util.KDestroyGameObject(base.gameObject);
		}

		private void OnSpawnedByDiscovered(object o)
		{
			this.IsNewGameSpawned = true;
			VineMother.MarkAsGrown(this);
		}

		private GameObject SpawnBranchOnCell(int cell)
		{
			Vector3 vector = Grid.CellToPosCBC(cell, Grid.SceneLayer.BuildingFront);
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(base.def.BRANCH_PREFAB_NAME), vector);
			gameObject.SetActive(true);
			gameObject.GetSMI<VineBranch.Instance>().SetupRootInformation(this);
			return gameObject;
		}

		public void UpdateAutoHarvestValue()
		{
			HarvestDesignatable component = base.GetComponent<HarvestDesignatable>();
			if (component != null)
			{
				if (this.LeftBranch != null)
				{
					VineBranch.Instance smi = this.LeftBranch.GetSMI<VineBranch.Instance>();
					if (smi != null)
					{
						smi.SetAutoHarvestInChainReaction(component.HarvestWhenReady);
					}
				}
				if (this.RightBranch != null)
				{
					VineBranch.Instance smi2 = this.RightBranch.GetSMI<VineBranch.Instance>();
					if (smi2 != null)
					{
						smi2.SetAutoHarvestInChainReaction(component.HarvestWhenReady);
					}
				}
			}
		}

		public bool IsNewGameSpawned;

		private Growing growing;

		private ReceptacleMonitor receptacleMonitor;

		private WiltCondition wiltCondition;
	}
}
