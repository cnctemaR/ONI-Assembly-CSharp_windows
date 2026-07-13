using System;
using System.Collections.Generic;
using UnityEngine;

public class ClusterMapRocketAnimator : GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer>
{
	public override void InitializeStates(out StateMachine.BaseState defaultState)
	{
		defaultState = this.idle;
		this.root.Enter(new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State.Callback(ClusterMapRocketAnimator.RefreshDrillConeSymbol)).Transition(null, new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.Transition.ConditionCallback(this.entityTarget.IsNull), UpdateRate.SIM_200ms).Target(this.entityTarget)
			.EventHandlerTransition(GameHashes.RocketSelfDestructRequested, this.exploding, (ClusterMapRocketAnimator.StatesInstance smi, object data) => true)
			.TagTransition(GameTags.RocketCollectingResources, this.utility.collecting, false)
			.EventHandlerTransition(GameHashes.RocketLaunched, this.moving.takeoff, (ClusterMapRocketAnimator.StatesInstance smi, object data) => true);
		this.idle.Enter(new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State.Callback(ClusterMapRocketAnimator.RefreshDillingAnimations)).Target(this.masterTarget).Enter(delegate(ClusterMapRocketAnimator.StatesInstance smi)
		{
			smi.PlayVisAnim("idle_loop", KAnim.PlayMode.Loop);
		})
			.Target(this.entityTarget)
			.EventHandler(GameHashes.TagsChanged, new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State.Callback(ClusterMapRocketAnimator.RefreshDillingAnimations))
			.Transition(this.moving.traveling, new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.Transition.ConditionCallback(this.IsTraveling), UpdateRate.SIM_200ms)
			.Transition(this.grounded, new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.Transition.ConditionCallback(this.IsGrounded), UpdateRate.SIM_200ms)
			.Transition(this.moving.landing, new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.Transition.ConditionCallback(this.IsLanding), UpdateRate.SIM_200ms)
			.Transition(this.utility.collecting, new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.Transition.ConditionCallback(this.IsCollectingResourcesFromHexCell), UpdateRate.SIM_200ms);
		this.grounded.Enter(delegate(ClusterMapRocketAnimator.StatesInstance smi)
		{
			this.ToggleSelectable(false, smi);
			smi.ToggleVisAnim(false);
		}).Exit(delegate(ClusterMapRocketAnimator.StatesInstance smi)
		{
			this.ToggleSelectable(true, smi);
			smi.ToggleVisAnim(true);
			ClusterMapRocketAnimator.RefreshDrillConeSymbol(smi);
		}).Target(this.entityTarget)
			.EventTransition(GameHashes.RocketLaunched, this.moving.takeoff, null);
		this.moving.takeoff.Transition(this.idle, GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.Not(new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.Transition.ConditionCallback(this.IsSurfaceTransitioning)), UpdateRate.SIM_200ms).Enter(delegate(ClusterMapRocketAnimator.StatesInstance smi)
		{
			smi.PlayVisAnim("launching", KAnim.PlayMode.Loop);
			this.ToggleSelectable(false, smi);
		}).Exit(delegate(ClusterMapRocketAnimator.StatesInstance smi)
		{
			this.ToggleSelectable(true, smi);
		});
		this.moving.Enter(new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State.Callback(ClusterMapRocketAnimator.RefreshDillingAnimations)).Target(this.entityTarget).EventHandler(GameHashes.TagsChanged, new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State.Callback(ClusterMapRocketAnimator.RefreshDillingAnimations));
		this.moving.landing.Transition(this.idle, GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.Not(new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.Transition.ConditionCallback(this.IsSurfaceTransitioning)), UpdateRate.SIM_200ms).Enter(delegate(ClusterMapRocketAnimator.StatesInstance smi)
		{
			smi.PlayVisAnim("landing", KAnim.PlayMode.Loop);
			this.ToggleSelectable(false, smi);
		}).Exit(delegate(ClusterMapRocketAnimator.StatesInstance smi)
		{
			this.ToggleSelectable(true, smi);
		});
		this.moving.traveling.DefaultState(this.moving.traveling.regular).Target(this.entityTarget).EventTransition(GameHashes.ClusterLocationChanged, this.idle, GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.Not(new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.Transition.ConditionCallback(this.IsTraveling)))
			.EventTransition(GameHashes.ClusterDestinationChanged, this.idle, GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.Not(new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.Transition.ConditionCallback(this.IsTraveling)));
		this.moving.traveling.regular.Target(this.entityTarget).Transition(this.moving.traveling.boosted, new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.Transition.ConditionCallback(this.IsBoosted), UpdateRate.SIM_200ms).Enter(delegate(ClusterMapRocketAnimator.StatesInstance smi)
		{
			smi.PlayVisAnim("inflight_loop", KAnim.PlayMode.Loop);
		});
		this.moving.traveling.boosted.Target(this.entityTarget).Transition(this.moving.traveling.regular, GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.Not(new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.Transition.ConditionCallback(this.IsBoosted)), UpdateRate.SIM_200ms).Enter(delegate(ClusterMapRocketAnimator.StatesInstance smi)
		{
			smi.PlayVisAnim("boosted", KAnim.PlayMode.Loop);
		});
		this.utility.Target(this.entityTarget).EventTransition(GameHashes.ClusterDestinationChanged, this.idle, new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.Transition.ConditionCallback(this.IsTraveling)).EventHandler(GameHashes.TagsChanged, new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State.Callback(ClusterMapRocketAnimator.RefreshDillingAnimations))
			.Enter(new StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State.Callback(ClusterMapRocketAnimator.RefreshDillingAnimations));
		this.utility.collecting.DefaultState(this.utility.collecting.pre).Target(this.entityTarget).TagTransition(GameTags.RocketCollectingResources, this.utility.collecting.pst, true)
			.ToggleStatusItem(Db.Get().BuildingStatusItems.CollectingHexCellInventoryItems, null);
		this.utility.collecting.pre.Enter(delegate(ClusterMapRocketAnimator.StatesInstance smi)
		{
			smi.PlayVisAnim("mining_pre", KAnim.PlayMode.Once);
			smi.SubscribeOnVisAnimComplete(delegate(object data)
			{
				smi.GoTo(this.utility.collecting.loop);
			});
		});
		this.utility.collecting.loop.Enter(delegate(ClusterMapRocketAnimator.StatesInstance smi)
		{
			smi.PlayVisAnim("mining_loop", KAnim.PlayMode.Loop);
		});
		this.utility.collecting.pst.Enter(delegate(ClusterMapRocketAnimator.StatesInstance smi)
		{
			smi.PlayVisAnim("mining_pst", KAnim.PlayMode.Once);
			smi.SubscribeOnVisAnimComplete(delegate(object data)
			{
				smi.GoTo(this.idle);
			});
		});
		this.exploding.Enter(delegate(ClusterMapRocketAnimator.StatesInstance smi)
		{
			smi.GetComponent<ClusterMapVisualizer>().GetFirstAnimController().SwapAnims(new KAnimFile[] { Assets.GetAnim("rocket_self_destruct_kanim") });
			smi.PlayVisAnim("explode", KAnim.PlayMode.Once);
			smi.SubscribeOnVisAnimComplete(delegate(object data)
			{
				smi.GoTo(this.exploding_pst);
			});
		});
		this.exploding_pst.Enter(delegate(ClusterMapRocketAnimator.StatesInstance smi)
		{
			smi.GetComponent<ClusterMapVisualizer>().GetFirstAnimController().Stop();
			smi.entity.gameObject.Trigger(-1311384361, null);
		});
	}

