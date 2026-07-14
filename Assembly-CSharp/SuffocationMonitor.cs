using System;
using Klei.AI;
using STRINGS;
using TUNING;

public class SuffocationMonitor : GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, SuffocationMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.satisfied;
		this.root.TagTransition(GameTags.Dead, this.dead, false);
		this.satisfied.DefaultState(this.satisfied.normal).ToggleAttributeModifier("Breathing", (SuffocationMonitor.Instance smi) => smi.increaseBreathModifier, null).EventTransition(GameHashes.OxygenBreatherHasAirChanged, this.noOxygen, (SuffocationMonitor.Instance smi) => !smi.CanBreath())
			.Transition(this.noOxygen, (SuffocationMonitor.Instance smi) => !smi.CanBreath(), UpdateRate.SIM_200ms);
		this.satisfied.normal.Transition(this.satisfied.low, (SuffocationMonitor.Instance smi) => smi.oxygenBreather.IsLowOxygen(), UpdateRate.SIM_200ms);
		this.satisfied.low.Transition(this.satisfied.normal, (SuffocationMonitor.Instance smi) => !smi.oxygenBreather.IsLowOxygen(), UpdateRate.SIM_200ms).ToggleEffect("LowOxygen");
		this.noOxygen.EventTransition(GameHashes.OxygenBreatherHasAirChanged, this.satisfied, (SuffocationMonitor.Instance smi) => smi.CanBreath()).TagTransition(GameTags.RecoveringBreath, this.satisfied, false).ToggleExpression(Db.Get().Expressions.Suffocate, (SuffocationMonitor.Instance smi) => smi.IsBreathDepletingSignificantly() || smi.IsBreathLow())
			.ToggleAttributeModifier("Holding Breath", (SuffocationMonitor.Instance smi) => smi.decreaseBreathModifier, null)
			.ToggleTag(GameTags.NoOxygen)
			.DefaultState(this.noOxygen.holdingbreath);
		this.noOxygen.holdingbreath.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.HoldingBreath, null).Transition(this.noOxygen.suffocating, (SuffocationMonitor.Instance smi) => smi.IsSuffocating(), UpdateRate.SIM_200ms);
		this.noOxygen.suffocating.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.Suffocating, null).Transition(this.noOxygen.incapacitated, (SuffocationMonitor.Instance smi) => smi.HasSuffocated(), UpdateRate.SIM_200ms);
		this.noOxygen.incapacitated.Enter(delegate(SuffocationMonitor.Instance smi)
		{
			smi.sm.timeUntilDeath.Set(smi.def.timeBeforeDeath, smi, false);
		}).ToggleRecurringChore((SuffocationMonitor.Instance smi) => new BeIncapacitatedSuffocatingChore(smi.master), null).ToggleUrge(Db.Get().Urges.BeIncapacitated)
			.ToggleTag(GameTags.SuffocatingIncapacitated)
			.Update(delegate(SuffocationMonitor.Instance smi, float dt)
			{
				this.UpdateTimeUntilDeath(smi, dt);
			}, UpdateRate.SIM_200ms, false)
			.ParamTransition<float>(this.timeUntilDeath, this.death, GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, SuffocationMonitor.Def>.IsLTZero)
			.EventTransition(GameHashes.IncapacitationRecovery, this.satisfied, null);
		this.death.Enter("SuffocationDeath", delegate(SuffocationMonitor.Instance smi)
		{
			smi.Kill();
		});
		this.dead.DoNothing();
	}

	private void UpdateTimeUntilDeath(SuffocationMonitor.Instance smi, float dt)
	{
		smi.sm.timeUntilDeath.Delta(dt * -1f, smi);
	}

	public StateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, SuffocationMonitor.Def>.FloatParameter timeUntilDeath;

	public SuffocationMonitor.SatisfiedState satisfied;

	public SuffocationMonitor.NoOxygenState noOxygen;

	public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, SuffocationMonitor.Def>.State death;

	public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, SuffocationMonitor.Def>.State dead;

	public class Def : StateMachine.BaseDef
	{
		public float timeBeforeDeath = 120f;
	}

	public class NoOxygenState : GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, SuffocationMonitor.Def>.State
	{
		public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, SuffocationMonitor.Def>.State holdingbreath;

		public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, SuffocationMonitor.Def>.State suffocating;

		public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, SuffocationMonitor.Def>.State incapacitated;
	}

	public class SatisfiedState : GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, SuffocationMonitor.Def>.State
	{
		public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, SuffocationMonitor.Def>.State normal;

		public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, SuffocationMonitor.Def>.State low;
	}

	public new class Instance : GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, SuffocationMonitor.Def>.GameInstance
	{
		public OxygenBreather oxygenBreather { get; private set; }

		public Instance(IStateMachineTarget master, SuffocationMonitor.Def def)
			: base(master, def)
		{
			this.breath = Db.Get().Amounts.Breath.Lookup(master.gameObject);
			Klei.AI.Attribute deltaAttribute = Db.Get().Amounts.Breath.deltaAttribute;
			float breath_RATE = DUPLICANTSTATS.STANDARD.Breath.BREATH_RATE;
			this.increaseBreathModifier = new AttributeModifier(deltaAttribute.Id, breath_RATE, DUPLICANTS.MODIFIERS.BREATHING.NAME, false, false, true);
			this.decreaseBreathModifier = new AttributeModifier(deltaAttribute.Id, -breath_RATE, DUPLICANTS.MODIFIERS.HOLDINGBREATH.NAME, false, false, true);
			this.oxygenBreather = base.GetComponent<OxygenBreather>();
		}

		public bool IsBreathDepletingSignificantly()
		{
			return this.breath.deltaAttribute.GetTotalValue() <= -DUPLICANTSTATS.STANDARD.Breath.BREATH_RATE * 0.5f;
		}

		public bool IsBreathLow()
		{
			return this.breath.value <= DUPLICANTSTATS.STANDARD.Breath.SUFFOCATE_AMOUNT;
		}

		public bool CanBreath()
		{
			return this.oxygenBreather.prefabID.HasTag(GameTags.RecoveringBreath) || this.oxygenBreather.prefabID.HasTag(GameTags.InTransitTube) || this.oxygenBreather.HasOxygen;
		}

		public bool HasSuffocated()
		{
			return this.breath.value <= 0f;
		}

		public bool IsSuffocating()
		{
			return this.breath.deltaAttribute.GetTotalValue() <= 0f && this.breath.value <= DUPLICANTSTATS.STANDARD.Breath.SUFFOCATE_AMOUNT;
		}

		public float GetTimeUntilDeath(SuffocationMonitor.Instance smi)
		{
			return smi.sm.timeUntilDeath.Get(smi);
		}

		public void Kill()
		{
			base.gameObject.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Suffocation);
		}

		private AmountInstance breath;

		public AttributeModifier increaseBreathModifier;

		public AttributeModifier decreaseBreathModifier;
	}
}
