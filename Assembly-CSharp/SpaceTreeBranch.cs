using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class SpaceTreeBranch : GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.growing;
		this.root.EventTransition(GameHashes.Uprooted, this.die, null).EventHandler(GameHashes.Wilt, new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.UpdateFlowerOnWilt)).EventHandler(GameHashes.WiltRecover, new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.UpdateFlowerOnWiltRecover));
		this.growing.InitializeStates(this.masterTarget, this.die).EnterTransition(this.grown, new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Transition.ConditionCallback(SpaceTreeBranch.IsBranchFullyGrown)).Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.DisableEntombDefenses))
			.Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.DisableGlowFlowerMeter))
			.Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.ForbidBranchToBeHarvestedForWood))
			.EventTransition(GameHashes.Wilt, this.halt, new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Transition.ConditionCallback(SpaceTreeBranch.IsWiltedConditionReportingWilted))
			.EventTransition(GameHashes.RootHealthChanged, this.halt, GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Not(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Transition.ConditionCallback(SpaceTreeBranch.IsTrunkHealthy)))
			.EventTransition(GameHashes.PlanterStorage, this.growing.planted, GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Not(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Transition.ConditionCallback(SpaceTreeBranch.IsTrunkWildPlanted)))
			.EventTransition(GameHashes.PlanterStorage, this.growing.wild, new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Transition.ConditionCallback(SpaceTreeBranch.IsTrunkWildPlanted))
			.ToggleStatusItem(Db.Get().CreatureStatusItems.Growing, null)
			.Update("CheckGrown", delegate(SpaceTreeBranch.Instance smi, float dt)
			{
				if (smi.GetcurrentGrowthPercentage() >= 1f)
				{
					smi.gameObject.Trigger(-254803949, null);
					smi.GoTo(this.grown);
				}
			}, UpdateRate.SIM_4000ms, false);
		this.growing.wild.DefaultState(this.growing.wild.visible).EnterTransition(this.growing.planted, GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Not(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Transition.ConditionCallback(SpaceTreeBranch.IsTrunkWildPlanted))).ToggleAttributeModifier("GrowingWild", (SpaceTreeBranch.Instance smi) => smi.wildGrowingRate, null);
		this.growing.wild.visible.PlayAnim((SpaceTreeBranch.Instance smi) => smi.GetAnimationForState(this.growing.wild.visible), KAnim.PlayMode.Paused).EventHandler(GameHashes.SpaceTreeInternalSyrupChanged, new GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.GameEvent.Callback(SpaceTreeBranch.OnTrunkSyrupFullnessChanged)).TagTransition(SpaceTreePlant.SpaceTreeReadyForHarvest, this.growing.wild.hidden, false)
			.Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.PlayFillAnimationForThisState));
		this.growing.wild.hidden.TagTransition(SpaceTreePlant.SpaceTreeReadyForHarvest, this.growing.wild.visible, true).PlayAnim((SpaceTreeBranch.Instance smi) => smi.GetAnimationForState(this.growing.wild.hidden), KAnim.PlayMode.Once);
		this.growing.planted.DefaultState(this.growing.planted.visible).EnterTransition(this.growing.wild, new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Transition.ConditionCallback(SpaceTreeBranch.IsTrunkWildPlanted)).ToggleAttributeModifier("Growing", (SpaceTreeBranch.Instance smi) => smi.baseGrowingRate, null);
		this.growing.planted.visible.PlayAnim((SpaceTreeBranch.Instance smi) => smi.GetAnimationForState(this.growing.planted.visible), KAnim.PlayMode.Paused).EventHandler(GameHashes.SpaceTreeInternalSyrupChanged, new GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.GameEvent.Callback(SpaceTreeBranch.OnTrunkSyrupFullnessChanged)).TagTransition(SpaceTreePlant.SpaceTreeReadyForHarvest, this.growing.planted.hidden, false)
			.Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.PlayFillAnimationForThisState));
		this.growing.planted.hidden.TagTransition(SpaceTreePlant.SpaceTreeReadyForHarvest, this.growing.planted.visible, true).PlayAnim((SpaceTreeBranch.Instance smi) => smi.GetAnimationForState(this.growing.planted.hidden), KAnim.PlayMode.Once);
		this.halt.InitializeStates(this.masterTarget, this.die).DefaultState(this.halt.wilted).EventHandlerTransition(GameHashes.RootHealthChanged, this.growing, (SpaceTreeBranch.Instance smi, object o) => SpaceTreeBranch.IsTrunkHealthy(smi) && !SpaceTreeBranch.IsWiltedConditionReportingWilted(smi))
			.EventTransition(GameHashes.WiltRecover, this.growing, null)
			.Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.DisableEntombDefenses))
			.TagTransition(SpaceTreePlant.SpaceTreeReadyForHarvest, this.halt.hidden, false);
		this.halt.wilted.PlayAnim((SpaceTreeBranch.Instance smi) => smi.GetAnimationForState(this.halt.wilted), KAnim.PlayMode.Paused).EventTransition(GameHashes.RootHealthChanged, this.halt.trunkWilted, GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Not(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Transition.ConditionCallback(SpaceTreeBranch.IsTrunkHealthy))).EventHandler(GameHashes.SpaceTreeInternalSyrupChanged, new GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.GameEvent.Callback(SpaceTreeBranch.OnTrunkSyrupFullnessChanged))
			.Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.PlayFillAnimationForThisState))
			.EventHandlerTransition(GameHashes.SpaceTreeUnentombDefenseTriggered, this.halt.shaking, (SpaceTreeBranch.Instance o, object smi) => true);
		this.halt.trunkWilted.EventTransition(GameHashes.RootHealthChanged, this.halt.wilted, new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Transition.ConditionCallback(SpaceTreeBranch.IsTrunkHealthy)).PlayAnim((SpaceTreeBranch.Instance smi) => smi.GetAnimationForState(this.halt.trunkWilted), KAnim.PlayMode.Once).EventHandlerTransition(GameHashes.SpaceTreeUnentombDefenseTriggered, this.halt.shaking, (SpaceTreeBranch.Instance o, object smi) => true);
		this.halt.shaking.PlayAnim((SpaceTreeBranch.Instance smi) => smi.GetAnimationForState(this.halt.shaking), KAnim.PlayMode.Once).ScheduleGoTo(1.8f, this.halt.wilted);
		this.halt.hidden.PlayAnim((SpaceTreeBranch.Instance smi) => smi.GetAnimationForState(this.halt.hidden), KAnim.PlayMode.Once).TagTransition(SpaceTreePlant.SpaceTreeReadyForHarvest, this.halt.wilted, true);
		this.grown.InitializeStates(this.masterTarget, this.die).Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.EnableEntombDefenses)).Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.AllowItToBeHarvestForWood))
			.EventTransition(GameHashes.Harvest, this.harvestedForWood, null)
			.EventTransition(GameHashes.ConsumePlant, this.growing, GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Not(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Transition.ConditionCallback(SpaceTreeBranch.IsBranchFullyGrown)))
			.DefaultState(this.grown.spawn);
		this.grown.spawn.EventTransition(GameHashes.Wilt, this.grown.trunkWilted, GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Not(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Transition.ConditionCallback(SpaceTreeBranch.IsTrunkHealthy))).EventTransition(GameHashes.RootHealthChanged, this.grown.trunkWilted, GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Not(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Transition.ConditionCallback(SpaceTreeBranch.IsTrunkHealthy))).ParamTransition<bool>(this.HasSpawn, this.grown.healthy, GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.IsTrue)
			.Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.DisableGlowFlowerMeter))
			.PlayAnim((SpaceTreeBranch.Instance smi) => smi.GetAnimationForState(this.grown.spawn), KAnim.PlayMode.Once)
			.OnAnimQueueComplete(this.grown.spawnPST);
		this.grown.spawnPST.PlayAnim((SpaceTreeBranch.Instance smi) => smi.GetAnimationForState(this.grown.spawnPST), KAnim.PlayMode.Once).OnAnimQueueComplete(this.grown.healthy);
		this.grown.healthy.Enter(delegate(SpaceTreeBranch.Instance smi)
		{
			this.HasSpawn.Set(true, smi, false);
		}).Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.PlayFillAnimationForThisState)).Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.EnableGlowFlowerMeter))
			.Exit(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.DisableGlowFlowerMeter))
			.ToggleStatusItem(Db.Get().CreatureStatusItems.SpaceTreeBranchLightStatus, null)
			.DefaultState(this.grown.healthy.filling);
		this.grown.healthy.filling.EventHandler(GameHashes.EntombedChanged, new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.PlayFillAnimationOnUnentomb)).EventHandler(GameHashes.SpaceTreeInternalSyrupChanged, new GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.GameEvent.Callback(SpaceTreeBranch.OnTrunkSyrupFullnessChanged)).EventTransition(GameHashes.Wilt, this.grown.trunkWilted, GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Not(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Transition.ConditionCallback(SpaceTreeBranch.IsTrunkHealthy)))
			.EventTransition(GameHashes.RootHealthChanged, this.grown.trunkWilted, GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Not(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Transition.ConditionCallback(SpaceTreeBranch.IsTrunkHealthy)))
			.TagTransition(SpaceTreePlant.SpaceTreeReadyForHarvest, this.grown.healthy.trunkReadyForHarvest, false);
		this.grown.healthy.trunkReadyForHarvest.DefaultState(this.grown.healthy.trunkReadyForHarvest.idle).TagTransition(SpaceTreePlant.SpaceTreeReadyForHarvest, this.grown.healthy.filling, true);
		this.grown.healthy.trunkReadyForHarvest.idle.PlayAnim((SpaceTreeBranch.Instance smi) => smi.GetAnimationForState(this.grown.healthy.trunkReadyForHarvest.idle), KAnim.PlayMode.Loop).EventHandler(GameHashes.EntombedChanged, new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.PlayReadyForHarvestAnimationOnUnentomb)).EventHandlerTransition(GameHashes.SpaceTreeUnentombDefenseTriggered, this.grown.healthy.trunkReadyForHarvest.shaking, (SpaceTreeBranch.Instance o, object smi) => true)
			.EventTransition(GameHashes.SpaceTreeManualHarvestBegan, this.grown.healthy.trunkReadyForHarvest.harvestInProgress, null)
			.Update(delegate(SpaceTreeBranch.Instance smi, float dt)
			{
				SpaceTreeBranch.SynchAnimationWithTrunk(smi, "harvest_ready");
			}, UpdateRate.SIM_200ms, false);
		this.grown.healthy.trunkReadyForHarvest.harvestInProgress.DefaultState(this.grown.healthy.trunkReadyForHarvest.harvestInProgress.pre).EventTransition(GameHashes.SpaceTreeManualHarvestStopped, this.grown.healthy.trunkReadyForHarvest.idle, null);
		this.grown.healthy.trunkReadyForHarvest.harvestInProgress.pre.PlayAnim((SpaceTreeBranch.Instance smi) => smi.GetAnimationForState(this.grown.healthy.trunkReadyForHarvest.harvestInProgress.pre), KAnim.PlayMode.Once).Update(delegate(SpaceTreeBranch.Instance smi, float dt)
		{
			SpaceTreeBranch.SynchAnimationWithTrunk(smi, "syrup_harvest_trunk_pre");
		}, UpdateRate.SIM_200ms, false).Transition(this.grown.healthy.trunkReadyForHarvest.harvestInProgress.loop, new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Transition.ConditionCallback(SpaceTreeBranch.TransitToManualHarvest_Loop), UpdateRate.SIM_200ms);
		this.grown.healthy.trunkReadyForHarvest.harvestInProgress.loop.PlayAnim((SpaceTreeBranch.Instance smi) => smi.GetAnimationForState(this.grown.healthy.trunkReadyForHarvest.harvestInProgress.loop), KAnim.PlayMode.Loop).Update(delegate(SpaceTreeBranch.Instance smi, float dt)
		{
			SpaceTreeBranch.SynchAnimationWithTrunk(smi, "syrup_harvest_trunk_loop");
		}, UpdateRate.SIM_200ms, false).Transition(this.grown.healthy.trunkReadyForHarvest.harvestInProgress.pst, new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Transition.ConditionCallback(SpaceTreeBranch.TransitToManualHarvest_Pst), UpdateRate.SIM_200ms);
		this.grown.healthy.trunkReadyForHarvest.harvestInProgress.pst.PlayAnim((SpaceTreeBranch.Instance smi) => smi.GetAnimationForState(this.grown.healthy.trunkReadyForHarvest.harvestInProgress.pst), KAnim.PlayMode.Once).Update(delegate(SpaceTreeBranch.Instance smi, float dt)
		{
			SpaceTreeBranch.SynchAnimationWithTrunk(smi, "syrup_harvest_trunk_pst");
		}, UpdateRate.SIM_200ms, false);
		this.grown.healthy.trunkReadyForHarvest.shaking.PlayAnim((SpaceTreeBranch.Instance smi) => smi.entombDefenseSMI.UnentombAnimName, KAnim.PlayMode.Once).OnAnimQueueComplete(this.grown.healthy.trunkReadyForHarvest.idle);
		this.grown.trunkWilted.DefaultState(this.grown.trunkWilted.wilted).EventTransition(GameHashes.RootHealthChanged, this.grown.spawn, new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.Transition.ConditionCallback(SpaceTreeBranch.IsTrunkHealthy)).EventTransition(GameHashes.WiltRecover, this.grown.spawn, null)
			.TagTransition(SpaceTreePlant.SpaceTreeReadyForHarvest, this.grown.healthy.trunkReadyForHarvest, false);
		this.grown.trunkWilted.wilted.PlayAnim((SpaceTreeBranch.Instance smi) => smi.GetAnimationForState(this.grown.trunkWilted), KAnim.PlayMode.Once).EventHandlerTransition(GameHashes.SpaceTreeUnentombDefenseTriggered, this.grown.trunkWilted.shaking, (SpaceTreeBranch.Instance o, object smi) => true);
		this.grown.trunkWilted.shaking.PlayAnim((SpaceTreeBranch.Instance smi) => smi.entombDefenseSMI.UnentombAnimName, KAnim.PlayMode.Once).OnAnimQueueComplete(this.grown.trunkWilted.wilted);
		this.harvestedForWood.PlayAnim((SpaceTreeBranch.Instance smi) => smi.GetAnimationForState(this.harvestedForWood), KAnim.PlayMode.Once).Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.DisableEntombDefenses)).Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.SpawnWoodOnHarvest))
			.Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.ForbidBranchToBeHarvestedForWood))
			.Exit(delegate(SpaceTreeBranch.Instance smi)
			{
				smi.Trigger(113170146, null);
			})
			.OnAnimQueueComplete(this.growing);
		this.die.Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.DisableEntombDefenses)).DefaultState(this.die.entering);
		this.die.entering.PlayAnim((SpaceTreeBranch.Instance smi) => smi.GetAnimationForState(this.die.entering), KAnim.PlayMode.Once).Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.SpawnWoodOnDeath)).OnAnimQueueComplete(this.die.selfDelete)
			.ScheduleGoTo(2f, this.die.selfDelete);
		this.die.selfDelete.Enter(new StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State.Callback(SpaceTreeBranch.SelfDestroy));
	}

	public static bool TransitToManualHarvest_Loop(SpaceTreeBranch.Instance smi)
	{
		return smi.GetCurrentTrunkAnim() != null && smi.GetCurrentTrunkAnim() == "syrup_harvest_trunk_loop";
	}

	public static bool TransitToManualHarvest_Pst(SpaceTreeBranch.Instance smi)
	{
		return smi.GetCurrentTrunkAnim() != null && smi.GetCurrentTrunkAnim() == "syrup_harvest_trunk_pst";
	}

	public static bool IsWiltedConditionReportingWilted(SpaceTreeBranch.Instance smi)
	{
		return smi.wiltCondition.IsWilting();
	}

	public static bool IsBranchFullyGrown(SpaceTreeBranch.Instance smi)
	{
		return smi.IsBranchFullyGrown;
	}

	public static bool IsTrunkWildPlanted(SpaceTreeBranch.Instance smi)
	{
		return smi.IsTrunkWildPlanted;
	}

	public static bool IsEntombed(SpaceTreeBranch.Instance smi)
	{
		return smi.IsEntombed;
	}

	public static bool IsTrunkHealthy(SpaceTreeBranch.Instance smi)
	{
		return smi.IsTrunkHealthy;
	}

	public static void PlayFillAnimationForThisState(SpaceTreeBranch.Instance smi)
	{
		smi.PlayFillAnimation();
	}

	public static void OnTrunkSyrupFullnessChanged(SpaceTreeBranch.Instance smi, object obj)
	{
		smi.PlayFillAnimation(((Boxed<float>)obj).value);
	}

	public static void SynchAnimationWithTrunk(SpaceTreeBranch.Instance smi, HashedString animName)
	{
		smi.SynchCurrentAnimWithTrunkAnim(animName);
	}

	public static void EnableGlowFlowerMeter(SpaceTreeBranch.Instance smi)
	{
		smi.ActivateGlowFlowerMeter();
	}

	public static void DisableGlowFlowerMeter(SpaceTreeBranch.Instance smi)
	{
		smi.DeactivateGlowFlowerMeter();
	}

	public static void UpdateFlowerOnWilt(SpaceTreeBranch.Instance smi)
	{
		smi.PlayAnimOnFlower(smi.Animations.meterAnim_flowerWilted, KAnim.PlayMode.Loop);
	}

	public static void UpdateFlowerOnWiltRecover(SpaceTreeBranch.Instance smi)
	{
		smi.PlayAnimOnFlower(smi.Animations.meterAnimNames, KAnim.PlayMode.Loop);
	}

	public static void EnableEntombDefenses(SpaceTreeBranch.Instance smi)
	{
		smi.GetSMI<UnstableEntombDefense.Instance>().SetActive(true);
	}

	public static void DisableEntombDefenses(SpaceTreeBranch.Instance smi)
	{
		smi.GetSMI<UnstableEntombDefense.Instance>().SetActive(false);
	}

	public static void AllowItToBeHarvestForWood(SpaceTreeBranch.Instance smi)
	{
		smi.harvestable.SetCanBeHarvested(true);
	}

	public static void ForbidBranchToBeHarvestedForWood(SpaceTreeBranch.Instance smi)
	{
		smi.harvestable.SetCanBeHarvested(false);
	}

	public static void SpawnWoodOnHarvest(SpaceTreeBranch.Instance smi)
	{
		smi.crop.SpawnConfiguredFruit(null);
	}

	public static void SpawnWoodOnDeath(SpaceTreeBranch.Instance smi)
	{
		if (smi.harvestable != null && smi.harvestable.CanBeHarvested)
		{
			smi.crop.SpawnConfiguredFruit(null);
		}
	}

	public static void OnConsumed(SpaceTreeBranch.Instance smi)
	{
	}

	public static void SelfDestroy(SpaceTreeBranch.Instance smi)
	{
		Util.KDestroyGameObject(smi.gameObject);
	}

	public static void PlayFillAnimationOnUnentomb(SpaceTreeBranch.Instance smi)
	{
		if (!smi.IsEntombed)
		{
			SpaceTreeBranch.PlayFillAnimationForThisState(smi);
		}
	}

	public static void PlayReadyForHarvestAnimationOnUnentomb(SpaceTreeBranch.Instance smi)
	{
		if (!smi.IsEntombed)
		{
			smi.PlayReadyForHarvestAnimation();
		}
	}

	public const int FILL_ANIM_FRAME_COUNT = 42;

	public const int SHAKE_ANIM_FRAME_COUNT = 54;

	public const float SHAKE_ANIM_DURATION = 1.8f;

	private StateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.BoolParameter HasSpawn;

	private SpaceTreeBranch.GrowingStates growing;

	private SpaceTreeBranch.GrowHaltState halt;

	private SpaceTreeBranch.GrownStates grown;

	private GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State harvestedForWood;

	private SpaceTreeBranch.DieStates die;

	public class AnimSet
	{
		public string[] meterTargets;

		public string[] meterAnimNames;

		public string undeveloped;

		public string spawn;

		public string spawn_pst;

		public string fill;

		public string ready_harvest;

		public string[] meterAnim_flowerWilted;

		public string wilted;

		public string wilted_short_trunk_healthy;

		public string wilted_short_trunk_wilted;

		public string hidden;

		public string die;

		public string manual_harvest_pre;

		public string manual_harvest_loop;

		public string manual_harvest_pst;
	}

	public class Def : StateMachine.BaseDef
	{
		public int OPTIMAL_LUX_LEVELS;

		public float GROWTH_RATE = 0.0016666667f;

		public float WILD_GROWTH_RATE = 0.00041666668f;
	}

	public class GrowingState : GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State
	{
		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State visible;

		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State hidden;
	}

	public class GrowingStates : GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.PlantAliveSubState
	{
		public SpaceTreeBranch.GrowingState wild;

		public SpaceTreeBranch.GrowingState planted;
	}

	public class GrownStates : GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.PlantAliveSubState
	{
		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State spawn;

		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State spawnPST;

		public SpaceTreeBranch.HealthyStates healthy;

		public SpaceTreeBranch.WiltStates trunkWilted;
	}

	public class GrowHaltState : GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.PlantAliveSubState
	{
		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State wilted;

		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State trunkWilted;

		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State shaking;

		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State hidden;
	}

	public class WiltStates : GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State
	{
		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State wilted;

		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State shaking;
	}

	public class DieStates : GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State
	{
		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State entering;

		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State selfDelete;
	}

	public class ReadyForHarvest : GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State
	{
		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State idle;

		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State shaking;

		public SpaceTreeBranch.ManualHarvestStates harvestInProgress;
	}

	public class ManualHarvestStates : GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State
	{
		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State pre;

		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State loop;

		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State pst;
	}

	public class HealthyStates : GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State
	{
		public GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.State filling;

		public SpaceTreeBranch.ReadyForHarvest trunkReadyForHarvest;
	}

	public new class Instance : GameStateMachine<SpaceTreeBranch, SpaceTreeBranch.Instance, IStateMachineTarget, SpaceTreeBranch.Def>.GameInstance, IManageGrowingStates
	{
		public int CurrentAmountOfLux
		{
			get
			{
				return Grid.LightIntensity[this.cell];
			}
		}

		public float Productivity
		{
			get
			{
				if (!this.IsBranchFullyGrown)
				{
					return 0f;
				}
				return Mathf.Clamp((float)this.CurrentAmountOfLux / (float)base.def.OPTIMAL_LUX_LEVELS, 0f, 1f);
			}
		}

		public bool IsTrunkHealthy
		{
			get
			{
				return this.trunk != null && !this.trunk.HasTag(GameTags.Wilting);
			}
		}

		public bool IsTrunkWildPlanted
		{
			get
			{
				return this.trunk != null && !this.trunk.GetComponent<ReceptacleMonitor>().Replanted;
			}
		}

		public bool IsEntombed
		{
			get
			{
				return this.entombDefenseSMI != null && this.entombDefenseSMI.IsEntombed;
			}
		}

		public bool IsBranchFullyGrown
		{
			get
			{
				return this.GetcurrentGrowthPercentage() >= 1f;
			}
		}

		private PlantBranchGrower.Instance trunk
		{
			get
			{
				if (this._trunk == null)
				{
					this._trunk = this.branch.GetTrunk();
					if (this._trunk != null)
					{
						this.trunkAnimController = this._trunk.GetComponent<KBatchedAnimController>();
					}
				}
				return this._trunk;
			}
		}

		public void OverrideMaturityLevel(float percent)
		{
			float num = this.maturity.GetMax() * percent;
			this.maturity.SetValue(num);
		}

		public Instance(IStateMachineTarget master, SpaceTreeBranch.Def def)
			: base(master, def)
		{
			this.cell = Grid.PosToCell(this);
			Amounts amounts = base.gameObject.GetAmounts();
			this.maturity = amounts.Get(Db.Get().Amounts.Maturity);
			this.baseGrowingRate = new AttributeModifier(this.maturity.deltaAttribute.Id, def.GROWTH_RATE, CREATURES.STATS.MATURITY.GROWING, false, false, true);
			this.wildGrowingRate = new AttributeModifier(this.maturity.deltaAttribute.Id, def.WILD_GROWTH_RATE, CREATURES.STATS.MATURITY.GROWINGWILD, false, false, true);
			base.Subscribe(1272413801, new Action<object>(this.ResetGrowth));
		}

		public float GetcurrentGrowthPercentage()
		{
			return this.maturity.value / this.maturity.GetMax();
		}

		public void ResetGrowth(object data = null)
		{
			this.maturity.value = 0f;
			base.sm.HasSpawn.Set(false, this, false);
			base.smi.gameObject.Trigger(-254803949, null);
		}

		public override void StartSM()
		{
			this.branch = base.smi.GetSMI<PlantBranch.Instance>();
			this.entombDefenseSMI = base.smi.GetSMI<UnstableEntombDefense.Instance>();
			if (this.Animations.meterTargets != null)
			{
				this.CreateMeters(this.Animations.meterTargets, this.Animations.meterAnimNames);
			}
			base.StartSM();
		}

		public void CreateMeters(string[] meterTargets, string[] meterAnimNames)
		{
			this.flowerMeters = new MeterController[meterTargets.Length];
			for (int i = 0; i < this.flowerMeters.Length; i++)
			{
				this.flowerMeters[i] = new MeterController(this.animController, meterTargets[i], meterAnimNames[i], Meter.Offset.NoChange, Grid.SceneLayer.Building, Array.Empty<string>());
			}
		}

		public void RefreshAnimation()
		{
			if (this.flowerMeters == null && this.Animations.meterTargets != null)
			{
				this.CreateMeters(this.Animations.meterTargets, this.Animations.meterAnimNames);
			}
			KAnim.PlayMode playMode = (base.IsInsideState(base.sm.grown.healthy) ? KAnim.PlayMode.Loop : KAnim.PlayMode.Once);
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			if (component != null)
			{
				component.Play(this.GetAnimationForState(this.GetCurrentState()), playMode, 1f, 0f);
			}
			if (base.IsInsideState(base.smi.sm.grown.healthy))
			{
				this.ActivateGlowFlowerMeter();
				return;
			}
			this.DeactivateGlowFlowerMeter();
		}

		public HashedString GetCurrentTrunkAnim()
		{
			if (this.trunk != null && this.trunkAnimController != null)
			{
				return this.trunkAnimController.currentAnim;
			}
			return null;
		}

		public void SynchCurrentAnimWithTrunkAnim(HashedString trunkAnimNameToSynchTo)
		{
			if (this.trunk != null && this.trunkAnimController != null && this.trunkAnimController.currentAnim == trunkAnimNameToSynchTo)
			{
				float elapsedTime = this.trunkAnimController.GetElapsedTime();
				base.smi.animController.SetElapsedTime(elapsedTime);
			}
		}

		public string GetAnimationForState(StateMachine.BaseState state)
		{
			if (state == base.sm.growing.wild.visible)
			{
				return this.Animations.undeveloped;
			}
			if (state == base.sm.growing.planted.visible)
			{
				return this.Animations.undeveloped;
			}
			if (state == base.sm.growing.wild.hidden)
			{
				return this.Animations.hidden;
			}
			if (state == base.sm.growing.planted.hidden)
			{
				return this.Animations.hidden;
			}
			if (state == base.sm.grown.spawn)
			{
				return this.Animations.spawn;
			}
			if (state == base.sm.grown.spawnPST)
			{
				return this.Animations.spawn_pst;
			}
			if (state == base.sm.grown.healthy.filling)
			{
				return this.Animations.fill;
			}
			if (state == base.sm.grown.healthy.trunkReadyForHarvest.idle)
			{
				return this.Animations.ready_harvest;
			}
			if (state == base.sm.grown.healthy.trunkReadyForHarvest.harvestInProgress.pre)
			{
				return this.Animations.manual_harvest_pre;
			}
			if (state == base.sm.grown.healthy.trunkReadyForHarvest.harvestInProgress.loop)
			{
				return this.Animations.manual_harvest_loop;
			}
			if (state == base.sm.grown.healthy.trunkReadyForHarvest.harvestInProgress.pst)
			{
				return this.Animations.manual_harvest_pst;
			}
			if (state == base.sm.grown.trunkWilted)
			{
				return this.Animations.wilted;
			}
			if (state == base.sm.halt.wilted)
			{
				return this.Animations.wilted_short_trunk_healthy;
			}
			if (state == base.sm.halt.trunkWilted)
			{
				return this.Animations.wilted_short_trunk_wilted;
			}
			if (state == base.sm.halt.shaking)
			{
				return this.Animations.hidden;
			}
			if (state == base.sm.halt.hidden)
			{
				return this.Animations.hidden;
			}
			if (state == base.sm.harvestedForWood)
			{
				return this.Animations.die;
			}
			if (state == base.sm.die.entering)
			{
				return this.Animations.die;
			}
			return this.Animations.spawn;
		}

		public string GetFillAnimNameForState(StateMachine.BaseState state)
		{
			string fill = this.Animations.fill;
			if (state == base.sm.grown.healthy.filling)
			{
				return this.Animations.fill;
			}
			if (state == base.sm.growing.wild.visible)
			{
				return this.Animations.undeveloped;
			}
			if (state == base.sm.growing.planted.visible)
			{
				return this.Animations.undeveloped;
			}
			if (state == base.sm.halt.wilted)
			{
				return this.Animations.wilted_short_trunk_healthy;
			}
			return fill;
		}

		public void PlayReadyForHarvestAnimation()
		{
			if (this.animController != null)
			{
				this.animController.Play(this.Animations.ready_harvest, KAnim.PlayMode.Loop, 1f, 0f);
			}
		}

		public void PlayFillAnimation()
		{
			this.PlayFillAnimation(this.lastFillAmountRecorded);
		}

		public void PlayFillAnimation(float fillLevel)
		{
			string fillAnimNameForState = this.GetFillAnimNameForState(base.smi.GetCurrentState());
			this.lastFillAmountRecorded = fillLevel;
			if (this.entombDefenseSMI.IsEntombed && this.entombDefenseSMI.IsActive)
			{
				return;
			}
			if (this.animController != null)
			{
				int num = Mathf.FloorToInt(fillLevel * 42f);
				if (this.animController.currentAnim != fillAnimNameForState)
				{
					this.animController.Play(fillAnimNameForState, KAnim.PlayMode.Once, 0f, 0f);
				}
				if (this.animController.currentFrame != num)
				{
					this.animController.SetPositionPercent(fillLevel);
				}
			}
		}

		public void ActivateGlowFlowerMeter()
		{
			if (this.flowerMeters != null)
			{
				for (int i = 0; i < this.flowerMeters.Length; i++)
				{
					this.flowerMeters[i].gameObject.SetActive(true);
					this.flowerMeters[i].meterController.Play(this.flowerMeters[i].meterController.currentAnim, KAnim.PlayMode.Loop, 1f, 0f);
				}
			}
		}

		public void PlayAnimOnFlower(string[] animNames, KAnim.PlayMode playMode)
		{
			if (this.flowerMeters != null)
			{
				for (int i = 0; i < this.flowerMeters.Length; i++)
				{
					this.flowerMeters[i].meterController.Play(animNames[i], playMode, 1f, 0f);
				}
			}
		}

		public void DeactivateGlowFlowerMeter()
		{
			if (this.flowerMeters != null)
			{
				for (int i = 0; i < this.flowerMeters.Length; i++)
				{
					this.flowerMeters[i].gameObject.SetActive(false);
				}
			}
		}

		public float TimeUntilNextHarvest()
		{
			return (this.maturity.GetMax() - this.maturity.value) / this.maturity.GetDelta();
		}

		public float PercentGrown()
		{
			return this.GetcurrentGrowthPercentage();
		}

		public Crop GetCropComponent()
		{
			return base.GetComponent<Crop>();
		}

		public float DomesticGrowthTime()
		{
			return this.maturity.GetMax() / base.smi.baseGrowingRate.Value;
		}

		public float WildGrowthTime()
		{
			return this.maturity.GetMax() / base.smi.wildGrowingRate.Value;
		}

		public bool IsWildPlanted()
		{
			return this.IsTrunkWildPlanted;
		}

		[MyCmpGet]
		public WiltCondition wiltCondition;

		[MyCmpGet]
		public Crop crop;

		[MyCmpGet]
		public Harvestable harvestable;

		[MyCmpGet]
		public KBatchedAnimController animController;

		public SpaceTreeBranch.AnimSet Animations = new SpaceTreeBranch.AnimSet();

		private int cell;

		private float lastFillAmountRecorded;

		private AmountInstance maturity;

		public AttributeModifier baseGrowingRate;

		public AttributeModifier wildGrowingRate;

		public UnstableEntombDefense.Instance entombDefenseSMI;

		private MeterController[] flowerMeters;

		private PlantBranch.Instance branch;

		private KBatchedAnimController trunkAnimController;

		private PlantBranchGrower.Instance _trunk;
	}
}