	private static void RefreshDillingAnimations(ClusterMapRocketAnimator.StatesInstance smi)
	{
		if (ClusterMapRocketAnimator.IsDrilling(smi))
		{
			smi.PlayDrillingAnimation();
			return;
		}
		smi.PlayIdleDrillConeAnimation();
	}

	private static void RefreshDrillConeSymbol(ClusterMapRocketAnimator.StatesInstance smi)
	{
		smi.RefreshDrillConeVisibility();
	}

	private bool ClusterChangedAtMyLocation(ClusterMapRocketAnimator.StatesInstance smi, object data)
	{
		ClusterLocationChangedEvent clusterLocationChangedEvent = (ClusterLocationChangedEvent)data;
		return clusterLocationChangedEvent.oldLocation == smi.entity.Location || clusterLocationChangedEvent.newLocation == smi.entity.Location;
	}

	private bool IsTraveling(ClusterMapRocketAnimator.StatesInstance smi)
	{
		return smi.entity.GetComponent<ClusterTraveler>().IsTraveling() && ((Clustercraft)smi.entity).HasResourcesToMove(1, Clustercraft.CombustionResource.All);
	}

	private bool IsBoosted(ClusterMapRocketAnimator.StatesInstance smi)
	{
		return ((Clustercraft)smi.entity).controlStationBuffTimeRemaining > 0f;
	}

