using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class FlytrapConsumptionMonitor : StateMachineComponent<FlytrapConsumptionMonitor.Instance>, IGameObjectEffectDescriptor, IPlantConsumeEntities
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public string GetConsumableEntitiesCategoryName()
	{
		return CREATURES.SPECIES.FLYTRAPPLANT.VICTIM_IDENTIFIER;
	}

	public bool AreEntitiesConsumptionRequirementsSatisfied()
	{
		return base.smi != null && base.smi.HasEaten;
	}

	public string GetRequirementText()
	{
		return CREATURES.SPECIES.FLYTRAPPLANT.PLANT_HUNGER_REQUIREMENT;
	}

	public string GetConsumedEntityName()
	{
		if (base.smi != null)
		{
			return base.smi.LastConsumedEntityName;
		}
		return "Unknown Critter";
	}

	public List<KPrefabID> GetPrefabsOfPossiblePrey()
	{
		List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag(FlytrapConsumptionMonitor.CONSUMABLE_TAG);
		List<KPrefabID> list = new List<KPrefabID>();
		for (int i = 0; i < prefabsWithTag.Count; i++)
		{
			KPrefabID component = prefabsWithTag[i].GetComponent<KPrefabID>();
			if (this.IsEntityEdible(component) && !list.Contains(component) && Game.IsCorrectDlcActiveForCurrentSave(component))
			{
				list.Add(component);
			}
		}
		return list;
	}

	public string[] GetFormattedPossiblePreyList()
	{
		List<string> list = new List<string>();
		foreach (KPrefabID kprefabID in this.GetPrefabsOfPossiblePrey())
		{
			CreatureBrain component = kprefabID.GetComponent<CreatureBrain>();
			if (component != null)
			{
				string text = component.species.ProperName();
				if (!list.Contains(text))
				{
					list.Add(text);
				}
			}
		}
		return list.ToArray();
	}

	public bool IsEntityEdible(GameObject entity)
	{
		return !(entity == null) && this.IsEntityEdible(entity.GetComponent<KPrefabID>());
	}

	public bool IsEntityEdible(KPrefabID entity)
	{
		return !(entity == null) && entity.HasTag(FlytrapConsumptionMonitor.CONSUMABLE_TAG) && entity.GetComponent<CreatureBrain>() != null && entity.GetComponent<OccupyArea>().OccupiedCellsOffsets.Length <= 1;
	}

	public List<Descriptor> GetDescriptors(GameObject obj)
	{
		return new List<Descriptor>
		{
			new Descriptor(this.GetRequirementText(), "", Descriptor.DescriptorType.Requirement, false)
		};
	}

	public static bool IsWilted(FlytrapConsumptionMonitor.Instance smi)
	{
		return smi.IsWilted;
	}

	public static void CompleteEat(FlytrapConsumptionMonitor.Instance smi)
	{
		smi.sm.HasEaten.Set(true, smi, false);
	}

	public static void RetriggerGrowAnimationIfInGrowState(FlytrapConsumptionMonitor.Instance smi)
	{
		StandardCropPlant component = smi.GetComponent<StandardCropPlant>();
		if (component == null || component.smi == null)
		{
			return;
		}
		if (component.smi.IsInsideState(component.smi.sm.alive.idle))
		{
			KBatchedAnimController component2 = smi.GetComponent<KBatchedAnimController>();
			if (component2 != null)
			{
				component2.Play(component.anims.grow, component.anims.grow_playmode, 1f, 0f);
			}
		}
	}

	public static void BecomeHungry(FlytrapConsumptionMonitor.Instance smi)
	{
		smi.sm.HasEaten.Set(false, smi, false);
	}

	public static void RegisterVictimProximityMonitor(FlytrapConsumptionMonitor.Instance smi)
	{
		smi.RegisterVictimProximityMonitor();
	}

	public static void UnregisterVictimProximityMonitor(FlytrapConsumptionMonitor.Instance smi)
	{
		smi.UnregisterVictimProximityMonitor();
	}

	public static void SetAndPlayConsumeCropPlantAnimations(FlytrapConsumptionMonitor.Instance smi)
	{
		StandardCropPlant component = smi.GetComponent<StandardCropPlant>();
		if (component == null || component.smi == null)
		{
			return;
		}
		component.anims = FlytrapConsumptionMonitor.EATING_STATE_ANIM_SET;
		component.smi.GoTo(component.smi.sm.alive.pre_idle);
	}

	public static void SetCropPlantAnimationsToAwaitPrey(FlytrapConsumptionMonitor.Instance smi)
	{
		FlytrapConsumptionMonitor.SetCropPlantAnimationSet(smi, FlytrapConsumptionMonitor.HUNGRY_STATE_ANIM_SET);
		FlytrapConsumptionMonitor.RetriggerGrowAnimationIfInGrowState(smi);
		StandardCropPlant component = smi.GetComponent<StandardCropPlant>();
		if (component == null || component.smi == null)
		{
			return;
		}
		component.preventGrowPositionUpdate = true;
	}

	public static void RestoreDefaultCropPlantAnimations(FlytrapConsumptionMonitor.Instance smi)
	{
		FlytrapConsumptionMonitor.SetCropPlantAnimationSet(smi, FlyTrapPlantConfig.Default_StandardCropAnimSet);
		StandardCropPlant component = smi.GetComponent<StandardCropPlant>();
		if (component == null || component.smi == null)
		{
			return;
		}
		component.preventGrowPositionUpdate = false;
	}

	private static void SetCropPlantAnimationSet(FlytrapConsumptionMonitor.Instance smi, StandardCropPlant.AnimSet set)
	{
		StandardCropPlant component = smi.GetComponent<StandardCropPlant>();
		if (component == null || component.smi == null)
		{
			return;
		}
		component.anims = set;
	}

	public const string AWAIT_PREY_ANIM_NAME = "awaiting_prey";

	public const string EAT_ANIM_NAME = "consume";

	private const string CONSUMED_ENTITY_NAME_FALLBACK = "Unknown Critter";

	private static Tag CONSUMABLE_TAG = GameTags.Creatures.Flyer;

	public static readonly StandardCropPlant.AnimSet HUNGRY_STATE_ANIM_SET = new StandardCropPlant.AnimSet(FlyTrapPlantConfig.Default_StandardCropAnimSet)
	{
		grow = "awaiting_prey",
		wilt_base = "flower_wilt",
		grow_playmode = KAnim.PlayMode.Loop
	};

	public static readonly StandardCropPlant.AnimSet EATING_STATE_ANIM_SET = new StandardCropPlant.AnimSet(FlyTrapPlantConfig.Default_StandardCropAnimSet)
	{
		pre_grow = "consume",
		grow_playmode = KAnim.PlayMode.Paused
	};

	public class States : GameStateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = StateMachine.SerializeType.ParamsOnly;
			default_state = this.hungry;
			this.hungry.ParamTransition<bool>(this.HasEaten, this.satisfied, GameStateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.IsTrue).Toggle("Toggle Standard Crop Plant Animations", new StateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.State.Callback(FlytrapConsumptionMonitor.SetCropPlantAnimationsToAwaitPrey), new StateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.State.Callback(FlytrapConsumptionMonitor.RestoreDefaultCropPlantAnimations)).ToggleAttributeModifier("Pause Growing", (FlytrapConsumptionMonitor.Instance smi) => smi.pauseGrowing, null)
				.DefaultState(this.hungry.idle);
			this.hungry.idle.EventTransition(GameHashes.Wilt, this.hungry.wilt, new StateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.Transition.ConditionCallback(FlytrapConsumptionMonitor.IsWilted)).ToggleStatusItem(Db.Get().CreatureStatusItems.CarnivorousPlantAwaitingVictim, (FlytrapConsumptionMonitor.Instance smi) => smi.master.GetComponent<IPlantConsumeEntities>()).Enter(new StateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.State.Callback(FlytrapConsumptionMonitor.RegisterVictimProximityMonitor))
				.TriggerOnEnter(GameHashes.CropSleep, null)
				.OnSignal(this.EatSignal, this.hungry.complete)
				.Exit(new StateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.State.Callback(FlytrapConsumptionMonitor.UnregisterVictimProximityMonitor));
			this.hungry.complete.Enter(new StateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.State.Callback(FlytrapConsumptionMonitor.SetAndPlayConsumeCropPlantAnimations)).Enter(new StateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.State.Callback(FlytrapConsumptionMonitor.CompleteEat));
			this.hungry.wilt.EventTransition(GameHashes.WiltRecover, this.hungry.idle, GameStateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.Not(new StateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.Transition.ConditionCallback(FlytrapConsumptionMonitor.IsWilted)));
			this.satisfied.Enter(new StateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.State.Callback(FlytrapConsumptionMonitor.RetriggerGrowAnimationIfInGrowState)).TriggerOnEnter(GameHashes.CropWakeUp, null).ParamTransition<bool>(this.HasEaten, this.hungry, GameStateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.IsFalse)
				.EventHandler(GameHashes.Harvest, new StateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.State.Callback(FlytrapConsumptionMonitor.BecomeHungry));
		}

		public FlytrapConsumptionMonitor.States.HungryStates hungry;

		public GameStateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.State satisfied;

		public StateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.BoolParameter HasEaten;

		public StateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.Signal EatSignal;

		public class HungryStates : GameStateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.State
		{
			public GameStateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.State wilt;

			public GameStateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.State idle;

			public GameStateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.State complete;
		}
	}

	public class Instance : GameStateMachine<FlytrapConsumptionMonitor.States, FlytrapConsumptionMonitor.Instance, FlytrapConsumptionMonitor, object>.GameInstance
	{
		public bool HasEaten
		{
			get
			{
				return base.sm.HasEaten.Get(this);
			}
		}

		public bool IsWilted
		{
			get
			{
				return this.wiltCondition.IsWilting();
			}
		}

		public string LastConsumedEntityName
		{
			get
			{
				if (!string.IsNullOrEmpty(this.lastConsumedEntityPrefabID))
				{
					return Assets.GetPrefab(this.lastConsumedEntityPrefabID).GetProperName();
				}
				return "Unknown Critter";
			}
		}

		public Instance(FlytrapConsumptionMonitor master)
			: base(master)
		{
			Amounts amounts = base.gameObject.GetAmounts();
			this.maturity = amounts.Get(Db.Get().Amounts.Maturity);
			this.pauseGrowing = new AttributeModifier(this.maturity.deltaAttribute.Id, -1f, CREATURES.SPECIES.FLYTRAPPLANT.HUNGRY, true, false, true);
			this.wiltCondition = base.GetComponent<WiltCondition>();
			this.growing = base.GetComponent<Growing>();
			this.growing.CustomGrowStallCondition_IsStalled = new Func<GameObject, bool>(this.ShouldStallGrowingComponent);
		}

		private bool ShouldStallGrowingComponent(GameObject plantGameObject)
		{
			return !this.HasEaten;
		}

		public void RegisterVictimProximityMonitor()
		{
			OccupyArea component = base.GetComponent<OccupyArea>();
			this.partitionerEntry = GameScenePartitioner.Instance.Add("FlytrapConsumptionMonitor.hungry.idle", base.gameObject, component.GetExtents(), GameScenePartitioner.Instance.pickupablesChangedLayer, new Action<object>(this.OnPickupableLayerObjectDetected));
		}

		public void UnregisterVictimProximityMonitor()
		{
			GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
			this.partitionerEntry = HandleVector<int>.InvalidHandle;
		}

		public void OnPickupableLayerObjectDetected(object obj)
		{
			Pickupable pickupable = obj as Pickupable;
			if (base.master.IsEntityEdible(pickupable.gameObject))
			{
				this.lastConsumedEntityPrefabID = pickupable.PrefabID().ToString();
				pickupable.gameObject.DeleteObject();
				base.sm.EatSignal.Trigger(this);
			}
		}

		public AttributeModifier pauseGrowing;

		[Serialize]
		private string lastConsumedEntityPrefabID;

		private Growing growing;

		private WiltCondition wiltCondition;

		private AmountInstance maturity;

		private HandleVector<int>.Handle partitionerEntry = HandleVector<int>.InvalidHandle;
	}
}
