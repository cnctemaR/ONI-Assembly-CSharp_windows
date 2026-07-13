using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SpaceTreePlant : GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.growing;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.root.EventHandler(GameHashes.OnStorageChange, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.RefreshFullnessVariable));
		this.growing.InitializeStates(this.masterTarget, this.dead).DefaultState(this.growing.idle);
		this.growing.idle.EventTransition(GameHashes.Grow, this.growing.complete, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Transition.ConditionCallback(SpaceTreePlant.IsTrunkMature)).EventTransition(GameHashes.Wilt, this.growing.wilted, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Transition.ConditionCallback(SpaceTreePlant.IsTrunkWilted)).PlayAnim((SpaceTreePlant.Instance smi) => "grow", KAnim.PlayMode.Paused)
			.Enter(new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.RefreshGrowingAnimation))
			.Update(new Action<SpaceTreePlant.Instance, float>(SpaceTreePlant.RefreshGrowingAnimationUpdate), UpdateRate.SIM_4000ms, false);
		this.growing.complete.EnterTransition(this.production, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Transition.ConditionCallback(SpaceTreePlant.TrunkHasAtLeastOneBranch)).PlayAnim("grow_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.production);
		this.growing.wilted.EventTransition(GameHashes.WiltRecover, this.growing.idle, GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Not(new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Transition.ConditionCallback(SpaceTreePlant.IsTrunkWilted))).PlayAnim(new Func<SpaceTreePlant.Instance, string>(SpaceTreePlant.GetGrowingStatesWiltedAnim), KAnim.PlayMode.Loop);
		this.production.InitializeStates(this.masterTarget, this.dead).EventTransition(GameHashes.Grow, this.growing, GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Not(new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Transition.ConditionCallback(SpaceTreePlant.IsTrunkMature))).ParamTransition<bool>(this.ReadyForHarvest, this.harvest, GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.IsTrue)
			.ParamTransition<float>(this.Fullness, this.harvest, GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.IsGTEOne)
			.DefaultState(this.production.producing);
		this.production.producing.EventTransition(GameHashes.Wilt, this.production.wilted, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Transition.ConditionCallback(SpaceTreePlant.IsTrunkWilted)).OnSignal(this.BranchWiltConditionChanged, this.production.halted, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Parameter<StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.SignalParameter>.Callback(SpaceTreePlant.CanNOTProduce)).OnSignal(this.BranchGrownStatusChanged, this.production.halted, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Parameter<StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.SignalParameter>.Callback(SpaceTreePlant.CanNOTProduce))
			.Enter(new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.RefreshFullnessAnimation))
			.EventHandler(GameHashes.OnStorageChange, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.RefreshFullnessAnimation))
			.ToggleStatusItem(Db.Get().CreatureStatusItems.ProducingSugarWater, null)
			.Update(new Action<SpaceTreePlant.Instance, float>(SpaceTreePlant.ProductionUpdate), UpdateRate.SIM_200ms, false);
		this.production.halted.EventTransition(GameHashes.Wilt, this.production.wilted, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Transition.ConditionCallback(SpaceTreePlant.IsTrunkWilted)).EventTransition(GameHashes.TreeBranchCountChanged, this.production.producing, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Transition.ConditionCallback(SpaceTreePlant.CanProduce)).OnSignal(this.BranchWiltConditionChanged, this.production.producing, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Parameter<StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.SignalParameter>.Callback(SpaceTreePlant.CanProduce))
			.OnSignal(this.BranchGrownStatusChanged, this.production.producing, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Parameter<StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.SignalParameter>.Callback(SpaceTreePlant.CanProduce))
			.ToggleStatusItem(Db.Get().CreatureStatusItems.SugarWaterProductionPaused, null)
			.Enter(new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.RefreshFullnessAnimation));
		this.production.wilted.EventTransition(GameHashes.WiltRecover, this.production.producing, GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Not(new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Transition.ConditionCallback(SpaceTreePlant.IsTrunkWilted))).ToggleStatusItem(Db.Get().CreatureStatusItems.SugarWaterProductionWilted, null).PlayAnim("idle_empty", KAnim.PlayMode.Once)
			.EventHandler(GameHashes.EntombDefenseReactionBegins, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.InformBranchesTrunkWantsToBreakFree));
		this.harvest.InitializeStates(this.masterTarget, this.dead).EventTransition(GameHashes.Grow, this.growing, GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Not(new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Transition.ConditionCallback(SpaceTreePlant.IsTrunkMature))).ParamTransition<float>(this.Fullness, this.harvestCompleted, GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.IsLTOne)
			.EventHandler(GameHashes.EntombDefenseReactionBegins, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.InformBranchesTrunkWantsToBreakFree))
			.ToggleStatusItem(Db.Get().CreatureStatusItems.ReadyForHarvest, null)
			.Enter(new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.SetReadyToHarvest))
			.Enter(new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.EnablePiping))
			.DefaultState(this.harvest.prevented);
		this.harvest.prevented.PlayAnim("harvest_ready", KAnim.PlayMode.Loop).Toggle("ToggleReadyForHarvest", new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.AddHarvestReadyTag), new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.RemoveHarvestReadyTag)).Toggle("SetTag_ReadyForHarvest_OnNewBanches", new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.SubscribeToUpdateNewBranchesReadyForHarvest), new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.UnsubscribeToUpdateNewBranchesReadyForHarvest))
			.EventHandler(GameHashes.EntombedChanged, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.PlayHarvestReadyOnUntentombed))
			.EventTransition(GameHashes.HarvestDesignationChanged, this.harvest.manualHarvest, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Transition.ConditionCallback(SpaceTreePlant.CanBeManuallyHarvested))
			.EventTransition(GameHashes.ConduitConnectionChanged, this.harvest.pipes, (SpaceTreePlant.Instance smi) => SpaceTreePlant.HasPipeConnected(smi) && smi.IsPipingEnabled)
			.ParamTransition<bool>(this.PipingEnabled, this.harvest.pipes, (SpaceTreePlant.Instance smi, bool pipeEnable) => pipeEnable && SpaceTreePlant.HasPipeConnected(smi));
		this.harvest.manualHarvest.DefaultState(this.harvest.manualHarvest.awaitingForFarmer).Toggle("ToggleReadyForHarvest", new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.AddHarvestReadyTag), new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.RemoveHarvestReadyTag)).Toggle("SetTag_ReadyForHarvest_OnNewBanches", new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.SubscribeToUpdateNewBranchesReadyForHarvest), new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.UnsubscribeToUpdateNewBranchesReadyForHarvest))
			.Enter(new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.ShowSkillRequiredStatusItemIfSkillMissing))
			.Enter(new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.StartHarvestWorkChore))
			.EventHandler(GameHashes.EntombedChanged, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.PlayHarvestReadyOnUntentombed))
			.EventTransition(GameHashes.HarvestDesignationChanged, this.harvest.prevented, GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Not(new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Transition.ConditionCallback(SpaceTreePlant.CanBeManuallyHarvested)))
			.EventTransition(GameHashes.ConduitConnectionChanged, this.harvest.pipes, (SpaceTreePlant.Instance smi) => SpaceTreePlant.HasPipeConnected(smi) && smi.IsPipingEnabled)
			.ParamTransition<bool>(this.PipingEnabled, this.harvest.pipes, (SpaceTreePlant.Instance smi, bool pipeEnable) => pipeEnable && SpaceTreePlant.HasPipeConnected(smi))
			.WorkableCompleteTransition(new Func<SpaceTreePlant.Instance, Workable>(this.GetWorkable), this.harvest.farmerWorkCompleted)
			.Exit(new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.CancelHarvestWorkChore))
			.Exit(new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.HideSkillRequiredStatusItemIfSkillMissing));
		this.harvest.manualHarvest.awaitingForFarmer.PlayAnim("harvest_ready", KAnim.PlayMode.Loop).WorkableStartTransition(new Func<SpaceTreePlant.Instance, Workable>(this.GetWorkable), this.harvest.manualHarvest.farmerWorking);
		this.harvest.manualHarvest.farmerWorking.WorkableStopTransition(new Func<SpaceTreePlant.Instance, Workable>(this.GetWorkable), this.harvest.manualHarvest.awaitingForFarmer);
		this.harvest.farmerWorkCompleted.Enter(new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.DropInventory));
		this.harvest.pipes.Enter(new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.RefreshFullnessAnimation)).Toggle("ToggleReadyForHarvest", new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.AddHarvestReadyTag), new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.RemoveHarvestReadyTag)).Toggle("SetTag_ReadyForHarvest_OnNewBanches", new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.SubscribeToUpdateNewBranchesReadyForHarvest), new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.UnsubscribeToUpdateNewBranchesReadyForHarvest))
			.PlayAnim("harvest_ready", KAnim.PlayMode.Loop)
			.EventHandler(GameHashes.EntombedChanged, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.RefreshOnPipesHarvestAnimations))
			.EventHandler(GameHashes.OnStorageChange, new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.RefreshFullnessAnimation))
			.EventTransition(GameHashes.ConduitConnectionChanged, this.harvest.prevented, (SpaceTreePlant.Instance smi) => !smi.IsPipingEnabled || !SpaceTreePlant.HasPipeConnected(smi))
			.ParamTransition<bool>(this.PipingEnabled, this.harvest.prevented, (SpaceTreePlant.Instance smi, bool pipeEnable) => !pipeEnable || !SpaceTreePlant.HasPipeConnected(smi));
		this.harvestCompleted.Enter(new StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State.Callback(SpaceTreePlant.UnsetReadyToHarvest)).GoTo(this.production);
		this.dead.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead, null).Enter(delegate(SpaceTreePlant.Instance smi)
		{
			if (!smi.IsWildPlanted && !smi.GetComponent<KPrefabID>().HasTag(GameTags.Uprooted))
			{
				Notifier notifier = smi.gameObject.AddOrGet<Notifier>();
				Notification notification = SpaceTreePlant.CreateDeathNotification(smi);
				notifier.Add(notification, "");
			}
			GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
			smi.Trigger(1623392196, null);
			smi.GetComponent<KBatchedAnimController>().StopAndClear();
			global::UnityEngine.Object.Destroy(smi.GetComponent<KBatchedAnimController>());
		}).ScheduleAction("Delayed Destroy", 0.5f, new Action<SpaceTreePlant.Instance>(SpaceTreePlant.SelfDestroy));
	}

	public Workable GetWorkable(SpaceTreePlant.Instance smi)
	{
		return smi.GetWorkable();
	}

	public static void EnablePiping(SpaceTreePlant.Instance smi)
	{
		smi.SetPipingState(true);
	}

	public static void InformBranchesTrunkWantsToBreakFree(SpaceTreePlant.Instance smi)
	{
		smi.InformBranchesTrunkWantsToUnentomb();
	}

	public static void UnsubscribeToUpdateNewBranchesReadyForHarvest(SpaceTreePlant.Instance smi)
	{
		smi.UnsubscribeToUpdateNewBranchesReadyForHarvest();
	}

	public static void SubscribeToUpdateNewBranchesReadyForHarvest(SpaceTreePlant.Instance smi)
	{
		smi.SubscribeToUpdateNewBranchesReadyForHarvest();
	}

	public static void RefreshFullnessVariable(SpaceTreePlant.Instance smi)
	{
		smi.RefreshFullnessVariable();
	}

	public static void ShowSkillRequiredStatusItemIfSkillMissing(SpaceTreePlant.Instance smi)
	{
		smi.GetWorkable().SetShouldShowSkillPerkStatusItem(true);
	}

	public static void HideSkillRequiredStatusItemIfSkillMissing(SpaceTreePlant.Instance smi)
	{
		smi.GetWorkable().SetShouldShowSkillPerkStatusItem(false);
	}

	public static void StartHarvestWorkChore(SpaceTreePlant.Instance smi)
	{
		smi.CreateHarvestChore();
	}

	public static void CancelHarvestWorkChore(SpaceTreePlant.Instance smi)
	{
		smi.CancelHarvestChore();
	}

	public static bool HasPipeConnected(SpaceTreePlant.Instance smi)
	{
		return smi.HasPipeConnected;
	}

	public static bool CanBeManuallyHarvested(SpaceTreePlant.Instance smi)
	{
		return smi.CanBeManuallyHarvested;
	}

	public static void SetReadyToHarvest(SpaceTreePlant.Instance smi)
	{
		smi.sm.ReadyForHarvest.Set(true, smi, false);
	}

	public static void UnsetReadyToHarvest(SpaceTreePlant.Instance smi)
	{
		smi.sm.ReadyForHarvest.Set(false, smi, false);
	}

	public static void RefreshOnPipesHarvestAnimations(SpaceTreePlant.Instance smi)
	{
		if (smi.IsReadyForHarvest)
		{
			SpaceTreePlant.PlayHarvestReadyOnUntentombed(smi);
			return;
		}
		SpaceTreePlant.RefreshFullnessAnimation(smi);
	}

	public static void RefreshFullnessAnimation(SpaceTreePlant.Instance smi)
	{
		smi.RefreshFullnessTreeTrunkAnimation();
	}

	public static void ProductionUpdate(SpaceTreePlant.Instance smi, float dt)
	{
		smi.ProduceUpdate(dt);
	}

	public static void DropInventory(SpaceTreePlant.Instance smi)
	{
		smi.DropInventory();
	}

	public static void AddHarvestReadyTag(SpaceTreePlant.Instance smi)
	{
		smi.SetReadyForHarvestTag(true);
	}

	public static void RemoveHarvestReadyTag(SpaceTreePlant.Instance smi)
	{
		smi.SetReadyForHarvestTag(false);
	}

	public static string GetGrowingStatesWiltedAnim(SpaceTreePlant.Instance smi)
	{
		return smi.GetTrunkWiltAnimation();
	}

	public static void RefreshGrowingAnimation(SpaceTreePlant.Instance smi)
	{
		smi.RefreshGrowingAnimation();
	}

	public static void RefreshGrowingAnimationUpdate(SpaceTreePlant.Instance smi, float dt)
	{
		smi.RefreshGrowingAnimation();
	}

	public static bool TrunkHasAtLeastOneBranch(SpaceTreePlant.Instance smi)
	{
		return smi.HasAtLeastOneBranch;
	}

	public static bool IsTrunkMature(SpaceTreePlant.Instance smi)
	{
		return smi.IsMature;
	}

	public static bool IsTrunkWilted(SpaceTreePlant.Instance smi)
	{
		return smi.IsWilting;
	}

	public static bool CanNOTProduce(SpaceTreePlant.Instance smi, StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.SignalParameter param)
	{
		return SpaceTreePlant.CanNOTProduce(smi);
	}

	public static bool CanNOTProduce(SpaceTreePlant.Instance smi)
	{
		return !SpaceTreePlant.CanProduce(smi);
	}

	public static void PlayHarvestReadyOnUntentombed(SpaceTreePlant.Instance smi)
	{
		if (!smi.IsEntombed)
		{
			smi.PlayHarvestReadyAnimation();
		}
	}

	public static void SelfDestroy(SpaceTreePlant.Instance smi)
	{
		Util.KDestroyGameObject(smi.gameObject);
	}

	public static bool CanProduce(SpaceTreePlant.Instance smi, StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.SignalParameter param)
	{
		return SpaceTreePlant.CanProduce(smi);
	}

	public static bool CanProduce(SpaceTreePlant.Instance smi)
	{
		return !smi.IsUprooted && !smi.IsWilting && smi.IsMature && !smi.IsReadyForHarvest && smi.HasAtLeastOneHealthyFullyGrownBranch();
	}

	public static Notification CreateDeathNotification(SpaceTreePlant.Instance smi)
	{
		return new Notification(CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION, NotificationType.Bad, (List<Notification> notificationList, object data) => CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), "/t• " + smi.gameObject.GetProperName(), true, 0f, null, null, null, true, false, false);
	}

	public const float WILD_PLANTED_SUGAR_WATER_PRODUCTION_SPEED_MODIFIER = 4f;

	public static Tag SpaceTreeReadyForHarvest = TagManager.Create("SpaceTreeReadyForHarvest");

	public const string GROWN_WILT_ANIM_NAME = "idle_empty";

	public const string WILT_ANIM_NAME = "wilt";

	public const string GROW_ANIM_NAME = "grow";

	public const string GROW_PST_ANIM_NAME = "grow_pst";

	public const string FILL_ANIM_NAME = "grow_fill";

	public const string MANUAL_HARVEST_READY_ANIM_NAME = "harvest_ready";

	private const int FILLING_ANIMATION_FRAME_COUNT = 42;

	private const int WILT_LEVELS = 3;

	private const float PIPING_ENABLE_TRESHOLD = 0.25f;

	public const SimHashes ProductElement = SimHashes.SugarWater;

	public SpaceTreePlant.GrowingState growing;

	public SpaceTreePlant.ProductionStates production;

	public SpaceTreePlant.HarvestStates harvest;

	public GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State harvestCompleted;

	public GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State dead;

	public StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.BoolParameter ReadyForHarvest;

	public StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.BoolParameter PipingEnabled;

	public StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.FloatParameter Fullness;

	public StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Signal BranchWiltConditionChanged;

	public StateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.Signal BranchGrownStatusChanged;

	public class Def : StateMachine.BaseDef
	{
		public int OptimalAmountOfBranches;

		public float OptimalProductionDuration;
	}

	public class GrowingState : GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.PlantAliveSubState
	{
		public GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State idle;

		public GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State complete;

		public GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State wilted;
	}

	public class ProductionStates : GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.PlantAliveSubState
	{
		public GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State wilted;

		public GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State halted;

		public GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State producing;
	}

	public class HarvestStates : GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.PlantAliveSubState
	{
		public GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State wilted;

		public GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State prevented;

		public SpaceTreePlant.ManualHarvestStates manualHarvest;

		public GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State farmerWorkCompleted;

		public GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State pipes;
	}

	public class ManualHarvestStates : GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State
	{
		public GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State awaitingForFarmer;

		public GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.State farmerWorking;
	}

	public new class Instance : GameStateMachine<SpaceTreePlant, SpaceTreePlant.Instance, IStateMachineTarget, SpaceTreePlant.Def>.GameInstance
	{
		public float OptimalProductionDuration
		{
			get
			{
				if (!this.IsWildPlanted)
				{
					return base.def.OptimalProductionDuration;
				}
				return base.def.OptimalProductionDuration * 4f;
			}
		}

		public float CurrentProductionProgress
		{
			get
			{
				return base.sm.Fullness.Get(this);
			}
		}

		public bool IsWilting
		{
			get
			{
				return base.gameObject.HasTag(GameTags.Wilting);
			}
		}

		public bool IsMature
		{
			get
			{
				return this.growingComponent.IsGrown();
			}
		}

		public bool HasAtLeastOneBranch
		{
			get
			{
				return this.BranchCount > 0;
			}
		}

		public bool IsReadyForHarvest
		{
			get
			{
				return base.sm.ReadyForHarvest.Get(base.smi);
			}
		}

		public bool CanBeManuallyHarvested
		{
			get
			{
				return this.UserAllowsHarvest && !this.HasPipeConnected;
			}
		}

		public bool UserAllowsHarvest
		{
			get
			{
				return this.harvestDesignatable == null || (this.harvestDesignatable.HarvestWhenReady && this.harvestDesignatable.MarkedForHarvest);
			}
		}

		public bool HasPipeConnected
		{
			get
			{
				return this.conduitDispenser.IsConnected;
			}
		}

		public bool IsUprooted
		{
			get
			{
				return this.uprootMonitor != null && this.uprootMonitor.IsUprooted;
			}
		}

		public bool IsWildPlanted
		{
			get
			{
				return !this.receptacleMonitor.Replanted;
			}
		}

		public bool IsEntombed
		{
			get
			{
				return this.entombDefenseSMI != null && this.entombDefenseSMI.IsEntombed;
			}
		}

		public bool IsPipingEnabled
		{
			get
			{
				return base.sm.PipingEnabled.Get(this);
			}
		}

		public int BranchCount
		{
			get
			{
				if (this.tree != null)
				{
					return this.tree.CurrentBranchCount;
				}
				return 0;
			}
		}

		public Workable GetWorkable()
		{
			return this.workable;
		}

		public Instance(IStateMachineTarget master, SpaceTreePlant.Def def)
			: base(master, def)
		{
		}

		public override void StartSM()
		{
			this.tree = base.gameObject.GetSMI<PlantBranchGrower.Instance>();
			this.tree.ActionPerBranch(new Action<GameObject>(this.SubscribeToBranchCallbacks));
			this.tree.Subscribe(-1586842875, new Action<object>(this.SubscribeToNewBranches));
			this.entombDefenseSMI = base.smi.GetSMI<UnstableEntombDefense.Instance>();
			base.StartSM();
			this.SetPipingState(this.IsPipingEnabled);
			this.RefreshFullnessVariable();
			SpaceTreeSyrupHarvestWorkable spaceTreeSyrupHarvestWorkable = this.workable;
			spaceTreeSyrupHarvestWorkable.OnWorkableEventCB = (Action<Workable, Workable.WorkableEvent>)Delegate.Combine(spaceTreeSyrupHarvestWorkable.OnWorkableEventCB, new Action<Workable, Workable.WorkableEvent>(this.OnManualHarvestWorkableStateChanges));
		}

		private void OnManualHarvestWorkableStateChanges(Workable workable, Workable.WorkableEvent workableEvent)
		{
			if (workableEvent == Workable.WorkableEvent.WorkStarted)
			{
				this.InformBranchesTrunkIsBeingHarvestedManually();
				return;
			}
			if (workableEvent == Workable.WorkableEvent.WorkStopped)
			{
				this.InformBranchesTrunkIsNoLongerBeingHarvestedManually();
			}
		}

		private void SubscribeToNewBranches(object obj)
		{
			if (obj == null)
			{
				return;
			}
			PlantBranch.Instance instance = (PlantBranch.Instance)obj;
			this.SubscribeToBranchCallbacks(instance.gameObject);
		}

		private void SubscribeToBranchCallbacks(GameObject branch)
		{
			branch.Subscribe(-724860998, new Action<object>(this.OnBranchWiltStateChanged));
			branch.Subscribe(712767498, new Action<object>(this.OnBranchWiltStateChanged));
			branch.Subscribe(-254803949, new Action<object>(this.OnBranchGrowStatusChanged));
		}

		private void OnBranchGrowStatusChanged(object obj)
		{
			base.sm.BranchGrownStatusChanged.Trigger(this);
		}

		private void OnBranchWiltStateChanged(object obj)
		{
			base.sm.BranchWiltConditionChanged.Trigger(this);
		}

		public void SubscribeToUpdateNewBranchesReadyForHarvest()
		{
			this.onNewBranchesReadyHandle = this.tree.Subscribe(-1586842875, new Action<object>(this.OnNewBranchSpawnedWhileTreeIsReadyForHarvest));
		}

		public void UnsubscribeToUpdateNewBranchesReadyForHarvest()
		{
			this.tree.Unsubscribe(ref this.onNewBranchesReadyHandle);
		}

		private void OnNewBranchSpawnedWhileTreeIsReadyForHarvest(object data)
		{
			if (data == null)
			{
				return;
			}
			((PlantBranch.Instance)data).gameObject.AddTag(SpaceTreePlant.SpaceTreeReadyForHarvest);
		}

		public void SetPipingState(bool enable)
		{
			base.sm.PipingEnabled.Set(enable, this, false);
			this.SetConduitDispenserAbilityToDispense(enable);
		}

		private void SetConduitDispenserAbilityToDispense(bool canDispense)
		{
			this.conduitDispenser.SetOnState(canDispense);
		}

		public void SetReadyForHarvestTag(bool isReady)
		{
			if (isReady)
			{
				base.gameObject.AddTag(SpaceTreePlant.SpaceTreeReadyForHarvest);
				if (this.tree == null)
				{
					return;
				}
				this.tree.ActionPerBranch(delegate(GameObject branch)
				{
					branch.AddTag(SpaceTreePlant.SpaceTreeReadyForHarvest);
				});
				return;
			}
			else
			{
				base.gameObject.RemoveTag(SpaceTreePlant.SpaceTreeReadyForHarvest);
				if (this.tree == null)
				{
					return;
				}
				this.tree.ActionPerBranch(delegate(GameObject branch)
				{
					branch.RemoveTag(SpaceTreePlant.SpaceTreeReadyForHarvest);
				});
				return;
			}
		}

		public bool HasAtLeastOneHealthyFullyGrownBranch()
		{
			if (this.tree == null || this.BranchCount <= 0)
			{
				return false;
			}
			bool healthyGrownBranchFound = false;
			this.tree.ActionPerBranch(delegate(GameObject branch)
			{
				SpaceTreeBranch.Instance smi = branch.GetSMI<SpaceTreeBranch.Instance>();
				if (smi != null && !smi.isMasterNull)
				{
					healthyGrownBranchFound = healthyGrownBranchFound || (smi.IsBranchFullyGrown && !smi.wiltCondition.IsWilting());
				}
			});
			return healthyGrownBranchFound;
		}

		public void CreateHarvestChore()
		{
			if (this.harvestChore == null)
			{
				this.harvestChore = new WorkChore<SpaceTreeSyrupHarvestWorkable>(Db.Get().ChoreTypes.Harvest, this.workable, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			}
		}

		public void CancelHarvestChore()
		{
			if (this.harvestChore != null)
			{
				this.harvestChore.Cancel("SpaceTreeSyrupProduction.CancelHarvestChore()");
				this.harvestChore = null;
			}
		}

		public void ProduceUpdate(float dt)
		{
			float num = Mathf.Min(dt / base.smi.OptimalProductionDuration * base.smi.GetProductionSpeed() * this.storage.capacityKg, this.storage.RemainingCapacity());
			float lowTemp = ElementLoader.GetElement(SimHashes.SugarWater.CreateTag()).lowTemp;
			float num2 = 8f;
			float num3 = Mathf.Max(this.pe.Temperature, lowTemp + num2);
			this.storage.AddLiquid(SimHashes.SugarWater, num, num3, byte.MaxValue, 0, false, true);
		}

		public void DropInventory()
		{
			List<GameObject> list = new List<GameObject>();
			Storage storage = this.storage;
			bool flag = false;
			bool flag2 = false;
			List<GameObject> list2 = list;
			storage.DropAll(flag, flag2, default(Vector3), true, list2);
			foreach (GameObject gameObject in list)
			{
				Vector3 position = gameObject.transform.position;
				position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
				gameObject.transform.SetPosition(position);
			}
		}

		public void PlayHarvestReadyAnimation()
		{
			if (this.animController != null)
			{
				this.animController.Play("harvest_ready", KAnim.PlayMode.Loop, 1f, 0f);
			}
		}

		public void InformBranchesTrunkIsBeingHarvestedManually()
		{
			this.tree.ActionPerBranch(delegate(GameObject branch)
			{
				branch.Trigger(2137182770, null);
			});
		}

		public void InformBranchesTrunkIsNoLongerBeingHarvestedManually()
		{
			this.tree.ActionPerBranch(delegate(GameObject branch)
			{
				branch.Trigger(-808006162, null);
			});
		}

		public void InformBranchesTrunkWantsToUnentomb()
		{
			this.tree.ActionPerBranch(delegate(GameObject branch)
			{
				branch.Trigger(570354093, null);
			});
		}

		public void RefreshFullnessVariable()
		{
			float num = this.storage.MassStored() / this.storage.capacityKg;
			base.sm.Fullness.Set(num, this, false);
			Boxed<float> boxed = Boxed<float>.Get(num);
			for (int i = 0; i < this.tree.CurrentBranchCount; i++)
			{
				GameObject branch = this.tree.GetBranch(i);
				if (branch != null)
				{
					branch.Trigger(-824970674, boxed);
				}
			}
			Boxed<float>.Release(boxed);
			if (num < 0.25f)
			{
				this.SetPipingState(false);
			}
		}

		public float GetProductionSpeed()
		{
			if (this.tree == null)
			{
				return 0f;
			}
			float totalProduction = 0f;
			this.tree.ActionPerBranch(delegate(GameObject branch)
			{
				SpaceTreeBranch.Instance smi = branch.GetSMI<SpaceTreeBranch.Instance>();
				if (smi != null && !smi.isMasterNull)
				{
					totalProduction += smi.Productivity;
				}
			});
			return totalProduction / (float)base.def.OptimalAmountOfBranches;
		}

		public string GetTrunkWiltAnimation()
		{
			int num = Mathf.Clamp(Mathf.FloorToInt(this.growing.PercentOfCurrentHarvest() / 0.33333334f), 0, 2);
			return "wilt" + (num + 1).ToString();
		}

		public void RefreshFullnessTreeTrunkAnimation()
		{
			int num = Mathf.FloorToInt(this.CurrentProductionProgress * 42f);
			if (this.animController.currentAnim != "grow_fill")
			{
				this.animController.Play("grow_fill", KAnim.PlayMode.Paused, 1f, 0f);
				this.animController.SetPositionPercent(this.CurrentProductionProgress);
				this.animController.enabled = false;
				this.animController.enabled = true;
				return;
			}
			if (this.animController.currentFrame != num)
			{
				this.animController.SetPositionPercent(this.CurrentProductionProgress);
			}
		}

		public void RefreshGrowingAnimation()
		{
			this.animController.SetPositionPercent(this.growing.PercentOfCurrentHarvest());
		}

		[MyCmpReq]
		private ReceptacleMonitor receptacleMonitor;

		[MyCmpReq]
		private KBatchedAnimController animController;

		[MyCmpReq]
		private Growing growingComponent;

		[MyCmpReq]
		private ConduitDispenser conduitDispenser;

		[MyCmpReq]
		private Storage storage;

		[MyCmpReq]
		private SpaceTreeSyrupHarvestWorkable workable;

		[MyCmpGet]
		private PrimaryElement pe;

		[MyCmpGet]
		private HarvestDesignatable harvestDesignatable;

		[MyCmpGet]
		private UprootedMonitor uprootMonitor;

		[MyCmpGet]
		private Growing growing;

		private PlantBranchGrower.Instance tree;

		private UnstableEntombDefense.Instance entombDefenseSMI;

		private Chore harvestChore;

		private int onNewBranchesReadyHandle = -1;
	}
}