	private bool IsGrounded(ClusterMapRocketAnimator.StatesInstance smi)
	{
		return ((Clustercraft)smi.entity).Status == Clustercraft.CraftStatus.Grounded;
	}

	private bool IsLanding(ClusterMapRocketAnimator.StatesInstance smi)
	{
		return ((Clustercraft)smi.entity).Status == Clustercraft.CraftStatus.Landing;
	}

	private static bool IsDrilling(ClusterMapRocketAnimator.StatesInstance smi)
	{
		return ((Clustercraft)smi.entity).HasTag(GameTags.RocketDrilling);
	}

	private bool IsCollectingResourcesFromHexCell(ClusterMapRocketAnimator.StatesInstance smi)
	{
		return ((Clustercraft)smi.entity).HasTag(GameTags.RocketCollectingResources);
	}

	private bool IsSurfaceTransitioning(ClusterMapRocketAnimator.StatesInstance smi)
	{
		Clustercraft clustercraft = smi.entity as Clustercraft;
		return clustercraft != null && (clustercraft.Status == Clustercraft.CraftStatus.Landing || clustercraft.Status == Clustercraft.CraftStatus.Launching);
	}

	private void ToggleSelectable(bool isSelectable, ClusterMapRocketAnimator.StatesInstance smi)
	{
		if (smi.entity.IsNullOrDestroyed())
		{
			return;
		}
		KSelectable component = smi.entity.GetComponent<KSelectable>();
		component.IsSelectable = isSelectable;
		if (!isSelectable && component.IsSelected && ClusterMapScreen.Instance.GetMode() != ClusterMapScreen.Mode.SelectDestination)
		{
			ClusterMapSelectTool.Instance.Select(null, true);
			SelectTool.Instance.Select(null, true);
		}
	}

	public StateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.TargetParameter entityTarget;

	public GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State idle;

	public GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State grounded;

	public ClusterMapRocketAnimator.MovingStates moving;

	public ClusterMapRocketAnimator.UtilityStates utility;

	public GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State exploding;

	public GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State exploding_pst;

	public const string DRILLCONE_METER_TARGET_NAME = "nose_target";

	public const string DRILLCONE_DEFAULT_ANIM_NAME = "drill_cone_idle";

	public const string DRILLCONE_DRILL_ANIM_NAME = "drilling_loop";

	public class TravelingStates : GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State
	{
		public GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State regular;

		public GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State boosted;
	}

	public class MovingStates : GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State
	{
		public GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State takeoff;

		public ClusterMapRocketAnimator.TravelingStates traveling;

		public GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State landing;
	}

	public class UtilityStates : GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State
	{
		public ClusterMapRocketAnimator.UtilityStates.CollectingStates collecting;

		public class CollectingStates : GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State
		{
			public GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State pre;

			public GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State loop;

			public GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.State pst;
		}
	}

	public class StatesInstance : GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer, object>.GameInstance
	{
		public StatesInstance(ClusterMapVisualizer master, ClusterGridEntity entity)
			: base(master)
		{
			this.entity = entity;
			base.sm.entityTarget.Set(entity, this);
		}

		public override void StartSM()
		{
			base.GetComponent<ClusterMapVisualizer>().GetFirstAnimController();
			base.StartSM();
		}

		public void PlayVisAnim(string animName, KAnim.PlayMode playMode)
		{
			base.GetComponent<ClusterMapVisualizer>().PlayAnim(animName, playMode);
		}

		public void ToggleVisAnim(bool on)
		{
			ClusterMapVisualizer component = base.GetComponent<ClusterMapVisualizer>();
			if (!on)
			{
				component.GetFirstAnimController().Play("grounded", KAnim.PlayMode.Once, 1f, 0f);
			}
		}

		public void SubscribeOnVisAnimComplete(Action<object> action)
		{
			ClusterMapVisualizer component = base.GetComponent<ClusterMapVisualizer>();
			this.UnsubscribeOnVisAnimComplete();
			this.animCompleteSubscriber = component.GetFirstAnimController().gameObject;
			this.animCompleteHandle = this.animCompleteSubscriber.Subscribe(-1061186183, action);
		}

