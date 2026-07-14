using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class SeaTreeBranch : PlantBranchGrowerBase<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>
{
	private static string GET_ANIM_NAME(bool isEndBranch, string animBaseName)
	{
		return (isEndBranch ? "end_branch_" : "branch_") + animBaseName;
	}

	private static string GetWiltAnimLevel(string baseSTR, float growingPercentage)
	{
		int num;
		if (growingPercentage < 0.75f)
		{
			num = 1;
		}
		else if (growingPercentage < 1f)
		{
			num = 2;
		}
		else
		{
			num = 3;
		}
		if (baseSTR == null)
		{
			return null;
		}
		if (!SeaTreeBranch.m_wilt.ContainsKey(baseSTR))
		{
			SeaTreeBranch.m_wilt[baseSTR] = new string[3];
			for (int i = 0; i < 3; i++)
			{
				SeaTreeBranch.m_wilt[baseSTR][i] = baseSTR + (i + 1).ToString();
			}
		}
		return SeaTreeBranch.m_wilt[baseSTR][num - 1];
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.earlyDeathHandler;
		this.earlyDeathHandler.ParamTransition<bool>(this.MarkedForDeath, this.dead, GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.IsTrue).ParamTransition<GameObject>(this.Root, this.dead, GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.IsNull).GoTo(this.undevelopedBranch);
		this.undevelopedBranch.InitializeStates(this.masterTarget, this.Root, this.dead, this.DieSignal).ParamTransition<bool>(this.MarkedForDeath, this.dead, GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.IsTrue).ParamTransition<GameObject>(this.Root, this.dead, GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.IsNull)
			.EventTransition(GameHashes.Grow, this.mature, (SeaTreeBranch.Instance smi) => smi.IsGrown)
			.UpdateTransition(this.mature, (SeaTreeBranch.Instance smi, float dt) => smi.IsGrown, UpdateRate.SIM_4000ms, false)
			.DefaultState(this.undevelopedBranch.growing);
		this.undevelopedBranch.wilted.PlayAnim(new Func<SeaTreeBranch.Instance, string>(SeaTreeBranch.GetWiltAnim), KAnim.PlayMode.Loop).EventTransition(GameHashes.WiltRecover, this.undevelopedBranch.growing, (SeaTreeBranch.Instance smi) => !smi.IsWilting);
		this.undevelopedBranch.growing.EventTransition(GameHashes.Wilt, this.undevelopedBranch.wilted, (SeaTreeBranch.Instance smi) => smi.IsWilting).PlayAnim((SeaTreeBranch.Instance smi) => SeaTreeBranch.GetAnimName(smi, "grow"), KAnim.PlayMode.Paused).ToggleStatusItem(Db.Get().CreatureStatusItems.Growing, (SeaTreeBranch.Instance smi) => smi)
			.Enter(new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.RefreshPositionPercent))
			.Update(new Action<SeaTreeBranch.Instance, float>(SeaTreeBranch.RefreshPositionPercent), UpdateRate.SIM_4000ms, false)
			.EventHandler(GameHashes.ConsumePlant, new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.RefreshPositionPercent))
			.DefaultState(this.undevelopedBranch.growing.wild);
		this.undevelopedBranch.growing.wild.ParamTransition<bool>(this.WildPlanted, this.undevelopedBranch.growing.domestic, GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.IsFalse).ToggleAttributeModifier("Growing", (SeaTreeBranch.Instance smi) => smi.wildGrowingRate, null);
		this.undevelopedBranch.growing.domestic.ParamTransition<bool>(this.WildPlanted, this.undevelopedBranch.growing.wild, GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.IsTrue).ToggleAttributeModifier("Growing", (SeaTreeBranch.Instance smi) => smi.baseGrowingRate, null);
		this.mature.InitializeStates(this.masterTarget, this.Root, this.dead, this.DieSignal).ParamTransition<bool>(this.MarkedForDeath, this.dead, GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.IsTrue).ParamTransition<GameObject>(this.Root, this.dead, GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.IsNull)
			.Enter(new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.SpawnBrancheIfSpawnedByDiscovery))
			.Enter(new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.SetupFruitMeter))
			.Update(new Action<SeaTreeBranch.Instance, float>(SeaTreeBranch.SpawnBranchIfPossible), UpdateRate.SIM_4000ms, false)
			.DefaultState(this.mature.healthy);
		this.mature.healthy.PlayAnim((SeaTreeBranch.Instance smi) => SeaTreeBranch.GetAnimName(smi, "idle"), KAnim.PlayMode.Loop).DefaultState(this.mature.healthy.growing);
		this.mature.healthy.growing.EventTransition(GameHashes.Grow, this.mature.healthy.harvestReady, (SeaTreeBranch.Instance smi) => smi.IsReadyForHarvest).UpdateTransition(this.mature.healthy.harvestReady, (SeaTreeBranch.Instance smi, float dt) => smi.IsReadyForHarvest, UpdateRate.SIM_4000ms, false).EventTransition(GameHashes.Wilt, this.mature.wilted, (SeaTreeBranch.Instance smi) => smi.IsWilting)
			.ToggleStatusItem(Db.Get().CreatureStatusItems.GrowingFruit, (SeaTreeBranch.Instance smi) => smi)
			.Enter(new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.UpdateFruitMeterGrowAnimations))
			.Update(new Action<SeaTreeBranch.Instance, float>(SeaTreeBranch.UpdateFruitMeterGrowAnimations), UpdateRate.SIM_200ms, false)
			.DefaultState(this.mature.healthy.growing.wild);
		this.mature.healthy.growing.wild.ParamTransition<bool>(this.WildPlanted, this.mature.healthy.growing.domestic, GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.IsFalse).ToggleAttributeModifier("Fruit Growing", (SeaTreeBranch.Instance smi) => smi.wildFruitGrowingRate, null);
		this.mature.healthy.growing.domestic.ParamTransition<bool>(this.WildPlanted, this.mature.healthy.growing.wild, GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.IsTrue).ToggleAttributeModifier("Fruit Growing", (SeaTreeBranch.Instance smi) => smi.baseFruitGrowingRate, null);
		this.mature.healthy.harvestReady.ToggleTag(GameTags.FullyGrown).EventTransition(GameHashes.Harvest, this.mature.healthy.harvest, null).Enter(new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.MakeItHarvestable))
			.Enter(delegate(SeaTreeBranch.Instance smi)
			{
				SeaTreeBranch.PlayAnimsOnFruit(smi, "bulb_meter_ready", KAnim.PlayMode.Loop);
			})
			.ToggleAttributeModifier("GetOld", (SeaTreeBranch.Instance smi) => smi.getOldRate, null)
			.UpdateTransition(this.mature.healthy.selfHarvestFromOld, new Func<SeaTreeBranch.Instance, float, bool>(SeaTreeBranch.ShouldSelfHarvestFromOldAge), UpdateRate.SIM_4000ms, false)
			.Exit(new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.ResetOldAge));
		this.mature.healthy.harvest.Target(this.Fruit).OnAnimQueueComplete(this.mature.healthy.spawnCritter).Target(this.masterTarget)
			.Enter(delegate(SeaTreeBranch.Instance smi)
			{
				SeaTreeBranch.PlayAnimsOnFruit(smi, "bulb_meter_birth", KAnim.PlayMode.Once);
			})
			.Enter(new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.CacheHarvesterWorker))
			.Enter(new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.MakeItNotHarvestable))
			.Enter(new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.ResetFruitGrowProgress))
			.TriggerOnExit(GameHashes.HarvestComplete, null)
			.ScheduleGoTo(3f, this.mature.healthy.spawnCritter);
		this.mature.healthy.selfHarvestFromOld.Target(this.Fruit).OnAnimQueueComplete(this.mature.healthy.spawnCritter).Target(this.masterTarget)
			.Enter(delegate(SeaTreeBranch.Instance smi)
			{
				SeaTreeBranch.PlayAnimsOnFruit(smi, "bulb_meter_birth", KAnim.PlayMode.Once);
			})
			.Enter(new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.ForceCancelHarvest))
			.Enter(new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.MakeItNotHarvestable))
			.Enter(new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.ResetOldAge))
			.Enter(new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.ResetFruitGrowProgress))
			.TriggerOnExit(GameHashes.HarvestComplete, null)
			.ScheduleGoTo(3f, this.mature.healthy.spawnCritter);
		this.mature.healthy.spawnCritter.Enter(new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.SpawnCritter)).EnterGoTo(this.mature.healthy.growing);
		this.mature.wilted.PlayAnim(new Func<SeaTreeBranch.Instance, string>(SeaTreeBranch.GetWiltAnim), KAnim.PlayMode.Loop).Enter(delegate(SeaTreeBranch.Instance smi)
		{
			SeaTreeBranch.PlayAnimsOnFruit(smi, SeaTreeBranch.GetFruitWiltAnim(smi), KAnim.PlayMode.Loop);
		}).EventTransition(GameHashes.WiltRecover, this.mature.healthy, (SeaTreeBranch.Instance smi) => !smi.IsWilting)
			.EventTransition(GameHashes.Harvest, this.mature.healthy.harvest, null);
		this.dead.Target(this.masterTarget).ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead, null).Enter(new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.HarvestOnDeath))
			.Enter(delegate(SeaTreeBranch.Instance smi)
			{
				if (!smi.gameObject.GetComponent<KPrefabID>().HasTag(GameTags.Uprooted) && !smi.IsWild)
				{
					Notifier notifier = smi.gameObject.AddOrGet<Notifier>();
					Notification notification = SeaTreeBranch.CreateDeathNotification(smi);
					notifier.Add(notification, "");
				}
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
				smi.Trigger(1623392196, null);
				smi.DestroySelf(null);
			});
	}

	private static bool ShouldSelfHarvestFromOldAge(SeaTreeBranch.Instance smi, float dt)
	{
		return smi.IsOld;
	}

	private static string GetWiltAnim(SeaTreeBranch.Instance smi)
	{
		return SeaTreeBranch.GetWiltAnimLevel(SeaTreeBranch.GetAnimName(smi, "wilted"), smi.GrowthPercentage);
	}

	private static string GetFruitWiltAnim(SeaTreeBranch.Instance smi)
	{
		return SeaTreeBranch.GetWiltAnimLevel("bulb_meter_wilt", smi.FruitGrowthPercentage);
	}

	private static void PlayAnimsOnFruit(SeaTreeBranch.Instance smi, string animName, KAnim.PlayMode playmode)
	{
		smi.PlayAnimOnFruitMeter(animName, playmode);
	}

	private static void UpdateFruitMeterGrowAnimations(SeaTreeBranch.Instance smi, float dt)
	{
		SeaTreeBranch.UpdateFruitMeterGrowAnimations(smi);
	}

	private static void UpdateFruitMeterGrowAnimations(SeaTreeBranch.Instance smi)
	{
		smi.UpdateFruitGrowMeterPosition();
	}

	private static void SetupFruitMeter(SeaTreeBranch.Instance smi)
	{
		smi.CreateFruitMeter();
	}

	private static void SpawnBranchIfPossible(SeaTreeBranch.Instance smi, float dt)
	{
		smi.AttemptToSpawnBranch();
	}

	private static void MakeItHarvestable(SeaTreeBranch.Instance smi)
	{
		smi.SetHarvestableState(true);
	}

	private static void ForceCancelHarvest(SeaTreeBranch.Instance smi)
	{
		smi.ForceCancelHarvest();
	}

	private static void MakeItNotHarvestable(SeaTreeBranch.Instance smi)
	{
		smi.SetHarvestableState(false);
	}

	private static void RefreshPositionPercent(SeaTreeBranch.Instance smi, float dt)
	{
		SeaTreeBranch.RefreshPositionPercent(smi);
	}

	private static void RefreshPositionPercent(SeaTreeBranch.Instance smi)
	{
		smi.animController.SetPositionPercent(smi.GrowthPercentage);
	}

	private static void ResetFruitGrowProgress(SeaTreeBranch.Instance smi)
	{
		smi.ResetFruitGrowProgress();
	}

	private static void ResetOldAge(SeaTreeBranch.Instance smi)
	{
		smi.ResetOldAge();
	}

	private static void SpawnCritter(SeaTreeBranch.Instance smi)
	{
		smi.SpawnCritter();
	}

	private static void OnRootRecovered(SeaTreeBranch.Instance smi)
	{
		smi.BoxingTrigger(912965142, true);
	}

	private static void OnRootWilted(SeaTreeBranch.Instance smi)
	{
		smi.BoxingTrigger(912965142, false);
	}

	public static string GetAnimName(SeaTreeBranch.Instance smi, string animName)
	{
		return SeaTreeBranch.GET_ANIM_NAME(smi.MaxBranchNumberReached, animName);
	}

	public static void CacheHarvesterWorker(SeaTreeBranch.Instance smi)
	{
		smi.CacheHarvesterWorker();
	}

	private static void SpawnBrancheIfSpawnedByDiscovery(SeaTreeBranch.Instance smi)
	{
		if (smi.IsNewGameSpawned)
		{
			SeaTreeBranch.SpawnBranchIfPossible(smi, 0f);
		}
	}

	public static void HarvestOnDeath(SeaTreeBranch.Instance smi)
	{
		bool isReadyForHarvest = smi.IsReadyForHarvest;
	}

	private static Notification CreateDeathNotification(SeaTreeBranch.Instance smi)
	{
		return new Notification(CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION, NotificationType.Bad, (List<Notification> notificationList, object data) => CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), "/t• " + smi.gameObject.GetProperName(), true, 0f, null, null, null, true, false, false);
	}

	public static bool CanGrowOnCell(GameObject questionerObj, int cell)
	{
		int num = Grid.PosToCell(questionerObj);
		int num2 = (int)Grid.WorldIdx[num];
		return cell != Grid.InvalidCell && (int)Grid.WorldIdx[cell] == num2 && Grid.IsLiquid(cell) && Grid.Objects[cell, 1] == null && Grid.Objects[cell, 5] == null;
	}

	public const string ANIM_PREFIX_COMMON_BRANCH = "branch_";

	public const string ANIM_PREFIX_END_BRANCH = "end_branch_";

	public const string ANIM_NAME_WILT_PREFIX = "wilted";

	public const string ANIM_NAME_GROWING = "grow";

	public const string ANIM_NAME_IDLE = "idle";

	public const string METER_TARGET_NAME = "bulb_meter_target";

	public const string METER_ANIM_NAME_WILT_PREFIX = "bulb_meter_wilt";

	public const string METER_ANIM_NAME_BIRTH = "bulb_meter_birth";

	public const string METER_ANIM_NAME_HARVEST = "bulb_meter_birth";

	public const string METER_ANIM_NAME_READY = "bulb_meter_ready";

	public const string METER_ANIM_NAME_GROWING = "bulb_meter_grow";

	public const string METER_DEFAULT_ANIM_NAME = "bulb_meter_grow";

	private const int WILT_LEVELS = 3;

	private static Dictionary<string, string[]> m_wilt = new Dictionary<string, string[]>();

	public StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.TargetParameter Fruit;

	public StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.TargetParameter Root;

	public StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.TargetParameter Branch;

	public StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.IntParameter BranchNumber;

	public StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.BoolParameter WildPlanted;

	public StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.BoolParameter MarkedForDeath;

	public StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.Signal DieSignal;

	public GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State earlyDeathHandler;

	public SeaTreeBranch.GrowingStates undevelopedBranch;

	public SeaTreeBranch.GrownStates mature;

	public GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State dead;

	public class Def : PlantBranchGrowerBase<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.PlantBranchGrowerBaseDef
	{
		public Tag SpawnCreatureID;

		public float GROWTH_RATE = 0.0016666667f;

		public float WILD_GROWTH_RATE = 0.00041666668f;
	}

	public class GrowingSpeedState : GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State
	{
		public GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State wild;

		public GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State domestic;
	}

	public class BranchAliveSubstate : GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.PlantAliveSubState
	{
		public GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State InitializeStates(StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.TargetParameter plant, StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.TargetParameter root, GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State death_state, StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.Signal dieSignal)
		{
			base.InitializeStates(plant, death_state);
			base.root.Target(plant).OnSignal(dieSignal, death_state).OnTargetLost(root, death_state)
				.Target(root)
				.EventHandler(GameHashes.Wilt, new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.OnRootWilted))
				.EventHandler(GameHashes.WiltRecover, new StateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State.Callback(SeaTreeBranch.OnRootRecovered))
				.Target(plant);
			return this;
		}
	}

	public class GrowingStates : SeaTreeBranch.BranchAliveSubstate
	{
		public SeaTreeBranch.GrowingSpeedState growing;

		public GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State wilted;
	}

	public class FruitGrowingStates : GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State
	{
		public SeaTreeBranch.GrowingSpeedState growing;

		public GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State wilted;

		public GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State harvestReady;

		public GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State selfHarvestFromOld;

		public GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State spawnCritter;

		public GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State harvest;
	}

	public class GrownStates : SeaTreeBranch.BranchAliveSubstate
	{
		public SeaTreeBranch.FruitGrowingStates healthy;

		public GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.State wilted;
	}

	public new class Instance : GameStateMachine<SeaTreeBranch, SeaTreeBranch.Instance, IStateMachineTarget, SeaTreeBranch.Def>.GameInstance, IManageGrowingStates, IWiltCause
	{
		public GameObject Root
		{
			get
			{
				return base.sm.Root.Get(this);
			}
		}

		public GameObject Branch
		{
			get
			{
				return base.sm.Branch.Get(this);
			}
		}

		public SeaTreeBranch.Instance BranchSMI
		{
			get
			{
				if (!(this.Branch == null))
				{
					return this.Branch.GetSMI<SeaTreeBranch.Instance>();
				}
				return null;
			}
		}

		public int MyBranchNumber
		{
			get
			{
				return base.sm.BranchNumber.Get(this);
			}
		}

		public bool IsWild
		{
			get
			{
				return base.sm.WildPlanted.Get(this);
			}
		}

		public bool MaxBranchNumberReached
		{
			get
			{
				return this.MyBranchNumber >= 8;
			}
		}

		public bool IsOld
		{
			get
			{
				return this.oldAge.value >= this.oldAge.GetMax();
			}
		}

		private bool IsRootWilting
		{
			get
			{
				return this.RootSMI != null && this.RootSMI.IsWilting;
			}
		}

		public bool IsWilting
		{
			get
			{
				return this.wiltCondition.IsWilting() || this.IsRootWilting;
			}
		}

		public bool IsGrown
		{
			get
			{
				return this.GrowthPercentage >= 1f;
			}
		}

		public float GrowthPercentage
		{
			get
			{
				return this.maturity.value / this.maturity.GetMax();
			}
		}

		public bool IsReadyForHarvest
		{
			get
			{
				return this.FruitGrowthPercentage >= 1f;
			}
		}

		public float FruitGrowthPercentage
		{
			get
			{
				return this.fruitMaturity.value / this.fruitMaturity.GetMax();
			}
		}

		public Instance(IStateMachineTarget master, SeaTreeBranch.Def def)
			: base(master, def)
		{
			Amounts amounts = base.gameObject.GetAmounts();
			this.maturity = amounts.Get(Db.Get().Amounts.Maturity);
			this.fruitMaturity = amounts.Get(Db.Get().Amounts.Maturity2);
			this.baseGrowingRate = new AttributeModifier(this.maturity.deltaAttribute.Id, def.GROWTH_RATE, CREATURES.STATS.MATURITY.GROWING, false, false, true);
			this.wildGrowingRate = new AttributeModifier(this.maturity.deltaAttribute.Id, def.WILD_GROWTH_RATE, CREATURES.STATS.MATURITY.GROWINGWILD, false, false, true);
			this.baseFruitGrowingRate = new AttributeModifier(this.fruitMaturity.deltaAttribute.Id, def.GROWTH_RATE, CREATURES.STATS.MATURITY.GROWING, false, false, true);
			this.wildFruitGrowingRate = new AttributeModifier(this.fruitMaturity.deltaAttribute.Id, def.WILD_GROWTH_RATE, CREATURES.STATS.MATURITY.GROWINGWILD, false, false, true);
			this.oldAge = amounts.Add(new AmountInstance(Db.Get().Amounts.OldAge, base.gameObject));
			this.oldAge.maxAttribute.ClearModifiers();
			this.oldAge.maxAttribute.Add(new AttributeModifier(Db.Get().Amounts.OldAge.maxAttribute.Id, 2400f, null, false, false, true));
			this.getOldRate = new AttributeModifier(this.oldAge.deltaAttribute.Id, 1f, null, false, false, true);
			this.wiltCondition = base.GetComponent<WiltCondition>();
			this.animController = base.GetComponent<KBatchedAnimController>();
			this.harvestable = base.GetComponent<Harvestable>();
			this.SetCellRegistrationAsPlant(true);
			base.Subscribe(1119167081, new Action<object>(this.OnSpawnedByDiscovery));
		}

		public override void StartSM()
		{
			this.wasMarkedForDeadBeforeStartSM = base.sm.MarkedForDeath.Get(this);
			base.master.gameObject.AddTag(GameTags.GrowingPlant);
			base.StartSM();
		}

		public override void PostParamsInitialized()
		{
			base.PostParamsInitialized();
			this.RootSMI = ((this.Root == null) ? null : this.Root.GetSMI<SeaTreeRoot.Instance>());
			if (this.wasMarkedForDeadBeforeStartSM)
			{
				base.sm.MarkedForDeath.Set(true, this, false);
			}
			this.HideAllFruitSymbols();
		}

		protected override void OnCleanUp()
		{
			this.DestroyFruitMeter();
			this.KillForwardBranch();
			this.SetCellRegistrationAsPlant(false);
			base.OnCleanUp();
		}

		public void DestroySelf(object o)
		{
			CreatureHelpers.DeselectCreature(base.gameObject);
			Util.KDestroyGameObject(base.gameObject);
		}

		public void SetCellRegistrationAsPlant(bool doRegister)
		{
			int num = Grid.PosToCell(this);
			if (doRegister && Grid.Objects[num, 5] == null)
			{
				Grid.Objects[num, 5] = base.gameObject;
				return;
			}
			if (!doRegister && Grid.Objects[num, 5] == base.gameObject)
			{
				Grid.Objects[num, 5] = null;
			}
		}

		public void SetHarvestableState(bool canBeHarvested)
		{
			this.harvestable.SetCanBeHarvested(canBeHarvested);
		}

		public void SetAutoHarvestInChainReaction(bool autoharvest)
		{
			HarvestDesignatable component = base.GetComponent<HarvestDesignatable>();
			if (component != null)
			{
				component.SetHarvestWhenReady(autoharvest);
				if (this.BranchSMI != null)
				{
					this.BranchSMI.SetAutoHarvestInChainReaction(autoharvest);
				}
			}
		}

		public void ForceCancelHarvest()
		{
			this.harvestable.ForceCancelHarvest(true);
		}

		public void ResetOldAge()
		{
			this.oldAge.SetValue(0f);
		}

		private void OnSpawnedByDiscovery(object o)
		{
			float num = 1f - (float)this.MyBranchNumber / (float)base.def.MAX_BRANCH_COUNT;
			float num2 = ((global::UnityEngine.Random.Range(0f, 1f) <= num) ? 1f : global::UnityEngine.Random.Range(0f, 1f));
			this.maturity.SetValue(this.maturity.maxAttribute.GetTotalValue() * num2);
			if (this.IsGrown)
			{
				this.IsNewGameSpawned = true;
				this.fruitMaturity.SetValue(this.fruitMaturity.maxAttribute.GetTotalValue() * global::UnityEngine.Random.Range(0f, 1f));
			}
		}

		public void CacheHarvesterWorker()
		{
			this.lastHarvesterWorker = this.harvestable.GetWorker();
		}

		public void SpawnCritter()
		{
			bool flag = this.lastHarvesterWorker != null;
			Crop component = base.GetComponent<Crop>();
			SeedProducer component2 = base.GetComponent<SeedProducer>();
			GameObject gameObject = component.SpawnAndGetConfiguredFruit(null, false);
			if (flag && component2 != null)
			{
				component2.SimulateCropPicked(this.lastHarvesterWorker);
			}
			bool flag2;
			Vector3 vector = this.animController.GetSymbolTransform("bulb_meter_target", out flag2).GetColumn(3);
			vector.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
			if (gameObject != null)
			{
				gameObject.transform.position = vector;
				gameObject.SetActive(true);
				gameObject.GetComponent<PrimaryElement>().Temperature = base.gameObject.GetComponent<PrimaryElement>().Temperature;
			}
			else
			{
				DebugUtil.LogErrorArgs(base.gameObject, new object[] { "failed at spawning critter for sea tree branch" });
			}
			this.lastHarvesterWorker = null;
		}

		public void ResetFruitGrowProgress()
		{
			this.fruitMaturity.SetValue(0f);
		}

		public void HideAllFruitSymbols()
		{
			this.animController.SetSymbolVisiblity("bulb_meter_target", false);
		}

		public void CreateFruitMeter()
		{
			this.DestroyFruitMeter();
			this.fruitMeter = new MeterController(this.animController, "bulb_meter_target", "bulb_meter_grow", Meter.Offset.NoChange, Grid.SceneLayer.Building, Array.Empty<string>());
			base.sm.Fruit.Set(this.fruitMeter.gameObject, this, false);
		}

		private void DestroyFruitMeter()
		{
			if (this.fruitMeter != null)
			{
				this.fruitMeter.Unlink();
				Util.KDestroyGameObject(this.fruitMeter.gameObject);
				this.fruitMeter = null;
				base.sm.Fruit.Set(null, this);
			}
		}

		public void PlayAnimOnFruitMeter(string animName, KAnim.PlayMode playMode)
		{
			if (this.fruitMeter != null)
			{
				this.fruitMeter.meterController.Play(animName, playMode, 1f, 0f);
			}
		}

		public void UpdateFruitGrowMeterPosition()
		{
			if (this.fruitMeter != null)
			{
				if (this.fruitMeter.meterController.currentAnim != "bulb_meter_grow")
				{
					this.PlayAnimOnFruitMeter("bulb_meter_grow", KAnim.PlayMode.Paused);
				}
				this.fruitMeter.SetPositionPercent(this.FruitGrowthPercentage);
			}
		}

		private void KillForwardBranch()
		{
			if (this.Branch != null)
			{
				SeaTreeBranch.Instance smi = this.Branch.GetSMI<SeaTreeBranch.Instance>();
				if (smi != null)
				{
					smi.sm.DieSignal.Trigger(smi);
					smi.sm.MarkedForDeath.Set(true, smi, false);
				}
				base.sm.Branch.Set(null, this);
			}
		}

		public void SetupRootInformation(SeaTreeRoot.Instance root)
		{
			base.sm.BranchNumber.Set(1, this, false);
			base.sm.WildPlanted.Set(root.IsWild, this, false);
			base.sm.Root.Set(root.gameObject, this, false);
			this.RootSMI = ((this.Root == null) ? null : this.Root.GetSMI<SeaTreeRoot.Instance>());
			HarvestDesignatable component = root.GetComponent<HarvestDesignatable>();
			base.GetComponent<HarvestDesignatable>().SetHarvestWhenReady(component.HarvestWhenReady);
		}

		public void SetupFromPreviousBranchInformation(SeaTreeBranch.Instance previous_branch)
		{
			base.sm.BranchNumber.Set(previous_branch.MyBranchNumber + 1, this, false);
			base.sm.WildPlanted.Set(previous_branch.IsWild, this, false);
			base.sm.Root.Set(previous_branch.Root, this, false);
			this.RootSMI = ((this.Root == null) ? null : this.Root.GetSMI<SeaTreeRoot.Instance>());
			HarvestDesignatable component = previous_branch.GetComponent<HarvestDesignatable>();
			base.GetComponent<HarvestDesignatable>().SetHarvestWhenReady(component.HarvestWhenReady);
		}

		public void AttemptToSpawnBranch()
		{
			if (this.CanSpawnBranch())
			{
				int cellToSpawnBranch = this.GetCellToSpawnBranch();
				GameObject gameObject = this.SpawnBranchOnCell(cellToSpawnBranch);
				base.sm.Branch.Set(gameObject, this, false);
				if (this.IsNewGameSpawned)
				{
					gameObject.Trigger(1119167081, null);
				}
			}
			if (this.IsNewGameSpawned)
			{
				this.IsNewGameSpawned = false;
			}
		}

		private GameObject SpawnBranchOnCell(int cell)
		{
			Vector3 vector = Grid.CellToPosCBC(cell, Grid.SceneLayer.BuildingFront);
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(base.def.BRANCH_PREFAB_NAME), vector);
			gameObject.SetActive(true);
			gameObject.GetSMI<SeaTreeBranch.Instance>().SetupFromPreviousBranchInformation(this);
			return gameObject;
		}

		private bool IsCellAvailable(int cell)
		{
			bool flag = SeaTreeBranch.CanGrowOnCell(base.gameObject, cell);
			if (flag && this.IsNewGameSpawned)
			{
				flag = SaveGame.Instance.worldGenSpawner.GetSpawnableInCell(cell) == null;
			}
			return flag;
		}

		public bool CanSpawnBranch()
		{
			bool flag = this.Branch == null;
			flag = flag && !this.MaxBranchNumberReached;
			flag = flag && this.IsGrown;
			if (flag)
			{
				int cellToSpawnBranch = this.GetCellToSpawnBranch();
				flag = flag && cellToSpawnBranch != Grid.InvalidCell;
				flag = flag && this.IsCellAvailable(cellToSpawnBranch);
			}
			return flag;
		}

		public int GetCellToSpawnBranch()
		{
			return Grid.OffsetCell(Grid.PosToCell(base.gameObject), 0, 1);
		}

		public float TimeUntilNextHarvest()
		{
			float num = ((this.maturity.GetDelta() <= 0f) ? 0f : ((this.maturity.GetMax() - this.maturity.value) / this.maturity.GetDelta()));
			float num2 = ((this.fruitMaturity.GetDelta() <= 0f) ? 0f : ((this.fruitMaturity.GetMax() - this.fruitMaturity.value) / this.fruitMaturity.GetDelta()));
			return num + num2;
		}

		public float GetCurrentGrowthPercentage()
		{
			if (!this.IsGrown)
			{
				return this.GrowthPercentage;
			}
			return this.FruitGrowthPercentage;
		}

		public float PercentGrown()
		{
			return this.GetCurrentGrowthPercentage();
		}

		public Crop GetCropComponent()
		{
			return base.GetComponent<Crop>();
		}

		public float DomesticGrowthTime()
		{
			return this.maturity.GetMax() / this.baseGrowingRate.Value;
		}

		public float WildGrowthTime()
		{
			return this.maturity.GetMax() / this.wildGrowingRate.Value;
		}

		public void OverrideMaturityLevel(float percent)
		{
			float num = this.maturity.GetMax() * percent;
			this.maturity.SetValue(num);
		}

		public bool IsWildPlanted()
		{
			return this.IsWild;
		}

		public string WiltStateString
		{
			get
			{
				return "    • " + DUPLICANTS.STATS.SEATREEROOTHEALTH.NAME;
			}
		}

		public WiltCondition.Condition[] Conditions
		{
			get
			{
				return new WiltCondition.Condition[] { WiltCondition.Condition.UnhealthyRoot };
			}
		}

		public bool IsNewGameSpawned;

		public AttributeModifier baseGrowingRate;

		public AttributeModifier wildGrowingRate;

		public AttributeModifier baseFruitGrowingRate;

		public AttributeModifier wildFruitGrowingRate;

		public AttributeModifier getOldRate;

		public KBatchedAnimController animController;

		private AmountInstance maturity;

		private AmountInstance fruitMaturity;

		private AmountInstance oldAge;

		private WiltCondition wiltCondition;

		private SeaTreeRoot.Instance RootSMI;

		private Harvestable harvestable;

		private MeterController fruitMeter;

		private WorkerBase lastHarvesterWorker;

		private bool wasMarkedForDeadBeforeStartSM;
	}
}
