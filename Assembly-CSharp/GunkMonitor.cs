using System;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class GunkMonitor : GameStateMachine<GunkMonitor, GunkMonitor.Instance, IStateMachineTarget, GunkMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.idle;
		this.root.Update(new Action<GunkMonitor.Instance, float>(GunkMonitor.GunkAmountWatcherUpdate), UpdateRate.SIM_200ms, false);
		this.idle.OnSignal(this.gunkValueChangedSignal, this.mildUrge, new Func<GunkMonitor.Instance, bool>(GunkMonitor.IsGunkLevelsOverMildUrgeThreshold));
		this.mildUrge.OnSignal(this.gunkValueChangedSignal, this.criticalUrge, new Func<GunkMonitor.Instance, bool>(GunkMonitor.IsGunkLevelsOverCriticalUrgeThreshold)).OnSignal(this.gunkValueChangedSignal, this.idle, new Func<GunkMonitor.Instance, bool>(GunkMonitor.DoesNotWantToExpellGunk)).DefaultState(this.mildUrge.prevented);
		this.mildUrge.prevented.ScheduleChange(this.mildUrge.allowed, new StateMachine<GunkMonitor, GunkMonitor.Instance, IStateMachineTarget, GunkMonitor.Def>.Transition.ConditionCallback(GunkMonitor.ScheduleAllowsExpelling));
		this.mildUrge.allowed.ScheduleChange(this.mildUrge.prevented, GameStateMachine<GunkMonitor, GunkMonitor.Instance, IStateMachineTarget, GunkMonitor.Def>.Not(new StateMachine<GunkMonitor, GunkMonitor.Instance, IStateMachineTarget, GunkMonitor.Def>.Transition.ConditionCallback(GunkMonitor.ScheduleAllowsExpelling))).ToggleUrge(Db.Get().Urges.Pee).ToggleUrge(Db.Get().Urges.GunkPee);
		this.criticalUrge.OnSignal(this.gunkValueChangedSignal, this.idle, new Func<GunkMonitor.Instance, bool>(GunkMonitor.DoesNotWantToExpellGunk)).OnSignal(this.gunkValueChangedSignal, this.mildUrge, (GunkMonitor.Instance smi) => !GunkMonitor.IsGunkLevelsOverCriticalUrgeThreshold(smi)).OnSignal(this.gunkValueChangedSignal, this.cantHold, new Func<GunkMonitor.Instance, bool>(GunkMonitor.CanNotHoldGunkAnymore))
			.ToggleUrge(Db.Get().Urges.GunkPee)
			.ToggleUrge(Db.Get().Urges.Pee)
			.ToggleEffect("GunkSick")
			.ToggleExpression(Db.Get().Expressions.FullBladder, null)
			.ToggleThought(Db.Get().Thoughts.ExpellGunkDesire, null)
			.ToggleAnims("anim_loco_walk_slouch_kanim", 0f)
			.ToggleAnims("anim_idle_slouch_kanim", 0f);
		this.cantHold.ToggleUrge(Db.Get().Urges.GunkPee).ToggleThought(Db.Get().Thoughts.ExpellingGunk, null).ToggleChore((GunkMonitor.Instance smi) => new BionicGunkSpillChore(smi.master), this.emptyRemaining);
		this.emptyRemaining.Enter(new StateMachine<GunkMonitor, GunkMonitor.Instance, IStateMachineTarget, GunkMonitor.Def>.State.Callback(GunkMonitor.ExpellAllGunk)).Enter(new StateMachine<GunkMonitor, GunkMonitor.Instance, IStateMachineTarget, GunkMonitor.Def>.State.Callback(GunkMonitor.ApplyGunkHungoverEffect)).GoTo(this.idle);
	}

	public static bool IsGunkLevelsOverCriticalUrgeThreshold(GunkMonitor.Instance smi)
	{
		return smi.CurrentGunkPercentage >= smi.def.DesperetlySeekForGunkToiletTreshold;
	}

	public static bool IsGunkLevelsOverMildUrgeThreshold(GunkMonitor.Instance smi)
	{
		return smi.CurrentGunkPercentage >= smi.def.SeekForGunkToiletTreshold_InSchedule;
	}

	public static bool ScheduleAllowsExpelling(GunkMonitor.Instance smi)
	{
		return smi.DoesCurrentScheduleAllowsGunkToilet;
	}

	public static bool DoesNotWantToExpellGunk(GunkMonitor.Instance smi)
	{
		return !GunkMonitor.IsGunkLevelsOverMildUrgeThreshold(smi);
	}

	public static bool CanNotHoldGunkAnymore(GunkMonitor.Instance smi)
	{
		return smi.IsGunkBuildupAtMax;
	}

	public static void ExpellAllGunk(GunkMonitor.Instance smi)
	{
		smi.ExpellAllGunk(null);
	}

	public static void ApplyGunkHungoverEffect(GunkMonitor.Instance smi)
	{
		smi.GetComponent<Effects>().Add("GunkHungover", true);
	}

	public static void GunkAmountWatcherUpdate(GunkMonitor.Instance smi, float dt)
	{
		smi.GunkAmountWatcherUpdate(dt);
	}

	public const float BIONIC_RADS_REMOVED_WHEN_PEE = 300f;

	public static readonly float GUNK_CAPACITY = 80f;

	public const string GUNK_FULL_EFFECT_NAME = "GunkSick";

	public const string GUNK_HUNGOVER_EFFECT_NAME = "GunkHungover";

	public static SimHashes GunkElement = SimHashes.LiquidGunk;

	public GameStateMachine<GunkMonitor, GunkMonitor.Instance, IStateMachineTarget, GunkMonitor.Def>.State idle;

	public GunkMonitor.MildUrgeStates mildUrge;

	public GameStateMachine<GunkMonitor, GunkMonitor.Instance, IStateMachineTarget, GunkMonitor.Def>.State criticalUrge;

	public GameStateMachine<GunkMonitor, GunkMonitor.Instance, IStateMachineTarget, GunkMonitor.Def>.State cantHold;

	public GameStateMachine<GunkMonitor, GunkMonitor.Instance, IStateMachineTarget, GunkMonitor.Def>.State emptyRemaining;

	public StateMachine<GunkMonitor, GunkMonitor.Instance, IStateMachineTarget, GunkMonitor.Def>.Signal gunkValueChangedSignal;

	public class Def : StateMachine.BaseDef
	{
		public float SeekForGunkToiletTreshold_InSchedule = 0.6f;

		public float DesperetlySeekForGunkToiletTreshold = 0.9f;
	}

	public class MildUrgeStates : GameStateMachine<GunkMonitor, GunkMonitor.Instance, IStateMachineTarget, GunkMonitor.Def>.State
	{
		public GameStateMachine<GunkMonitor, GunkMonitor.Instance, IStateMachineTarget, GunkMonitor.Def>.State allowed;

		public GameStateMachine<GunkMonitor, GunkMonitor.Instance, IStateMachineTarget, GunkMonitor.Def>.State prevented;
	}

	public new class Instance : GameStateMachine<GunkMonitor, GunkMonitor.Instance, IStateMachineTarget, GunkMonitor.Def>.GameInstance
	{
		public bool HasGunk
		{
			get
			{
				return this.CurrentGunkMass > 0f;
			}
		}

		public bool IsGunkBuildupAtMax
		{
			get
			{
				return this.CurrentGunkPercentage >= 1f;
			}
		}

		public float CurrentGunkMass
		{
			get
			{
				if (this.gunkAmount != null)
				{
					return this.gunkAmount.value;
				}
				return 0f;
			}
		}

		public float CurrentGunkPercentage
		{
			get
			{
				return this.CurrentGunkMass / this.gunkAmount.GetMax();
			}
		}

		public bool DoesCurrentScheduleAllowsGunkToilet
		{
			get
			{
				return this.schedulable.IsAllowed(Db.Get().ScheduleBlockTypes.Eat) || this.schedulable.IsAllowed(Db.Get().ScheduleBlockTypes.Hygiene);
			}
		}

		public Instance(IStateMachineTarget master, GunkMonitor.Def def)
			: base(master, def)
		{
			this.bodyTemperature = Db.Get().Amounts.Temperature.Lookup(base.gameObject);
			this.gunkAmount = Db.Get().Amounts.BionicGunk.Lookup(base.gameObject);
			this.schedulable = base.GetComponent<Schedulable>();
		}

		public override void StartSM()
		{
			this.oilMonitor = base.gameObject.GetSMI<BionicOilMonitor.Instance>();
			BionicOilMonitor.Instance instance = this.oilMonitor;
			instance.OnOilValueChanged = (Action<float>)Delegate.Combine(instance.OnOilValueChanged, new Action<float>(this.OnOilValueChanged));
			this.LastAmountOfGunkObserved = this.CurrentGunkMass;
			base.StartSM();
		}

		public void GunkAmountWatcherUpdate(float dt)
		{
			if (this.LastAmountOfGunkObserved != this.CurrentGunkMass)
			{
				this.LastAmountOfGunkObserved = this.CurrentGunkMass;
				base.sm.gunkValueChangedSignal.Trigger(this);
			}
		}

		protected override void OnCleanUp()
		{
			if (this.oilMonitor != null)
			{
				BionicOilMonitor.Instance instance = this.oilMonitor;
				instance.OnOilValueChanged = (Action<float>)Delegate.Remove(instance.OnOilValueChanged, new Action<float>(this.OnOilValueChanged));
			}
			base.OnCleanUp();
		}

		private void OnOilValueChanged(float delta)
		{
			float num = ((delta < 0f) ? Mathf.Abs(delta) : 0f);
			float num2 = Mathf.Clamp(this.CurrentGunkMass + num, 0f, this.gunkAmount.GetMax());
			this.SetGunkMassValue(num2);
		}

		public void SetGunkMassValue(float value)
		{
			float currentGunkMass = this.CurrentGunkMass;
			this.gunkAmount.SetValue(value);
			this.LastAmountOfGunkObserved = this.CurrentGunkMass;
			base.sm.gunkValueChangedSignal.Trigger(this);
		}

		public void ExpellGunk(float mass, Storage targetStorage = null)
		{
			if (this.HasGunk)
			{
				float currentGunkMass = this.CurrentGunkMass;
				float num = Mathf.Min(mass, this.CurrentGunkMass);
				num = Mathf.Max(num, Mathf.Epsilon);
				int num2 = Grid.PosToCell(base.transform.position);
				byte index = Db.Get().Diseases.GetIndex(DUPLICANTSTATS.BIONICS.Secretions.PEE_DISEASE);
				float num3 = num / GunkMonitor.GUNK_CAPACITY;
				if (targetStorage != null)
				{
					targetStorage.AddLiquid(GunkMonitor.GunkElement, num, this.bodyTemperature.value, index, (int)((float)DUPLICANTSTATS.BIONICS.Secretions.DISEASE_PER_PEE * num3), false, true);
				}
				else
				{
					Equippable equippable = base.GetComponent<SuitEquipper>().IsWearingAirtightSuit();
					if (equippable != null)
					{
						equippable.GetComponent<Storage>().AddLiquid(GunkMonitor.GunkElement, num, this.bodyTemperature.value, index, (int)((float)DUPLICANTSTATS.BIONICS.Secretions.DISEASE_PER_PEE * num3), false, true);
					}
					else
					{
						SimMessages.AddRemoveSubstance(num2, GunkMonitor.GunkElement, CellEventLogger.Instance.Vomit, num, this.bodyTemperature.value, index, (int)((float)DUPLICANTSTATS.BIONICS.Secretions.DISEASE_PER_PEE * num3), true, -1);
					}
				}
				if (Sim.IsRadiationEnabled())
				{
					MinionIdentity component = base.transform.GetComponent<MinionIdentity>();
					AmountInstance amountInstance = Db.Get().Amounts.RadiationBalance.Lookup(component);
					RadiationMonitor.Instance smi = component.GetSMI<RadiationMonitor.Instance>();
					float num4 = DUPLICANTSTATS.STANDARD.BaseStats.BLADDER_INCREASE_PER_SECOND / DUPLICANTSTATS.BIONICS.BaseStats.BLADDER_INCREASE_PER_SECOND;
					float num5 = Math.Min(amountInstance.value, 300f * num4 * smi.difficultySettingMod * num3);
					if (num5 >= 1f)
					{
						PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, Math.Floor((double)num5).ToString() + UI.UNITSUFFIXES.RADIATION.RADS, component.transform, Vector3.up * 2f, 1.5f, false, false);
					}
					amountInstance.ApplyDelta(-num5);
				}
				this.SetGunkMassValue(Mathf.Clamp(this.CurrentGunkMass - num, 0f, this.gunkAmount.GetMax()));
			}
		}

		public void ExpellAllGunk(Storage targetStorage = null)
		{
			this.ExpellGunk(this.CurrentGunkMass, targetStorage);
		}

		private float LastAmountOfGunkObserved;

		private BionicOilMonitor.Instance oilMonitor;

		private AmountInstance gunkAmount;

		private AmountInstance bodyTemperature;

		private Schedulable schedulable;
	}
}