		public void UnsubscribeOnVisAnimComplete()
		{
			if (this.animCompleteHandle != -1)
			{
				DebugUtil.DevAssert(this.animCompleteSubscriber != null, "ClusterMapRocketAnimator animCompleteSubscriber GameObject is null. Whatever the previous gameObject in this variable was, it may not have unsubscribed from an event properly", null);
				this.animCompleteSubscriber.Unsubscribe(this.animCompleteHandle);
				this.animCompleteHandle = -1;
			}
		}

		public void RefreshDrillConeVisibility()
		{
			List<ResourceHarvestModule.StatesInstance> allResourceHarvestModules = ((Clustercraft)base.smi.entity).GetAllResourceHarvestModules();
			bool flag = allResourceHarvestModules != null && allResourceHarvestModules.Count > 0;
			this.SetDrillConeVisibility(flag);
		}

		private void SetDrillConeVisibility(bool shouldBeVisible)
		{
			if (shouldBeVisible)
			{
				if (this.drillConeSubAnim == null)
				{
					this.drillConeSubAnim = this.CreateSymbolController("nose_target", true);
				}
				this.drillConeSubAnim.gameObject.SetActive(true);
				return;
			}
			if (this.drillConeSubAnim != null)
			{
				this.drillConeSubAnim.gameObject.SetActive(false);
			}
			base.GetComponent<ClusterMapVisualizer>().GetFirstAnimController().SetSymbolVisiblity("nose_target", false);
		}

		public void PlayDrillingAnimation()
		{
			if (this.drillConeSubAnim != null)
			{
				this.drillConeSubAnim.Play("drilling_loop", KAnim.PlayMode.Loop, 1f, 0f);
			}
		}

		public void PlayIdleDrillConeAnimation()
		{
			if (this.drillConeSubAnim != null)
			{
				this.drillConeSubAnim.Play("drill_cone_idle", KAnim.PlayMode.Once, 1f, 0f);
			}
		}

		private void DeleteDrillConeSubAnim()
		{
			if (this.drillConeSubAnim != null)
			{
				this.drillConeSubAnim.gameObject.DeleteObject();
				this.drillConeSubAnim = null;
			}
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			this.DeleteDrillConeSubAnim();
			this.UnsubscribeOnVisAnimComplete();
		}

		private KBatchedAnimController CreateSymbolController(string symbolName, bool require_sound = false)
		{
			KBatchedAnimController firstAnimController = base.GetComponent<ClusterMapVisualizer>().GetFirstAnimController();
			KBatchedAnimController kbatchedAnimController = this.CreateEmptyKAnimController(symbolName, firstAnimController);
			kbatchedAnimController.transform.SetParent(firstAnimController.transform, false);
			kbatchedAnimController.initialAnim = "drill_cone_idle";
			KBatchedAnimTracker kbatchedAnimTracker = kbatchedAnimController.gameObject.AddComponent<KBatchedAnimTracker>();
			HashedString hashedString = new HashedString(symbolName);
			kbatchedAnimTracker.controller = firstAnimController;
			kbatchedAnimTracker.symbol = hashedString;
			kbatchedAnimTracker.forceAlwaysVisible = false;
			if (require_sound)
			{
				kbatchedAnimController.gameObject.AddComponent<LoopingSounds>();
			}
			kbatchedAnimController.gameObject.SetActive(false);
			firstAnimController.SetSymbolVisiblity(symbolName, false);
			return kbatchedAnimController;
		}

		private KBatchedAnimController CreateEmptyKAnimController(string name, KBatchedAnimController animController)
		{
			GameObject gameObject = new GameObject(base.gameObject.name + "-" + name);
			gameObject.SetActive(false);
			KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
			kbatchedAnimController.AnimFiles = new KAnimFile[] { Assets.GetAnim("rocket01_kanim") };
			kbatchedAnimController.materialType = KAnimBatchGroup.MaterialType.UI;
			kbatchedAnimController.animScale = ((animController == null) ? 0.08f : animController.animScale);
			kbatchedAnimController.fgLayer = Grid.SceneLayer.NoLayer;
			kbatchedAnimController.sceneLayer = Grid.SceneLayer.NoLayer;
			kbatchedAnimController.forceUseGameTime = true;
			return kbatchedAnimController;
		}

		public ClusterGridEntity entity;

		private KBatchedAnimController drillConeSubAnim;

		private int animCompleteHandle = -1;

		private GameObject animCompleteSubscriber;
	}
}
