using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class VineBranch : PlantBranchGrowerBase<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>
{
	public static bool IsCellFoundation(int cell)
	{
		return Grid.IsSolidCell(cell) || Grid.HasDoor[cell];
	}

	public static bool IsCellAvailable(GameObject questionerObj, int cell, Func<int, bool> foundationCheckFunction = null)
	{
		int num = Grid.PosToCell(questionerObj);
		int num2 = (int)Grid.WorldIdx[num];
		return cell != Grid.InvalidCell && (int)Grid.WorldIdx[cell] == num2 && ((foundationCheckFunction == null) ? (!VineBranch.IsCellFoundation(cell)) : (!foundationCheckFunction(cell))) && !Grid.IsLiquid(cell) && Grid.Objects[cell, 1] == null && Grid.Objects[cell, 5] == null;
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.undevelopedBranch;
		this.undevelopedBranch.InitializeStates(this.masterTarget, this.Mother, this.dead, this.DieSignal).ParamTransition<bool>(this.MarkedForDeath, this.dead, GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.IsTrue).ParamTransition<GameObject>(this.Mother, this.dead, GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.IsNull)
			.EventTransition(GameHashes.Grow, this.mature, (VineBranch.Instance smi) => smi.IsGrown)
			.UpdateTransition(this.mature, (VineBranch.Instance smi, float dt) => smi.IsGrown, UpdateRate.SIM_4000ms, false)
			.Enter(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.RecalculateMyShape))
			.Enter(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.SubscribreSurroundingCellChangeListeners))
			.Exit(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.UnSubscribreSurroundingSolidChangesListeners))
			.DefaultState(this.undevelopedBranch.growing);
		this.undevelopedBranch.wilted.PlayAnim(new Func<VineBranch.Instance, string>(VineBranch.GetWiltAnim), KAnim.PlayMode.Loop).EventHandler(GameHashes.BranchShapeChanged, delegate(VineBranch.Instance smi)
		{
			VineBranch.RefreshAnim(smi, VineBranch.GetWiltAnim(smi), KAnim.PlayMode.Loop);
		}).EventTransition(GameHashes.WiltRecover, this.undevelopedBranch.growing, (VineBranch.Instance smi) => !smi.IsWilting);
		this.undevelopedBranch.growing.EventTransition(GameHashes.Wilt, this.undevelopedBranch.wilted, (VineBranch.Instance smi) => smi.IsWilting).PlayAnim((VineBranch.Instance smi) => smi.Anims.grow, KAnim.PlayMode.Paused).EventHandler(GameHashes.BranchShapeChanged, delegate(VineBranch.Instance smi)
		{
			VineBranch.RefreshAnim(smi, smi.Anims.grow, KAnim.PlayMode.Paused);
		})
			.ToggleStatusItem(Db.Get().CreatureStatusItems.Growing, (VineBranch.Instance smi) => smi)
			.Enter(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.RefreshPositionPercent))
			.Update(new Action<VineBranch.Instance, float>(VineBranch.RefreshPositionPercent), UpdateRate.SIM_4000ms, false)
			.EventHandler(GameHashes.ConsumePlant, new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.RefreshPositionPercent))
			.DefaultState(this.undevelopedBranch.growing.wild);
		this.undevelopedBranch.growing.wild.ParamTransition<bool>(this.WildPlanted, this.undevelopedBranch.growing.domestic, GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.IsFalse).ToggleAttributeModifier("Growing", (VineBranch.Instance smi) => smi.wildGrowingRate, null);
		this.undevelopedBranch.growing.domestic.ParamTransition<bool>(this.WildPlanted, this.undevelopedBranch.growing.wild, GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.IsTrue).ToggleAttributeModifier("Growing", (VineBranch.Instance smi) => smi.baseGrowingRate, null);
		this.mature.InitializeStates(this.masterTarget, this.Mother, this.dead, this.DieSignal).ParamTransition<bool>(this.MarkedForDeath, this.dead, GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.IsTrue).ParamTransition<GameObject>(this.Mother, this.dead, GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.IsNull)
			.Enter(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.RecalculateShapeAndSpawnBranchesIfSpawnedByDiscovery))
			.Enter(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.SetupFruitMeter))
			.Enter(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.SubscribreSurroundingCellChangeListeners))
			.Exit(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.UnSubscribreSurroundingSolidChangesListeners))
			.Update(new Action<VineBranch.Instance, float>(VineBranch.SpawnBranchIfPossible), UpdateRate.SIM_4000ms, false)
			.DefaultState(this.mature.healthy);
		this.mature.healthy.PlayAnim((VineBranch.Instance smi) => smi.Anims.idle, KAnim.PlayMode.Loop).EventHandler(GameHashes.BranchShapeChanged, delegate(VineBranch.Instance smi)
		{
			VineBranch.RefreshAnim(smi, smi.Anims.idle, KAnim.PlayMode.Loop);
		}).DefaultState(this.mature.healthy.growing);
		this.mature.healthy.growing.EventTransition(GameHashes.Grow, this.mature.healthy.harvestReady, (VineBranch.Instance smi) => smi.IsReadyForHarvest).UpdateTransition(this.mature.healthy.harvestReady, (VineBranch.Instance smi, float dt) => smi.IsReadyForHarvest, UpdateRate.SIM_4000ms, false).EventTransition(GameHashes.Wilt, this.mature.wilted, (VineBranch.Instance smi) => smi.IsWilting)
			.EventHandler(GameHashes.BranchShapeChanged, new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.RecreateFruitMeter))
			.EventHandler(GameHashes.BranchShapeChanged, new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.UpdateFruitMeterGrowAnimations))
			.ToggleStatusItem(Db.Get().CreatureStatusItems.GrowingFruit, (VineBranch.Instance smi) => smi)
			.Enter(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.UpdateFruitMeterGrowAnimations))
			.Update(new Action<VineBranch.Instance, float>(VineBranch.UpdateFruitMeterGrowAnimations), UpdateRate.SIM_200ms, false)
			.DefaultState(this.mature.healthy.growing.wild);
		this.mature.healthy.growing.wild.ParamTransition<bool>(this.WildPlanted, this.mature.healthy.growing.domestic, GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.IsFalse).ToggleAttributeModifier("Fruit Growing", (VineBranch.Instance smi) => smi.wildFruitGrowingRate, null);
		this.mature.healthy.growing.domestic.ParamTransition<bool>(this.WildPlanted, this.mature.healthy.growing.wild, GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.IsTrue).ToggleAttributeModifier("Fruit Growing", (VineBranch.Instance smi) => smi.baseFruitGrowingRate, null);
		this.mature.healthy.harvestReady.ToggleTag(GameTags.FullyGrown).EventTransition(GameHashes.Harvest, this.mature.healthy.harvest, null).EventHandler(GameHashes.BranchShapeChanged, new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.RecreateFruitMeter))
			.EventHandler(GameHashes.BranchShapeChanged, delegate(VineBranch.Instance smi)
			{
				VineBranch.PlayAnimsOnFruit(smi, smi.Anims.meter_harvest_ready, KAnim.PlayMode.Loop);
			})
			.Enter(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.MakeItHarvestable))
			.Exit(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.ResetOldAge))
			.ToggleAttributeModifier("GetOld", (VineBranch.Instance smi) => smi.getOldRate, null)
			.Enter(delegate(VineBranch.Instance smi)
			{
				VineBranch.PlayAnimsOnFruit(smi, smi.Anims.meter_harvest_ready, KAnim.PlayMode.Loop);
			})
			.UpdateTransition(this.mature.healthy.selfHarvestFromOld, new Func<VineBranch.Instance, float, bool>(VineBranch.ShouldSelfHarvestFromOldAge), UpdateRate.SIM_4000ms, false);
		this.mature.healthy.harvest.Target(this.Fruit).OnAnimQueueComplete(this.mature.healthy.growing).Target(this.masterTarget)
			.Enter(delegate(VineBranch.Instance smi)
			{
				VineBranch.PlayAnimsOnFruit(smi, smi.Anims.meter_harvest, KAnim.PlayMode.Once);
			})
			.Enter(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.MakeItNotHarvestable))
			.Enter(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.ResetFruitGrowProgress))
			.Enter(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.SpawnHarvestedFruit))
			.TriggerOnExit(GameHashes.HarvestComplete, null)
			.ScheduleGoTo(3f, this.mature.healthy.growing);
		this.mature.healthy.selfHarvestFromOld.Target(this.Fruit).OnAnimQueueComplete(this.mature.healthy.growing).Target(this.masterTarget)
			.Enter(delegate(VineBranch.Instance smi)
			{
				VineBranch.PlayAnimsOnFruit(smi, smi.Anims.meter_harvest, KAnim.PlayMode.Once);
			})
			.Enter(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.ForceCancelHarvest))
			.Enter(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.MakeItNotHarvestable))
			.Enter(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.ResetOldAge))
			.Enter(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.ResetFruitGrowProgress))
			.Enter(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.SpawnHarvestedFruit))
			.TriggerOnExit(GameHashes.HarvestComplete, null)
			.ScheduleGoTo(3f, this.mature.healthy.growing);
		this.mature.wilted.PlayAnim(new Func<VineBranch.Instance, string>(VineBranch.GetWiltAnim), KAnim.PlayMode.Loop).Enter(delegate(VineBranch.Instance smi)
		{
			VineBranch.PlayAnimsOnFruit(smi, VineBranch.GetFruitWiltAnim(smi), KAnim.PlayMode.Loop);
		}).EventHandler(GameHashes.BranchShapeChanged, delegate(VineBranch.Instance smi)
		{
			VineBranch.RefreshAnim(smi, VineBranch.GetWiltAnim(smi), KAnim.PlayMode.Loop);
		})
			.EventHandler(GameHashes.BranchShapeChanged, new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.RecreateFruitMeter))
			.EventHandler(GameHashes.BranchShapeChanged, delegate(VineBranch.Instance smi)
			{
				VineBranch.PlayAnimsOnFruit(smi, smi.Anims.meter_wilted, KAnim.PlayMode.Loop);
			})
			.EventTransition(GameHashes.WiltRecover, this.mature.healthy, (VineBranch.Instance smi) => !smi.IsWilting)
			.EventTransition(GameHashes.Harvest, this.mature.healthy.harvest, null);
		this.dead.Target(this.masterTarget).ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead, null).Enter(new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.HarvestOnDeath))
			.Enter(delegate(VineBranch.Instance smi)
			{
				if (!smi.gameObject.GetComponent<KPrefabID>().HasTag(GameTags.Uprooted) && !smi.IsWild)
				{
					Notifier notifier = smi.gameObject.AddOrGet<Notifier>();
					Notification notification = VineBranch.CreateDeathNotification(smi);
					notifier.Add(notification, "");
				}
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
				smi.Trigger(1623392196, null);
				smi.DestroySelf(null);
			});
	}

	private static bool ShouldSelfHarvestFromOldAge(VineBranch.Instance smi, float dt)
	{
		return smi.IsOld;
	}

	private static string GetWiltAnim(VineBranch.Instance smi)
	{
		return smi.Anims.GetBaseWiltAnim(smi.Anims.GetWiltLevel(smi.GrowthPercentage));
	}

	private static string GetFruitWiltAnim(VineBranch.Instance smi)
	{
		return smi.Anims.GetMeterWiltAnim(smi.Anims.GetWiltLevel(smi.FruitGrowthPercentage));
	}

	private static void PlayAnimsOnFruit(VineBranch.Instance smi, string animName, KAnim.PlayMode playmode)
	{
		smi.PlayAnimOnFruitMeter(animName, playmode);
	}

	private static void UpdateFruitMeterGrowAnimations(VineBranch.Instance smi, float dt)
	{
		VineBranch.UpdateFruitMeterGrowAnimations(smi);
	}

	private static void UpdateFruitMeterGrowAnimations(VineBranch.Instance smi)
	{
		smi.UpdateFruitGrowMeterPosition();
	}

	private static void RecreateFruitMeter(VineBranch.Instance smi)
	{
		smi.CreateFruitMeter();
	}

	private static void SetupFruitMeter(VineBranch.Instance smi)
	{
		smi.CreateFruitMeter();
	}

	private static void SpawnBranchIfPossible(VineBranch.Instance smi, float dt)
	{
		smi.AttemptToSpawnBranch();
	}

	private static void MakeItHarvestable(VineBranch.Instance smi)
	{
		smi.SetHarvestableState(true);
	}

	private static void ForceCancelHarvest(VineBranch.Instance smi)
	{
		smi.ForceCancelHarvest();
	}

	private static void MakeItNotHarvestable(VineBranch.Instance smi)
	{
		smi.SetHarvestableState(false);
	}

	private static void RefreshPositionPercent(VineBranch.Instance smi, float dt)
	{
		VineBranch.RefreshPositionPercent(smi);
	}

	private static void RefreshPositionPercent(VineBranch.Instance smi)
	{
		smi.AnimController.SetPositionPercent(smi.GrowthPercentage);
	}

	private static void SubscribreSurroundingCellChangeListeners(VineBranch.Instance smi)
	{
		smi.SubscribeSurroundingSolidChangesListeners();
	}

	private static void UnSubscribreSurroundingSolidChangesListeners(VineBranch.Instance smi)
	{
		smi.UnSubscribreSurroundingSolidChangesListeners();
	}

	private static void ResetFruitGrowProgress(VineBranch.Instance smi)
	{
		smi.ResetFruitGrowProgress();
	}

	private static void ResetOldAge(VineBranch.Instance smi)
	{
		smi.ResetOldAge();
	}

	private static void SpawnHarvestedFruit(VineBranch.Instance smi)
	{
		smi.SpawnHarvestedFruit();
	}

	private static void RecalculateMyShape(VineBranch.Instance smi)
	{
		smi.RecalculateMyShape();
	}

	private static void OnMotherRecovered(VineBranch.Instance smi)
	{
		smi.BoxingTrigger(912965142, true);
	}

	private static void OnMotherWilted(VineBranch.Instance smi)
	{
		smi.BoxingTrigger(912965142, false);
	}

	private static void RecalculateShapeAndSpawnBranchesIfSpawnedByDiscovery(VineBranch.Instance smi)
	{
		if (smi.IsNewGameSpawned)
		{
			smi.RecalculateMyShape();
			VineBranch.SpawnBranchIfPossible(smi, 0f);
		}
	}

	public static void HarvestOnDeath(VineBranch.Instance smi)
	{
		if (smi.IsReadyForHarvest)
		{
			VineBranch.SpawnHarvestedFruit(smi);
		}
	}

	private static void RefreshAnim(VineBranch.Instance smi, string animName, KAnim.PlayMode playmode)
	{
		float elapsedTime = smi.AnimController.GetElapsedTime();
		smi.AnimController.Play(animName, playmode, 1f, 0f);
		smi.AnimController.SetElapsedTime(elapsedTime);
	}

	private static Notification CreateDeathNotification(VineBranch.Instance smi)
	{
		return new Notification(CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION, NotificationType.Bad, (List<Notification> notificationList, object data) => CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), "/t• " + smi.gameObject.GetProperName(), true, 0f, null, null, null, true, false, false);
	}

	// Note: this type is marked as 'beforefieldinit'.
	static VineBranch()
	{
		Dictionary<VineBranch.Shape, VineBranch.ShapeCategory> dictionary = new Dictionary<VineBranch.Shape, VineBranch.ShapeCategory>();
		dictionary[VineBranch.Shape.Top] = VineBranch.ShapeCategory.Line;
		dictionary[VineBranch.Shape.Bottom] = VineBranch.ShapeCategory.Line;
		dictionary[VineBranch.Shape.Left] = VineBranch.ShapeCategory.Line;
		dictionary[VineBranch.Shape.Right] = VineBranch.ShapeCategory.Line;
		dictionary[VineBranch.Shape.InCornerTopLeft] = VineBranch.ShapeCategory.InCorner;
		dictionary[VineBranch.Shape.InCornerTopRight] = VineBranch.ShapeCategory.InCorner;
		dictionary[VineBranch.Shape.InCornerBottomLeft] = VineBranch.ShapeCategory.InCorner;
		dictionary[VineBranch.Shape.InCornerBottomRight] = VineBranch.ShapeCategory.InCorner;
		dictionary[VineBranch.Shape.OutCornerTopLeft] = VineBranch.ShapeCategory.OutCorner;
		dictionary[VineBranch.Shape.OutCornerTopRight] = VineBranch.ShapeCategory.OutCorner;
		dictionary[VineBranch.Shape.OutCornerBottomLeft] = VineBranch.ShapeCategory.OutCorner;
		dictionary[VineBranch.Shape.OutCornerBottomRight] = VineBranch.ShapeCategory.OutCorner;
		dictionary[VineBranch.Shape.TopEnd] = VineBranch.ShapeCategory.DeadEnd;
		dictionary[VineBranch.Shape.BottomEnd] = VineBranch.ShapeCategory.DeadEnd;
		dictionary[VineBranch.Shape.LeftEnd] = VineBranch.ShapeCategory.DeadEnd;
		dictionary[VineBranch.Shape.RightEnd] = VineBranch.ShapeCategory.DeadEnd;
		VineBranch.GetShapeCategory = dictionary;
		Dictionary<VineBranch.ShapeCategory, VineBranch.AnimSet> dictionary2 = new Dictionary<VineBranch.ShapeCategory, VineBranch.AnimSet>();
		dictionary2[VineBranch.ShapeCategory.Line] = new VineBranch.AnimSet("line_");
		dictionary2[VineBranch.ShapeCategory.InCorner] = new VineBranch.AnimSet("incorner_");
		dictionary2[VineBranch.ShapeCategory.OutCorner] = new VineBranch.AnimSet("outcorner_");
		dictionary2[VineBranch.ShapeCategory.DeadEnd] = new VineBranch.AnimSet("end_");
		VineBranch.GetAnimSetByShapeCategory = dictionary2;
	}

	private static Dictionary<VineBranch.Shape, VineBranch.ShapeCategory> GetShapeCategory;

	private static Dictionary<VineBranch.ShapeCategory, VineBranch.AnimSet> GetAnimSetByShapeCategory;

	public StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.TargetParameter Fruit;

	public StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.TargetParameter Mother;

	public StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.TargetParameter Branch;

	public StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.IntParameter BranchNumber;

	public StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.IntParameter BranchShape;

	public StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.BoolParameter GrowingClockwise;

	public StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.IntParameter RootShape;

	public StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.IntParameter RootDirection;

	public StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.BoolParameter WildPlanted;

	public StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.BoolParameter MarkedForDeath;

	public StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.Signal DieSignal;

	public StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.Signal OnShapeChangedSignal;

	public VineBranch.GrowingStates undevelopedBranch;

	public VineBranch.GrownStates mature;

	public GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State dead;

	public class AnimSet
	{
		public string pre_grow
		{
			get
			{
				return this.suffix + "pre_grow";
			}
		}

		public string grow
		{
			get
			{
				return this.suffix + "grow";
			}
		}

		public string grow_pst
		{
			get
			{
				return this.suffix + "grow_pst";
			}
		}

		public string idle
		{
			get
			{
				return this.suffix + "idle";
			}
		}

		public string meter_target
		{
			get
			{
				return this.suffix + "meter_target";
			}
		}

		public string meter
		{
			get
			{
				return this.suffix + "meter";
			}
		}

		public string meter_wilted
		{
			get
			{
				return this.suffix + "meter_wilted";
			}
		}

		public string meter_harvest
		{
			get
			{
				return this.suffix + "meter_harvest";
			}
		}

		public string meter_harvest_ready
		{
			get
			{
				return this.suffix + "meter_harvest_ready";
			}
		}

		private string wilted
		{
			get
			{
				return this.suffix + "wilted";
			}
		}

		public int GetWiltLevel(float growthPercentage)
		{
			int num;
			if (growthPercentage < 0.75f)
			{
				num = 1;
			}
			else if (growthPercentage < 1f)
			{
				num = 2;
			}
			else
			{
				num = 3;
			}
			return num;
		}

		public string GetBaseWiltAnim(int level)
		{
			return this.GetWiltAnim(this.wilted, level);
		}

		public string GetMeterWiltAnim(int level)
		{
			return this.GetWiltAnim(this.meter_wilted, level);
		}

		private string GetWiltAnim(string wiltName, int level)
		{
			return wiltName + level.ToString();
		}

		public AnimSet(string suffix)
		{
			this.suffix = suffix;
		}

		public string suffix;

		private const int WILT_LEVELS = 3;
	}

	public enum ShapeCategory
	{
		Line,
		InCorner,
		OutCorner,
		DeadEnd
	}

	public enum Shape
	{
		Top,
		Bottom,
		Left,
		Right,
		InCornerTopLeft,
		InCornerTopRight,
		InCornerBottomLeft,
		InCornerBottomRight,
		OutCornerTopLeft,
		OutCornerTopRight,
		OutCornerBottomLeft,
		OutCornerBottomRight,
		TopEnd,
		BottomEnd,
		LeftEnd,
		RightEnd
	}

	public class Def : PlantBranchGrowerBase<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.PlantBranchGrowerBaseDef
	{
		public float GROWTH_RATE = 0.0016666667f;

		public float WILD_GROWTH_RATE = 0.00041666668f;
	}

	public class GrowingSpeedState : GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State
	{
		public GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State wild;

		public GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State domestic;
	}

	public class BranchAliveSubstate : GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.PlantAliveSubState
	{
		public GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State InitializeStates(StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.TargetParameter plant, StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.TargetParameter mother, GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State death_state, StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.Signal dieSignal)
		{
			base.InitializeStates(plant, death_state);
			base.root.Target(plant).OnSignal(dieSignal, death_state).OnTargetLost(mother, death_state)
				.Target(mother)
				.EventHandler(GameHashes.Wilt, new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.OnMotherWilted))
				.EventHandler(GameHashes.WiltRecover, new StateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State.Callback(VineBranch.OnMotherRecovered))
				.Target(plant);
			return this;
		}
	}

	public class GrowingStates : VineBranch.BranchAliveSubstate
	{
		public VineBranch.GrowingSpeedState growing;

		public GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State wilted;
	}

	public class FruitGrowingStates : GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State
	{
		public VineBranch.GrowingSpeedState growing;

		public GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State wilted;

		public GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State harvestReady;

		public GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State selfHarvestFromOld;

		public GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State harvest;
	}

	public class GrownStates : VineBranch.BranchAliveSubstate
	{
		public VineBranch.FruitGrowingStates healthy;

		public GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.State wilted;
	}

	public new class Instance : GameStateMachine<VineBranch, VineBranch.Instance, IStateMachineTarget, VineBranch.Def>.GameInstance, IManageGrowingStates, IWiltCause
	{
		public GameObject Mother
		{
			get
			{
				return base.sm.Mother.Get(this);
			}
		}

		public GameObject Branch
		{
			get
			{
				return base.sm.Branch.Get(this);
			}
		}

		public VineBranch.Instance BranchSMI
		{
			get
			{
				if (!(this.Branch == null))
				{
					return this.Branch.GetSMI<VineBranch.Instance>();
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

		public bool IsGrowingClockwise
		{
			get
			{
				return base.sm.GrowingClockwise.Get(this);
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
				return this.MyBranchNumber >= 12;
			}
		}

		public bool CanChangeShape
		{
			get
			{
				return !this.isSpawningNextBranch && this.Branch == null && !this.MaxBranchNumberReached;
			}
		}

		public VineBranch.Shape MyShape
		{
			get
			{
				return (VineBranch.Shape)base.sm.BranchShape.Get(this);
			}
		}

		public VineBranch.ShapeCategory MyShapeCategory
		{
			get
			{
				return VineBranch.GetShapeCategory[this.MyShape];
			}
		}

		public VineBranch.Shape RootShape
		{
			get
			{
				return (VineBranch.Shape)base.sm.RootShape.Get(this);
			}
		}

		public Direction RootDirection
		{
			get
			{
				return (Direction)base.sm.RootDirection.Get(this);
			}
		}

		public VineBranch.AnimSet Anims
		{
			get
			{
				return VineBranch.GetAnimSetByShapeCategory[this.MyShapeCategory];
			}
		}

		public bool IsOld
		{
			get
			{
				return this.oldAge.value >= this.oldAge.GetMax();
			}
		}

		public bool IsWilting
		{
			get
			{
				return this.wiltCondition.IsWilting() || this.MotherSMI.IsWilting;
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

		public Instance(IStateMachineTarget master, VineBranch.Def def)
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
			this.AnimController = base.GetComponent<KBatchedAnimController>();
			this.uprootMonitor = base.GetComponent<UprootedMonitor>();
			this.harvestable = base.GetComponent<Harvestable>();
			this.uprootMonitor.customFoundationCheckFn = new Func<int, bool>(this.IsCellFoundation);
			this.SetCellRegistrationAsPlant(true);
			base.Subscribe(1119167081, new Action<object>(this.OnSpawnedByDiscovery));
		}

		public override void StartSM()
		{
			this.wasMarkedForDeadBeforeStartSM = base.sm.MarkedForDeath.Get(this);
			base.master.gameObject.AddTag(GameTags.GrowingPlant);
			base.StartSM();
			this.SetAnimOrientation(this.MyShape, this.IsGrowingClockwise);
			this.Schedule(1f, new Action<object>(this.DelayedResetUprootMonitor), null);
		}

		public override void PostParamsInitialized()
		{
			base.PostParamsInitialized();
			this.MotherSMI = ((this.Mother == null) ? null : this.Mother.GetSMI<VineMother.Instance>());
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
			float num = 1f - (float)this.MyBranchNumber / 12f;
			float num2 = ((global::UnityEngine.Random.Range(0f, 1f) <= num) ? 1f : global::UnityEngine.Random.Range(0f, 1f));
			this.maturity.SetValue(this.maturity.maxAttribute.GetTotalValue() * num2);
			if (this.IsGrown)
			{
				this.IsNewGameSpawned = true;
				this.fruitMaturity.SetValue(this.fruitMaturity.maxAttribute.GetTotalValue() * global::UnityEngine.Random.Range(0f, 1f));
			}
		}

		public void ResetFruitGrowProgress()
		{
			this.fruitMaturity.SetValue(0f);
		}

		public void SpawnHarvestedFruit()
		{
			base.GetComponent<Crop>().SpawnConfiguredFruit(null);
		}

		public void HideAllFruitSymbols()
		{
			foreach (VineBranch.ShapeCategory shapeCategory in VineBranch.GetAnimSetByShapeCategory.Keys)
			{
				VineBranch.AnimSet animSet = VineBranch.GetAnimSetByShapeCategory[shapeCategory];
				this.AnimController.SetSymbolVisiblity(animSet.meter_target, false);
			}
		}

		public void CreateFruitMeter()
		{
			this.DestroyFruitMeter();
			this.fruitMeter = new MeterController(this.AnimController, this.Anims.meter_target, this.Anims.meter, Meter.Offset.NoChange, Grid.SceneLayer.Building, Array.Empty<string>());
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
				if (this.fruitMeter.meterController.currentAnim != this.Anims.meter)
				{
					this.PlayAnimOnFruitMeter(this.Anims.meter, KAnim.PlayMode.Paused);
				}
				this.fruitMeter.SetPositionPercent(this.FruitGrowthPercentage);
			}
		}

		private void KillForwardBranch()
		{
			if (this.Branch != null)
			{
				VineBranch.Instance smi = this.Branch.GetSMI<VineBranch.Instance>();
				if (smi != null)
				{
					smi.sm.DieSignal.Trigger(smi);
					smi.sm.MarkedForDeath.Set(true, smi, false);
				}
				base.sm.Branch.Set(null, this);
			}
		}

		public void SetupRootInformation(VineMother.Instance mother)
		{
			CellOffset cellOffsetDirection = Grid.GetCellOffsetDirection(Grid.PosToCell(this), Grid.PosToCell(mother));
			Direction direction = ((cellOffsetDirection == CellOffset.left) ? Direction.Left : ((cellOffsetDirection == CellOffset.right) ? Direction.Right : ((cellOffsetDirection == CellOffset.up) ? Direction.Up : Direction.Down)));
			base.sm.RootDirection.Set((int)direction, this, false);
			base.sm.RootShape.Set(1, this, false);
			base.sm.BranchNumber.Set(1, this, false);
			base.sm.WildPlanted.Set(mother.IsWild, this, false);
			base.sm.Mother.Set(mother.gameObject, this, false);
			this.MotherSMI = ((this.Mother == null) ? null : this.Mother.GetSMI<VineMother.Instance>());
			HarvestDesignatable component = mother.GetComponent<HarvestDesignatable>();
			base.GetComponent<HarvestDesignatable>().SetHarvestWhenReady(component.HarvestWhenReady);
		}

		public void SetupRootInformation(VineBranch.Instance root)
		{
			CellOffset cellOffsetDirection = Grid.GetCellOffsetDirection(Grid.PosToCell(this), Grid.PosToCell(root));
			Direction direction = ((cellOffsetDirection == CellOffset.left) ? Direction.Left : ((cellOffsetDirection == CellOffset.right) ? Direction.Right : ((cellOffsetDirection == CellOffset.up) ? Direction.Up : Direction.Down)));
			base.sm.RootDirection.Set((int)direction, this, false);
			base.sm.RootShape.Set((int)root.MyShape, this, false);
			base.sm.BranchNumber.Set(root.MyBranchNumber + 1, this, false);
			base.sm.WildPlanted.Set(root.IsWild, this, false);
			base.sm.Mother.Set(root.Mother, this, false);
			this.MotherSMI = ((this.Mother == null) ? null : this.Mother.GetSMI<VineMother.Instance>());
			HarvestDesignatable component = root.GetComponent<HarvestDesignatable>();
			base.GetComponent<HarvestDesignatable>().SetHarvestWhenReady(component.HarvestWhenReady);
		}

		public void AttemptToSpawnBranch()
		{
			if (this.CanSpawnBranch())
			{
				this.isSpawningNextBranch = true;
				int cellToSpawnBranch = this.GetCellToSpawnBranch();
				GameObject gameObject = this.SpawnBranchOnCell(cellToSpawnBranch);
				base.sm.Branch.Set(gameObject, this, false);
				this.isSpawningNextBranch = false;
				if (this.IsNewGameSpawned)
				{
					gameObject.Trigger(1119167081, null);
				}
				this.ResetUprootMonitor();
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
			gameObject.GetSMI<VineBranch.Instance>().SetupRootInformation(this);
			return gameObject;
		}

		private bool IsCellFoundation(int cell)
		{
			return VineBranch.IsCellFoundation(cell) || (this.MotherSMI.IsOnPlanterBox && this.MotherSMI.PlanterboxCell == cell);
		}

		private bool IsCellAvailable(int cell)
		{
			bool flag = VineBranch.IsCellAvailable(base.gameObject, cell, new Func<int, bool>(this.IsCellFoundation));
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
			int num = Grid.PosToCell(base.gameObject);
			switch (this.MyShape)
			{
			case VineBranch.Shape.Top:
			case VineBranch.Shape.Bottom:
				if (this.RootDirection != Direction.Left)
				{
					return Grid.OffsetCell(num, -1, 0);
				}
				return Grid.OffsetCell(num, 1, 0);
			case VineBranch.Shape.Left:
			case VineBranch.Shape.Right:
				if (this.RootDirection != Direction.Up)
				{
					return Grid.OffsetCell(num, 0, 1);
				}
				return Grid.OffsetCell(num, 0, -1);
			case VineBranch.Shape.InCornerTopLeft:
				if (this.RootDirection != Direction.Down)
				{
					return Grid.OffsetCell(num, 0, -1);
				}
				return Grid.OffsetCell(num, 1, 0);
			case VineBranch.Shape.InCornerTopRight:
				if (this.RootDirection != Direction.Down)
				{
					return Grid.OffsetCell(num, 0, -1);
				}
				return Grid.OffsetCell(num, -1, 0);
			case VineBranch.Shape.InCornerBottomLeft:
				if (this.RootDirection != Direction.Up)
				{
					return Grid.OffsetCell(num, 0, 1);
				}
				return Grid.OffsetCell(num, 1, 0);
			case VineBranch.Shape.InCornerBottomRight:
				if (this.RootDirection != Direction.Up)
				{
					return Grid.OffsetCell(num, 0, 1);
				}
				return Grid.OffsetCell(num, -1, 0);
			case VineBranch.Shape.OutCornerTopLeft:
				if (this.RootDirection != Direction.Up)
				{
					return Grid.OffsetCell(num, 0, 1);
				}
				return Grid.OffsetCell(num, -1, 0);
			case VineBranch.Shape.OutCornerTopRight:
				if (this.RootDirection != Direction.Up)
				{
					return Grid.OffsetCell(num, 0, 1);
				}
				return Grid.OffsetCell(num, 1, 0);
			case VineBranch.Shape.OutCornerBottomLeft:
				if (this.RootDirection != Direction.Down)
				{
					return Grid.OffsetCell(num, 0, -1);
				}
				return Grid.OffsetCell(num, -1, 0);
			case VineBranch.Shape.OutCornerBottomRight:
				if (this.RootDirection != Direction.Down)
				{
					return Grid.OffsetCell(num, 0, -1);
				}
				return Grid.OffsetCell(num, 1, 0);
			default:
				return Grid.InvalidCell;
			}
		}

		public void SubscribeSurroundingSolidChangesListeners()
		{
			KPrefabID component = base.gameObject.GetComponent<KPrefabID>();
			this.UnSubscribreSurroundingSolidChangesListeners();
			CellOffset[] array = new CellOffset[]
			{
				new CellOffset(-1, -1),
				new CellOffset(0, -1),
				new CellOffset(1, -1),
				new CellOffset(-1, 0),
				new CellOffset(1, 0),
				new CellOffset(-1, 1),
				new CellOffset(0, 1),
				new CellOffset(1, 1)
			};
			Extents extents = new Extents(Grid.PosToCell(base.gameObject), array);
			this.solidPartitionerEntry = GameScenePartitioner.Instance.Add("VineBranchSurroundingListenerSolids", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSurroundingCellsBlockageChangedDetected));
			this.buildingsPartitionerEntry = GameScenePartitioner.Instance.Add("VineBranchSurroundingListenerBuildings", base.gameObject, extents, GameScenePartitioner.Instance.objectLayers[1], new Action<object>(this.OnSurroundingCellsBlockageChangedDetected));
			this.plantsPartitionerEntry = GameScenePartitioner.Instance.Add("VineBranchSurroundingListenerPlants", component, extents, GameScenePartitioner.Instance.plantsChangedLayer, new Action<object>(this.OnSurroundingCellsBlockageChangedDetected));
			this.liquidsPartitionerEntry = GameScenePartitioner.Instance.Add("VineBranchSurroundingListenerLiquids", base.gameObject, extents, GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(this.OnSurroundingCellsBlockageChangedDetected));
		}

		public void UnSubscribreSurroundingSolidChangesListeners()
		{
			GameScenePartitioner.Instance.Free(ref this.solidPartitionerEntry);
			GameScenePartitioner.Instance.Free(ref this.buildingsPartitionerEntry);
			GameScenePartitioner.Instance.Free(ref this.plantsPartitionerEntry);
			GameScenePartitioner.Instance.Free(ref this.liquidsPartitionerEntry);
			this.solidPartitionerEntry = HandleVector<int>.InvalidHandle;
			this.buildingsPartitionerEntry = HandleVector<int>.InvalidHandle;
			this.plantsPartitionerEntry = HandleVector<int>.InvalidHandle;
			this.liquidsPartitionerEntry = HandleVector<int>.InvalidHandle;
		}

		private void OnSurroundingCellsBlockageChangedDetected(object o)
		{
			if (this.CanChangeShape)
			{
				this.RecalculateMyShape();
			}
		}

		private void SetShape(VineBranch.Shape shape, bool clockwise)
		{
			base.sm.BranchShape.Set((int)shape, this, false);
			base.sm.GrowingClockwise.Set(clockwise, this, false);
			this.SetAnimOrientation(shape, clockwise);
			base.Trigger(838747413, null);
		}

		public void RecalculateMyShape()
		{
			VineBranch.Shape shape = VineBranch.Shape.TopEnd;
			bool flag = false;
			switch (this.RootDirection)
			{
			case Direction.Up:
				switch (base.smi.RootShape)
				{
				case VineBranch.Shape.Left:
				case VineBranch.Shape.InCornerTopLeft:
				case VineBranch.Shape.OutCornerBottomLeft:
					shape = this.ChooseCompatibleShape(new VineBranch.Shape[]
					{
						VineBranch.Shape.BottomEnd,
						VineBranch.Shape.InCornerBottomLeft,
						VineBranch.Shape.OutCornerTopLeft,
						VineBranch.Shape.Left
					});
					flag = shape == VineBranch.Shape.OutCornerTopLeft;
					break;
				case VineBranch.Shape.Right:
				case VineBranch.Shape.InCornerTopRight:
				case VineBranch.Shape.OutCornerBottomRight:
					shape = this.ChooseCompatibleShape(new VineBranch.Shape[]
					{
						VineBranch.Shape.BottomEnd,
						VineBranch.Shape.InCornerBottomRight,
						VineBranch.Shape.OutCornerTopRight,
						VineBranch.Shape.Right
					});
					flag = shape != VineBranch.Shape.OutCornerTopRight;
					break;
				}
				break;
			case Direction.Right:
				switch (base.smi.RootShape)
				{
				case VineBranch.Shape.Top:
				case VineBranch.Shape.InCornerTopRight:
				case VineBranch.Shape.OutCornerTopLeft:
					shape = this.ChooseCompatibleShape(new VineBranch.Shape[]
					{
						VineBranch.Shape.LeftEnd,
						VineBranch.Shape.InCornerTopLeft,
						VineBranch.Shape.OutCornerTopRight,
						VineBranch.Shape.Top
					});
					flag = shape == VineBranch.Shape.OutCornerTopRight;
					break;
				case VineBranch.Shape.Bottom:
				case VineBranch.Shape.InCornerBottomRight:
				case VineBranch.Shape.OutCornerBottomLeft:
					shape = this.ChooseCompatibleShape(new VineBranch.Shape[]
					{
						VineBranch.Shape.LeftEnd,
						VineBranch.Shape.InCornerBottomLeft,
						VineBranch.Shape.OutCornerBottomRight,
						VineBranch.Shape.Bottom
					});
					flag = shape != VineBranch.Shape.OutCornerBottomRight;
					break;
				}
				break;
			case Direction.Down:
				switch (base.smi.RootShape)
				{
				case VineBranch.Shape.Left:
				case VineBranch.Shape.InCornerBottomLeft:
				case VineBranch.Shape.OutCornerTopLeft:
					shape = this.ChooseCompatibleShape(new VineBranch.Shape[]
					{
						VineBranch.Shape.TopEnd,
						VineBranch.Shape.InCornerTopLeft,
						VineBranch.Shape.OutCornerBottomLeft,
						VineBranch.Shape.Left
					});
					flag = shape != VineBranch.Shape.OutCornerBottomLeft;
					break;
				case VineBranch.Shape.Right:
				case VineBranch.Shape.InCornerBottomRight:
				case VineBranch.Shape.OutCornerTopRight:
					shape = this.ChooseCompatibleShape(new VineBranch.Shape[]
					{
						VineBranch.Shape.TopEnd,
						VineBranch.Shape.InCornerTopRight,
						VineBranch.Shape.OutCornerBottomRight,
						VineBranch.Shape.Right
					});
					flag = shape == VineBranch.Shape.OutCornerBottomRight;
					break;
				}
				break;
			case Direction.Left:
			{
				VineBranch.Shape rootShape = this.RootShape;
				switch (rootShape)
				{
				case VineBranch.Shape.Top:
				case VineBranch.Shape.InCornerTopLeft:
					break;
				case VineBranch.Shape.Bottom:
				case VineBranch.Shape.InCornerBottomLeft:
					goto IL_0084;
				case VineBranch.Shape.Left:
				case VineBranch.Shape.Right:
				case VineBranch.Shape.InCornerTopRight:
					goto IL_0230;
				default:
					if (rootShape != VineBranch.Shape.OutCornerTopRight)
					{
						if (rootShape != VineBranch.Shape.OutCornerBottomRight)
						{
							goto IL_0230;
						}
						goto IL_0084;
					}
					break;
				}
				shape = this.ChooseCompatibleShape(new VineBranch.Shape[]
				{
					VineBranch.Shape.RightEnd,
					VineBranch.Shape.InCornerTopRight,
					VineBranch.Shape.OutCornerTopLeft,
					VineBranch.Shape.Top
				});
				flag = shape != VineBranch.Shape.OutCornerTopLeft;
				break;
				IL_0084:
				shape = this.ChooseCompatibleShape(new VineBranch.Shape[]
				{
					VineBranch.Shape.RightEnd,
					VineBranch.Shape.InCornerBottomRight,
					VineBranch.Shape.OutCornerBottomLeft,
					VineBranch.Shape.Bottom
				});
				flag = shape == VineBranch.Shape.OutCornerBottomLeft;
				break;
			}
			}
			IL_0230:
			base.smi.SetShape(shape, flag);
		}

		private void SetAnimOrientation(VineBranch.Shape shape, bool clockwise)
		{
			this.AnimController.FlipX = false;
			this.AnimController.FlipY = false;
			this.AnimController.Rotation = 0f;
			switch (shape)
			{
			case VineBranch.Shape.Top:
				this.AnimController.FlipY = clockwise;
				this.AnimController.Rotation = (float)(clockwise ? 0 : 180);
				break;
			case VineBranch.Shape.Bottom:
				this.AnimController.FlipX = clockwise;
				break;
			case VineBranch.Shape.Left:
				this.AnimController.FlipX = clockwise;
				this.AnimController.Rotation = 90f;
				break;
			case VineBranch.Shape.Right:
				this.AnimController.FlipX = clockwise;
				this.AnimController.Rotation = 270f;
				break;
			case VineBranch.Shape.InCornerTopLeft:
				this.AnimController.FlipX = clockwise;
				this.AnimController.Rotation = (float)(clockwise ? 90 : 180);
				break;
			case VineBranch.Shape.InCornerTopRight:
				this.AnimController.FlipY = clockwise;
				this.AnimController.Rotation = (float)(clockwise ? 0 : 270);
				break;
			case VineBranch.Shape.InCornerBottomLeft:
				this.AnimController.FlipX = clockwise;
				this.AnimController.Rotation = (float)(clockwise ? 0 : 90);
				break;
			case VineBranch.Shape.InCornerBottomRight:
				this.AnimController.FlipY = clockwise;
				this.AnimController.Rotation = (float)(clockwise ? 90 : 0);
				break;
			case VineBranch.Shape.OutCornerTopLeft:
				this.AnimController.FlipY = clockwise;
				this.AnimController.Rotation = (float)(clockwise ? 90 : 0);
				break;
			case VineBranch.Shape.OutCornerTopRight:
				this.AnimController.FlipX = clockwise;
				this.AnimController.Rotation = (float)(clockwise ? 0 : 90);
				break;
			case VineBranch.Shape.OutCornerBottomLeft:
				this.AnimController.FlipY = clockwise;
				this.AnimController.Rotation = (float)(clockwise ? 0 : 270);
				break;
			case VineBranch.Shape.OutCornerBottomRight:
				this.AnimController.FlipX = clockwise;
				this.AnimController.Rotation = (float)(clockwise ? 90 : 180);
				break;
			case VineBranch.Shape.TopEnd:
				this.AnimController.FlipX = clockwise;
				this.AnimController.Rotation = (float)(clockwise ? 90 : 270);
				break;
			case VineBranch.Shape.BottomEnd:
				this.AnimController.FlipX = clockwise;
				this.AnimController.Rotation = (float)(clockwise ? 270 : 90);
				break;
			case VineBranch.Shape.LeftEnd:
				this.AnimController.FlipX = clockwise;
				this.AnimController.Rotation = (float)(clockwise ? 0 : 180);
				break;
			case VineBranch.Shape.RightEnd:
				this.AnimController.FlipX = clockwise;
				this.AnimController.Rotation = (float)(clockwise ? 180 : 0);
				break;
			}
			this.AnimController.Rotation *= -1f;
			this.AnimController.Offset = new Vector3(0f, (float)(this.AnimController.FlipY ? 1 : 0), 0f);
			bool flag = this.AnimController.FlipY && Mathf.Abs(this.AnimController.Rotation) == 90f;
			this.AnimController.Pivot = new Vector3(0f, flag ? (-0.5f) : 0.5f, 0f);
		}

		private VineBranch.Shape ChooseCompatibleShape(VineBranch.Shape[] possibleShapesOrderedByPriority)
		{
			bool flag = false;
			VineBranch.Shape shape = VineBranch.Shape.TopEnd;
			int num = Grid.PosToCell(base.gameObject);
			int num2 = Grid.OffsetCell(num, -1, 0);
			int num3 = Grid.OffsetCell(num, 1, 0);
			int num4 = Grid.OffsetCell(num, 0, 1);
			int num5 = Grid.OffsetCell(num, 0, -1);
			int num6 = Grid.OffsetCell(num, -1, 1);
			int num7 = Grid.OffsetCell(num, 1, 1);
			int num8 = Grid.OffsetCell(num, -1, -1);
			int num9 = Grid.OffsetCell(num, 1, -1);
			foreach (VineBranch.Shape shape2 in possibleShapesOrderedByPriority)
			{
				VineBranch.ShapeCategory shapeCategory = VineBranch.GetShapeCategory[shape2];
				if (shapeCategory == VineBranch.ShapeCategory.DeadEnd)
				{
					shape = shape2;
				}
				if (!this.MaxBranchNumberReached || shapeCategory == VineBranch.ShapeCategory.DeadEnd)
				{
					switch (shape2)
					{
					case VineBranch.Shape.Top:
						flag = this.IsCellFoundation(num4) && (this.IsCellAvailable(num2) || this.IsCellAvailable(num3));
						break;
					case VineBranch.Shape.Bottom:
						flag = this.IsCellFoundation(num5) && (this.IsCellAvailable(num2) || this.IsCellAvailable(num3));
						break;
					case VineBranch.Shape.Left:
						flag = this.IsCellFoundation(num2) && (this.IsCellAvailable(num4) || this.IsCellAvailable(num5));
						break;
					case VineBranch.Shape.Right:
						flag = this.IsCellFoundation(num3) && (this.IsCellAvailable(num4) || this.IsCellAvailable(num5));
						break;
					case VineBranch.Shape.InCornerTopLeft:
						flag = this.IsCellFoundation(num4) && this.IsCellFoundation(num2);
						break;
					case VineBranch.Shape.InCornerTopRight:
						flag = this.IsCellFoundation(num4) && this.IsCellFoundation(num3);
						break;
					case VineBranch.Shape.InCornerBottomLeft:
						flag = this.IsCellFoundation(num5) && this.IsCellFoundation(num2);
						break;
					case VineBranch.Shape.InCornerBottomRight:
						flag = this.IsCellFoundation(num5) && this.IsCellFoundation(num3);
						break;
					case VineBranch.Shape.OutCornerTopLeft:
						flag = (this.IsCellAvailable(num4) || this.IsCellAvailable(num2)) && this.IsCellFoundation(num6);
						break;
					case VineBranch.Shape.OutCornerTopRight:
						flag = (this.IsCellAvailable(num4) || this.IsCellAvailable(num3)) && this.IsCellFoundation(num7);
						break;
					case VineBranch.Shape.OutCornerBottomLeft:
						flag = (this.IsCellAvailable(num5) || this.IsCellAvailable(num2)) && this.IsCellFoundation(num8);
						break;
					case VineBranch.Shape.OutCornerBottomRight:
						flag = (this.IsCellAvailable(num5) || this.IsCellAvailable(num3)) && this.IsCellFoundation(num9);
						break;
					case VineBranch.Shape.TopEnd:
						flag = !this.IsCellAvailable(num2) && !this.IsCellAvailable(num4) && !this.IsCellAvailable(num3);
						break;
					case VineBranch.Shape.BottomEnd:
						flag = !this.IsCellAvailable(num2) && !this.IsCellAvailable(num5) && !this.IsCellAvailable(num3);
						break;
					case VineBranch.Shape.LeftEnd:
						flag = !this.IsCellAvailable(num4) && !this.IsCellAvailable(num5) && !this.IsCellAvailable(num2);
						break;
					case VineBranch.Shape.RightEnd:
						flag = !this.IsCellAvailable(num4) && !this.IsCellAvailable(num5) && !this.IsCellAvailable(num3);
						break;
					}
					if (flag)
					{
						return shape2;
					}
				}
			}
			return shape;
		}

		private void DelayedResetUprootMonitor(object o)
		{
			this.ResetUprootMonitor();
		}

		public void ResetUprootMonitor()
		{
			CellOffset[] array = new CellOffset[0];
			if (!this.CanChangeShape && !this.MaxBranchNumberReached)
			{
				switch (this.MyShape)
				{
				case VineBranch.Shape.Top:
					array = new CellOffset[] { CellOffset.up };
					break;
				case VineBranch.Shape.Bottom:
					array = new CellOffset[] { CellOffset.down };
					break;
				case VineBranch.Shape.Left:
					array = new CellOffset[] { CellOffset.left };
					break;
				case VineBranch.Shape.Right:
					array = new CellOffset[] { CellOffset.right };
					break;
				case VineBranch.Shape.InCornerTopLeft:
					array = new CellOffset[]
					{
						CellOffset.up,
						CellOffset.left
					};
					break;
				case VineBranch.Shape.InCornerTopRight:
					array = new CellOffset[]
					{
						CellOffset.up,
						CellOffset.right
					};
					break;
				case VineBranch.Shape.InCornerBottomLeft:
					array = new CellOffset[]
					{
						CellOffset.down,
						CellOffset.left
					};
					break;
				case VineBranch.Shape.InCornerBottomRight:
					array = new CellOffset[]
					{
						CellOffset.down,
						CellOffset.right
					};
					break;
				case VineBranch.Shape.OutCornerTopLeft:
					array = new CellOffset[]
					{
						new CellOffset(-1, 1)
					};
					break;
				case VineBranch.Shape.OutCornerTopRight:
					array = new CellOffset[]
					{
						new CellOffset(1, 1)
					};
					break;
				case VineBranch.Shape.OutCornerBottomLeft:
					array = new CellOffset[]
					{
						new CellOffset(-1, -1)
					};
					break;
				case VineBranch.Shape.OutCornerBottomRight:
					array = new CellOffset[]
					{
						new CellOffset(1, -1)
					};
					break;
				case VineBranch.Shape.TopEnd:
					array = new CellOffset[] { this.IsGrowingClockwise ? CellOffset.left : CellOffset.right };
					break;
				case VineBranch.Shape.BottomEnd:
					array = new CellOffset[] { this.IsGrowingClockwise ? CellOffset.right : CellOffset.left };
					break;
				case VineBranch.Shape.LeftEnd:
					array = new CellOffset[] { this.IsGrowingClockwise ? CellOffset.down : CellOffset.up };
					break;
				case VineBranch.Shape.RightEnd:
					array = new CellOffset[] { this.IsGrowingClockwise ? CellOffset.up : CellOffset.down };
					break;
				}
			}
			this.uprootMonitor.SetNewMonitorCells(array);
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
				return "    • " + DUPLICANTS.STATS.VINEMOTHERHEALTH.NAME;
			}
		}

		public WiltCondition.Condition[] Conditions
		{
			get
			{
				return new WiltCondition.Condition[] { WiltCondition.Condition.UnhealthyRoot };
			}
		}

		private bool isSpawningNextBranch;

		public bool IsNewGameSpawned;

		public AttributeModifier baseGrowingRate;

		public AttributeModifier wildGrowingRate;

		public AttributeModifier baseFruitGrowingRate;

		public AttributeModifier wildFruitGrowingRate;

		public AttributeModifier getOldRate;

		public KBatchedAnimController AnimController;

		private AmountInstance maturity;

		private AmountInstance fruitMaturity;

		private AmountInstance oldAge;

		private WiltCondition wiltCondition;

		private VineMother.Instance MotherSMI;

		private UprootedMonitor uprootMonitor;

		private Harvestable harvestable;

		private MeterController fruitMeter;

		private HandleVector<int>.Handle solidPartitionerEntry = HandleVector<int>.InvalidHandle;

		private HandleVector<int>.Handle buildingsPartitionerEntry = HandleVector<int>.InvalidHandle;

		private HandleVector<int>.Handle plantsPartitionerEntry = HandleVector<int>.InvalidHandle;

		private HandleVector<int>.Handle liquidsPartitionerEntry = HandleVector<int>.InvalidHandle;

		private bool wasMarkedForDeadBeforeStartSM;
	}
}
