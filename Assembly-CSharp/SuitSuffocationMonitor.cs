using System;
using Klei.AI;
using STRINGS;

public class SuitSuffocationMonitor : GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.DefaultState(this.satisfied.normal).ToggleAttributeModifier("Breathing", (SuitSuffocationMonitor.Instance smi) => smi.breathing, null).Transition(this.nooxygen, (SuitSuffocationMonitor.Instance smi) => smi.IsTankEmpty(), UpdateRate.SIM_200ms);
		this.satisfied.normal.Transition(this.satisfied.low, (SuitSuffocationMonitor.Instance smi) => smi.suitTank.IsLow(), UpdateRate.SIM_200ms);
		this.satisfied.low.DoNothing();
		this.nooxygen.ToggleExpression(Db.Get().Expressions.Suffocate, null).ToggleAttributeModifier("Holding Breath", (SuitSuffocationMonitor.Instance smi) => smi.holdingbreath, null).ToggleTag(GameTags.NoOxygen)
			.DefaultState(this.nooxygen.holdingbreath);
		this.nooxygen.holdingbreath.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.HoldingBreath, null).Transition(this.nooxygen.suffocating, (SuitSuffocationMonitor.Instance smi) => smi.IsSuffocating(), UpdateRate.SIM_200ms);
		this.nooxygen.suffocating.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.Suffocating, null).Transition(this.death, (SuitSuffocationMonitor.Instance smi) => smi.HasSuffocated(), UpdateRate.SIM_200ms);
		this.death.Enter("SuffocationDeath", delegate(SuitSuffocationMonitor.Instance smi)
		{
			smi.Kill();
		});
	}

	public SuitSuffocationMonitor.SatisfiedState satisfied;

	public SuitSuffocationMonitor.NoOxygenState nooxygen;

	public GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.State death;

	public class NoOxygenState : GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.State holdingbreath;

		public GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.State suffocating;
	}

	public class SatisfiedState : GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.State normal;

		public GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.State low;
	}

	public new class Instance : GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, SuitTank suit_tank)
			: base(master)
		{
			this.breath = Db.Get().Amounts.Breath.Lookup(master.gameObject);
			Klei.AI.Attribute deltaAttribute = Db.Get().Amounts.Breath.deltaAttribute;
			float num = 0.90909094f;
			this.breathing = new AttributeModifier(deltaAttribute.Id, num, DUPLICANTS.MODIFIERS.BREATHING.NAME, false, false, true);
			this.holdingbreath = new AttributeModifier(deltaAttribute.Id, -num, DUPLICANTS.MODIFIERS.HOLDINGBREATH.NAME, false, false, true);
			this.suitTank = suit_tank;
		}

		public SuitTank suitTank { get; private set; }

		public bool IsTankEmpty()
		{
			return this.suitTank.IsEmpty();
		}

		public bool HasSuffocated()
		{
			return this.breath.value <= 0f;
		}

		public bool IsSuffocating()
		{
			return this.breath.value <= 45.454544f;
		}

		public void Kill()
		{
			base.gameObject.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Suffocation);
		}

		private AmountInstance breath;

		public AttributeModifier breathing;

		public AttributeModifier holdingbreath;

		private OxygenBreather masterOxygenBreather;
	}
}
