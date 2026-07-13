using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class BionicOilMonitor : GameStateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>
{
	private static Effect CreateFreshOilEffectVariation(string id, float stressBonus, float moralBonus)
	{
		Effect effect = new Effect("FreshOil_" + id, DUPLICANTS.MODIFIERS.FRESHOIL.NAME, DUPLICANTS.MODIFIERS.FRESHOIL.TOOLTIP, 4800f, true, true, false, null, -1f, 0f, null, "");
		effect.Add(new AttributeModifier(Db.Get().Attributes.QualityOfLife.Id, moralBonus, DUPLICANTS.MODIFIERS.FRESHOIL.NAME, false, false, true));
		effect.Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, stressBonus, DUPLICANTS.MODIFIERS.FRESHOIL.NAME, false, false, true));
		return effect;
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.offline;
		this.root.Update(new Action<BionicOilMonitor.Instance, float>(BionicOilMonitor.OilAmountInstanceWatcherUpdate), UpdateRate.SIM_200ms, false).Exit(new StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.State.Callback(BionicOilMonitor.RemoveBaseOilDeltaModifier));
		this.offline.EventTransition(GameHashes.BionicOnline, this.online, new StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.Transition.ConditionCallback(BionicOilMonitor.IsBionicOnline)).Enter(new StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.State.Callback(BionicOilMonitor.RemoveBaseOilDeltaModifier));
		this.online.EventTransition(GameHashes.BionicOffline, this.offline, GameStateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.Not(new StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.Transition.ConditionCallback(BionicOilMonitor.IsBionicOnline))).Enter(new StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.State.Callback(BionicOilMonitor.AddBaseOilDeltaModifier)).DefaultState(this.online.idle)
			.Enter(new StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.State.Callback(BionicOilMonitor.EnableSolidLubricationSensor))
			.Exit(new StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.State.Callback(BionicOilMonitor.DisableSolidLubricationSensor));
		this.online.idle.EnterTransition(this.online.seeking, new StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.Transition.ConditionCallback(BionicOilMonitor.WantsOilChange)).OnSignal(this.OilValueChanged, this.online.seeking, new StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.Parameter<StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.SignalParameter>.Callback(BionicOilMonitor.WantsOilChange));
		this.online.seeking.OnSignal(this.OilFilledSignal, this.online.idle).OnSignal(this.OilValueChanged, this.online.idle, new StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.Parameter<StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.SignalParameter>.Callback(BionicOilMonitor.HasDecentAmountOfOil)).DefaultState(this.online.seeking.hasOil)
			.ToggleThought(Db.Get().Thoughts.RefillOilDesire, null)
			.ToggleUrge(Db.Get().Urges.OilRefill)
			.ToggleChore((BionicOilMonitor.Instance smi) => new UseSolidLubricantChore(smi.master), this.online.idle);
		this.online.seeking.hasOil.EnterTransition(this.online.seeking.noOil, GameStateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.Not(new StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.Transition.ConditionCallback(BionicOilMonitor.HasAnyAmountOfOil))).OnSignal(this.OilRanOutSignal, this.online.seeking.noOil).ToggleStatusItem(Db.Get().DuplicantStatusItems.BionicWantsOilChange, null);
		this.online.seeking.noOil.Enter(delegate(BionicOilMonitor.Instance smi)
		{
			smi.currentNoLubricationEffectApplied = smi.effects.Add(smi.GetEffect(), false).effect.IdHash;
		}).Exit(delegate(BionicOilMonitor.Instance smi)
		{
			smi.effects.Remove(smi.currentNoLubricationEffectApplied);
		}).ToggleReactable(new Func<BionicOilMonitor.Instance, Reactable>(BionicOilMonitor.GrindingGearsReactable))
			.EventTransition(GameHashes.AssignedRoleChanged, this.online.seeking.hasOil, null);
	}

	public static bool IsBionicOnline(BionicOilMonitor.Instance smi)
	{
		return smi.IsOnline;
	}

	public static bool HasAnyAmountOfOil(BionicOilMonitor.Instance smi)
	{
		return smi.CurrentOilMass > 0f;
	}

	public static bool HasDecentAmountOfOil(BionicOilMonitor.Instance smi)
	{
		return BionicOilMonitor.HasDecentAmountOfOil(smi, null);
	}

	public static bool HasDecentAmountOfOil(BionicOilMonitor.Instance smi, StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.SignalParameter param)
	{
		return smi.CurrentOilPercentage > 0.2f;
	}

	public static bool WantsOilChange(BionicOilMonitor.Instance smi)
	{
		return BionicOilMonitor.WantsOilChange(smi, null);
	}

	public static bool WantsOilChange(BionicOilMonitor.Instance smi, StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.SignalParameter param)
	{
		return smi.CurrentOilPercentage <= 0.2f;
	}

	public static void AddBaseOilDeltaModifier(BionicOilMonitor.Instance smi)
	{
		smi.SetBaseDeltaModifierActiveState(true);
	}

	public static void RemoveBaseOilDeltaModifier(BionicOilMonitor.Instance smi)
	{
		smi.SetBaseDeltaModifierActiveState(false);
	}

	public static void OilAmountInstanceWatcherUpdate(BionicOilMonitor.Instance smi, float dt)
	{
		float lastOilAmountMassRecorded = smi.LastOilAmountMassRecorded;
		float num = smi.CurrentOilMass - lastOilAmountMassRecorded;
		if (num != 0f)
		{
			smi.LastOilAmountMassRecorded = smi.CurrentOilMass;
			if (!smi.HasOil)
			{
				smi.ReportOilRanOut();
			}
			smi.ReportOilValueChanged(num);
		}
	}

	public static void EnableSolidLubricationSensor(BionicOilMonitor.Instance smi)
	{
		smi.SetSolidLubricationSensorActiveState(true);
	}

	public static void DisableSolidLubricationSensor(BionicOilMonitor.Instance smi)
	{
		smi.SetSolidLubricationSensorActiveState(false);
	}

	private static Reactable GrindingGearsReactable(BionicOilMonitor.Instance smi)
	{
		return smi.GetGrindingGearReactable();
	}

	public static void ApplyLubricationEffects(Effects targetBionicEffects, SimHashes lubricant)
	{
		foreach (SimHashes simHashes in BionicOilMonitor.LUBRICANT_TYPE_EFFECT.Keys)
		{
			if (BionicOilMonitor.LUBRICANT_TYPE_EFFECT.ContainsKey(simHashes))
			{
				Effect effect = BionicOilMonitor.LUBRICANT_TYPE_EFFECT[simHashes];
				if (lubricant == simHashes)
				{
					targetBionicEffects.Add(effect, true);
				}
				else
				{
					targetBionicEffects.Remove(effect);
				}
			}
		}
	}

	// Note: this type is marked as 'beforefieldinit'.
	static BionicOilMonitor()
	{
		Dictionary<SimHashes, Effect> dictionary = new Dictionary<SimHashes, Effect>();
		dictionary[SimHashes.Tallow] = BionicOilMonitor.CreateFreshOilEffectVariation(SimHashes.Tallow.ToString(), -0.016666668f, 3f);
		dictionary[SimHashes.CrudeOil] = BionicOilMonitor.CreateFreshOilEffectVariation(SimHashes.CrudeOil.ToString(), -0.016666668f, 3f);
		dictionary[SimHashes.PhytoOil] = BionicOilMonitor.CreateFreshOilEffectVariation(SimHashes.PhytoOil.ToString(), -0.008333334f, 2f);
		BionicOilMonitor.LUBRICANT_TYPE_EFFECT = dictionary;
	}

	public static Dictionary<SimHashes, Effect> LUBRICANT_TYPE_EFFECT;

	public const float OIL_CAPACITY = 200f;

	public const float OIL_TANK_DURATION = 6000f;

	public const float OIL_REFILL_TRESHOLD = 0.2f;

	public const string NO_OIL_EFFECT_NAME_MINOR = "NoLubricationMinor";

	public const string NO_OIL_EFFECT_NAME_MAJOR = "NoLubricationMajor";

	public GameStateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.State offline;

	public BionicOilMonitor.OnlineStates online;

	public StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.Signal OilFilledSignal;

	public StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.Signal OilRanOutSignal;

	public StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.Signal OilValueChanged;

	public StateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.Signal OnClosestSolidLubricantChangedSignal;

	public class Def : StateMachine.BaseDef
	{
	}

	public class WantsOilChangeState : GameStateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.State
	{
		public GameStateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.State hasOil;

		public GameStateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.State noOil;
	}

	public class OnlineStates : GameStateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.State
	{
		public GameStateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.State idle;

		public BionicOilMonitor.WantsOilChangeState seeking;
	}

	public new class Instance : GameStateMachine<BionicOilMonitor, BionicOilMonitor.Instance, IStateMachineTarget, BionicOilMonitor.Def>.GameInstance
	{
		public bool IsOnline
		{
			get
			{
				return this.batterySMI != null && this.batterySMI.IsOnline;
			}
		}

		public bool HasOil
		{
			get
			{
				return this.CurrentOilMass > 0f;
			}
		}

		public float CurrentOilPercentage
		{
			get
			{
				return this.CurrentOilMass / this.oilAmount.GetMax();
			}
		}

		public float CurrentOilMass
		{
			get
			{
				if (this.oilAmount != null)
				{
					return this.oilAmount.value;
				}
				return 0f;
			}
		}

		public AmountInstance oilAmount { get; private set; }

		public Instance(IStateMachineTarget master, BionicOilMonitor.Def def)
			: base(master, def)
		{
			this.oilAmount = Db.Get().Amounts.BionicOil.Lookup(base.gameObject);
			this.batterySMI = base.gameObject.GetSMI<BionicBatteryMonitor.Instance>();
		}

		public override void StartSM()
		{
			this.closestSolidLubricantSensor = base.GetComponent<Sensors>().GetSensor<ClosestLubricantSensor>();
			ClosestLubricantSensor closestLubricantSensor = this.closestSolidLubricantSensor;
			closestLubricantSensor.OnItemChanged = (Action<Pickupable>)Delegate.Combine(closestLubricantSensor.OnItemChanged, new Action<Pickupable>(this.OnClosestSolidLubricantChanged));
			this.LastOilAmountMassRecorded = this.CurrentOilMass;
			base.StartSM();
		}

		public string GetEffect()
		{
			if (!this.resume.HasPerk(Db.Get().SkillPerks.EfficientBionicGears))
			{
				return "NoLubricationMajor";
			}
			return "NoLubricationMinor";
		}

		private void ReportOilTankFilled()
		{
			base.sm.OilFilledSignal.Trigger(this);
		}

		public void ReportOilRanOut()
		{
			base.sm.OilRanOutSignal.Trigger(this);
		}

		public void ReportOilValueChanged(float delta)
		{
			base.sm.OilValueChanged.Trigger(this);
			Action<float> onOilValueChanged = this.OnOilValueChanged;
			if (onOilValueChanged == null)
			{
				return;
			}
			onOilValueChanged(delta);
		}

		public void SetOilMassValue(float value)
		{
			this.oilAmount.SetValue(value);
		}

		public void SetBaseDeltaModifierActiveState(bool isActive)
		{
			MinionModifiers component = base.GetComponent<MinionModifiers>();
			if (isActive)
			{
				bool flag = false;
				int count = component.attributes.Get(this.BaseOilDeltaModifier.AttributeId).Modifiers.Count;
				for (int i = 0; i < count; i++)
				{
					if (component.attributes.Get(this.BaseOilDeltaModifier.AttributeId).Modifiers[i] == this.BaseOilDeltaModifier)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					component.attributes.Add(this.BaseOilDeltaModifier);
					return;
				}
			}
			else
			{
				component.attributes.Remove(this.BaseOilDeltaModifier);
			}
		}

		public void RefillOil(float amount)
		{
			this.oilAmount.SetValue(this.CurrentOilMass + amount);
			this.ReportOilTankFilled();
		}

		private void OnClosestSolidLubricantChanged(Pickupable newItem)
		{
			base.sm.OnClosestSolidLubricantChangedSignal.Trigger(this);
		}

		public Pickupable GetClosestSolidLubricant()
		{
			return this.closestSolidLubricantSensor.GetItem();
		}

		public void SetSolidLubricationSensorActiveState(bool shouldItBeActive)
		{
			this.closestSolidLubricantSensor.SetActive(shouldItBeActive);
			if (shouldItBeActive)
			{
				this.closestSolidLubricantSensor.Update();
			}
		}

		public Reactable GetGrindingGearReactable()
		{
			SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(base.master.gameObject, Db.Get().Emotes.Minion.GrindingGears.Id, Db.Get().ChoreTypes.EmoteHighPriority, 0f, 10f, float.PositiveInfinity, 0f);
			Emote grindingGears = Db.Get().Emotes.Minion.GrindingGears;
			selfEmoteReactable.SetEmote(grindingGears);
			selfEmoteReactable.SetThought(Db.Get().Thoughts.RefillOilDesire);
			selfEmoteReactable.preventChoreInterruption = true;
			return selfEmoteReactable;
		}

		public float LastOilAmountMassRecorded = -1f;

		public Action<float> OnOilValueChanged;

		private BionicBatteryMonitor.Instance batterySMI;

		[MyCmpGet]
		private MinionResume resume;

		[MyCmpGet]
		public Effects effects;

		public HashedString currentNoLubricationEffectApplied;

		private AttributeModifier BaseOilDeltaModifier = new AttributeModifier(Db.Get().Amounts.BionicOil.deltaAttribute.Id, -0.033333335f, BionicMinionConfig.NAME, false, false, true);

		private ClosestLubricantSensor closestSolidLubricantSensor;
	}
}
