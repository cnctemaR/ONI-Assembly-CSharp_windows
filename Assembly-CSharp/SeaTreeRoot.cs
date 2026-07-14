using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SeaTreeRoot : PlantBranchGrowerBase<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.growing;
		this.growing.InitializeStates(this.masterTarget, this.dead).DefaultState(this.growing.growing);
		this.growing.growing.ParamTransition<bool>(this.IsGrown, this.grown, GameStateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.IsTrue).PlayAnim("grow", KAnim.PlayMode.Once).OnAnimQueueComplete(this.growing.growing_pst);
		this.growing.growing_pst.Enter(new StateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.State.Callback(SeaTreeRoot.MarkAsGrown)).PlayAnim("grow_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.grown);
		this.grown.InitializeStates(this.masterTarget, this.dead).DefaultState(this.grown.growingBranches);
		this.grown.growingBranches.EventTransition(GameHashes.Wilt, this.grown.wilt, (SeaTreeRoot.Instance smi) => smi.IsWilting).ParamTransition<GameObject>(this.Branch, this.grown.idle, (SeaTreeRoot.Instance smi, GameObject b) => SeaTreeRoot.HasGrownBranch(smi)).PlayAnim("idle_full", KAnim.PlayMode.Loop)
			.Enter(new StateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.State.Callback(SeaTreeRoot.SpawnBranchIfNewGameSpawn))
			.Update(new Action<SeaTreeRoot.Instance, float>(SeaTreeRoot.AttemptToSpawnBranch), UpdateRate.SIM_4000ms, false)
			.DefaultState(this.grown.growingBranches.growing);
		this.grown.growingBranches.growing.ParamTransition<GameObject>(this.Branch, this.grown.growingBranches.blocked, (SeaTreeRoot.Instance smi, GameObject b) => SeaTreeRoot.HasNoBranch(smi));
		this.grown.growingBranches.blocked.ParamTransition<GameObject>(this.Branch, this.grown.growingBranches.growing, GameStateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.IsNotNull);
		this.grown.idle.EventTransition(GameHashes.Wilt, this.grown.wilt, (SeaTreeRoot.Instance smi) => smi.IsWilting).ParamTransition<GameObject>(this.Branch, this.grown.growingBranches, GameStateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.IsNull).PlayAnim("idle_full", KAnim.PlayMode.Loop);
		this.grown.wilt.EventTransition(GameHashes.WiltRecover, this.grown.idle, (SeaTreeRoot.Instance smi) => !smi.IsWilting).PlayAnim("wilt3", KAnim.PlayMode.Loop);
		this.dead.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead, null).Enter(delegate(SeaTreeRoot.Instance smi)
		{
			if (!smi.IsWild && !smi.GetComponent<KPrefabID>().HasTag(GameTags.Uprooted))
			{
				Notifier notifier = smi.gameObject.AddOrGet<Notifier>();
				Notification notification = SeaTreeRoot.CreateDeathNotification(smi);
				notifier.Add(notification, "");
			}
			GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
			smi.Trigger(1623392196, null);
			smi.DestroySelf(null);
		});
	}

	private static void MarkAsGrown(SeaTreeRoot.Instance smi)
	{
		smi.sm.IsGrown.Set(true, smi, false);
	}

	private static bool HasNoBranch(SeaTreeRoot.Instance smi)
	{
		return smi.Branch == null;
	}

	private static bool HasGrownBranch(SeaTreeRoot.Instance smi)
	{
		return smi.HasABranch;
	}

	private static void SpawnBranchIfNewGameSpawn(SeaTreeRoot.Instance smi)
	{
		if (smi.IsNewGameSpawned)
		{
			SeaTreeRoot.AttemptToSpawnBranches(smi);
		}
	}

	private static void AttemptToSpawnBranch(SeaTreeRoot.Instance smi, float dt)
	{
		SeaTreeRoot.AttemptToSpawnBranches(smi);
	}

	private static void AttemptToSpawnBranches(SeaTreeRoot.Instance smi)
	{
		smi.AttemptToSpawnBranches();
	}

	public static Notification CreateDeathNotification(SeaTreeRoot.Instance smi)
	{
		return new Notification(CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION, NotificationType.Bad, (List<Notification> notificationList, object data) => CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), "/t• " + smi.gameObject.GetProperName(), true, 0f, null, null, null, true, false, false);
	}

	private const string GROW_ANIM_NAME = "grow";

	private const string GROW_PST_ANIM_NAME = "grow_pst";

	private const string IDLE_ANIM_NAME = "idle_full";

	private const string WILT_ANIM_NAME = "wilt3";

	public GameStateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.State dead;

	public SeaTreeRoot.GrowingStates growing;

	public SeaTreeRoot.GrownStates grown;

	public StateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.BoolParameter IsGrown;

	public StateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.TargetParameter Branch;

	public class Def : PlantBranchGrowerBase<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.PlantBranchGrowerBaseDef
	{
	}

	public class GrowingBranchesStates : GameStateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.State
	{
		public GameStateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.State growing;

		public GameStateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.State blocked;
	}

	public class GrownStates : GameStateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.PlantAliveSubState
	{
		public SeaTreeRoot.GrowingBranchesStates growingBranches;

		public GameStateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.State idle;

		public GameStateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.State wilt;
	}

	public class GrowingStates : GameStateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.PlantAliveSubState
	{
		public GameStateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.State growing;

		public GameStateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.State growing_pst;
	}

	public new class Instance : GameStateMachine<SeaTreeRoot, SeaTreeRoot.Instance, IStateMachineTarget, SeaTreeRoot.Def>.GameInstance
	{
		public GameObject Branch
		{
			get
			{
				return base.sm.Branch.Get(this);
			}
		}

		public bool HasABranch
		{
			get
			{
				return this.Branch != null;
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

		public Instance(IStateMachineTarget master, SeaTreeRoot.Def def)
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
			if (this.Branch == null)
			{
				int num2 = Grid.OffsetCell(num, new CellOffset(0, 2));
				if (SeaTreeBranch.CanGrowOnCell(base.gameObject, num2))
				{
					GameObject gameObject = this.SpawnBranchOnCell(num2);
					base.sm.Branch.Set(gameObject, this, false);
					if (this.IsNewGameSpawned)
					{
						gameObject.Trigger(1119167081, null);
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
			SeaTreeRoot.MarkAsGrown(this);
		}

		private GameObject SpawnBranchOnCell(int cell)
		{
			Vector3 vector = Grid.CellToPosCBC(cell, Grid.SceneLayer.BuildingFront);
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(base.def.BRANCH_PREFAB_NAME), vector);
			gameObject.SetActive(true);
			gameObject.GetSMI<SeaTreeBranch.Instance>().SetupRootInformation(this);
			return gameObject;
		}

		public void UpdateAutoHarvestValue()
		{
			HarvestDesignatable component = base.GetComponent<HarvestDesignatable>();
			if (component != null && this.Branch != null)
			{
				SeaTreeBranch.Instance smi = this.Branch.GetSMI<SeaTreeBranch.Instance>();
				if (smi != null)
				{
					smi.SetAutoHarvestInChainReaction(component.HarvestWhenReady);
				}
			}
		}

		public bool IsNewGameSpawned;

		private Growing growing;

		private ReceptacleMonitor receptacleMonitor;

		private WiltCondition wiltCondition;
	}
}
