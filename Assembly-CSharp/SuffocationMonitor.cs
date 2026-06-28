using System;
using Klei.AI;
using STRINGS;

public class SuffocationMonitor : GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.EventTransition(GameHashes.ExitedBreathableArea, this.nooxygen, (SuffocationMonitor.Instance smi) => !smi.IsInBreathableArea() && !smi.IsUsingTank()).Transition(this.satisfied.fullbreath, (SuffocationMonitor.Instance smi) => smi.IsFullBreath()).ToggleAttributeModifier("Breathing", (SuffocationMonitor.Instance smi) => smi.breathing);
		this.satisfied.fullbreath.EventTransition(GameHashes.ExitedBreathableArea, this.onTank, (SuffocationMonitor.Instance smi) => !smi.IsInBreathableArea() && smi.IsUsingTank()).EventTransition(GameHashes.ExitedBreathableArea, this.nooxygen, (SuffocationMonitor.Instance smi) => !smi.IsInBreathableArea() && !smi.IsUsingTank()).Enter("OxygenProgressBar", delegate(SuffocationMonitor.Instance smi)
		{
			if (NameDisplayScreen.Instance != null)
			{
				NameDisplayScreen.Instance.SetBreathDisplay(smi.master.gameObject, new Func<float>(smi.GetBreath), false);
			}
		});
		this.onTank.EventTransition(GameHashes.EnteredBreathableArea, this.satisfied, (SuffocationMonitor.Instance smi) => smi.IsInBreathableArea()).Transition(this.nooxygen, (SuffocationMonitor.Instance smi) => smi.IsTankEmpty() || (smi.IsUnderwater() && !smi.IsUsingUnderwaterTank())).Enter("OxygenTankProgressBar", delegate(SuffocationMonitor.Instance smi)
		{
			if (NameDisplayScreen.Instance != null)
			{
				NameDisplayScreen.Instance.SetSuitTankDisplay(smi.master.gameObject, new Func<float>(smi.GetTankPercentage), true);
				NameDisplayScreen.Instance.SetBreathDisplay(smi.master.gameObject, new Func<float>(smi.GetBreath), true);
			}
		});
		this.nooxygen.EventTransition(GameHashes.EnteredBreathableArea, this.satisfied, (SuffocationMonitor.Instance smi) => smi.IsInBreathableArea() || (smi.IsUsingTank() && !smi.IsUnderwater()) || (smi.IsUsingUnderwaterTank() && smi.IsUnderwater() && !smi.IsTankEmpty())).ToggleExpression(Db.Get().Expressions.Suffocate, null).ToggleAttributeModifier("Holding Breath", (SuffocationMonitor.Instance smi) => smi.holdingbreath)
			.Enter("OxygenProgressBar", delegate(SuffocationMonitor.Instance smi)
			{
				NameDisplayScreen.Instance.SetBreathDisplay(smi.master.gameObject, new Func<float>(smi.GetBreath), true);
			})
			.DefaultState(this.nooxygen.holdingbreath);
		this.nooxygen.holdingbreath.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.HoldingBreath, null).Transition(this.nooxygen.suffocating, (SuffocationMonitor.Instance smi) => smi.IsSuffocating());
		this.nooxygen.suffocating.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.Suffocating, null).Transition(this.death, (SuffocationMonitor.Instance smi) => smi.HasSuffocated());
		this.death.Enter("SuffocationDeath", delegate(SuffocationMonitor.Instance smi)
		{
			smi.Kill();
		});
	}

	public SuffocationMonitor.Statisfied satisfied;

	public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget>.State onTank;

	public SuffocationMonitor.NoOxygenState nooxygen;

	public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget>.State death;

	public class NoOxygenState : GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget>.State
	{
		public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget>.State holdingbreath;

		public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget>.State suffocating;
	}

	public class Statisfied : GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget>.State
	{
		public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget>.State fullbreath;
	}

	public new class Instance : GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.breath = Db.Get().Amounts.Breath.Lookup(master.gameObject);
			Klei.AI.Attribute deltaAttribute = Db.Get().Amounts.Breath.deltaAttribute;
			float num = 0.90909094f;
			this.breathing = new AttributeModifier(deltaAttribute.Id, num, DUPLICANTS.MODIFIERS.BREATHING.NAME, false);
			this.holdingbreath = new AttributeModifier(deltaAttribute.Id, -num, DUPLICANTS.MODIFIERS.HOLDINGBREATH.NAME, false);
			NameDisplayScreen.Instance.RegisterComponent(base.gameObject, this);
		}

		public float GetBreath()
		{
			return this.breath.value / this.breath.GetMax();
		}

		public float GetTankPercentage()
		{
			if (this.masterOxygenBreather == null)
			{
				this.masterOxygenBreather = base.master.GetComponent<OxygenBreather>();
			}
			return this.masterOxygenBreather.SuitTank.PercentFull();
		}

		public bool IsInBreathableArea()
		{
			return base.master.GetComponent<Sensors>().GetSensor<BreathableAreaSensor>().IsBreathable();
		}

		public bool IsUnderwater()
		{
			return base.master.GetComponent<Sensors>().GetSensor<BreathableAreaSensor>().IsUnderwater();
		}

		public bool IsTankEmpty()
		{
			return base.master.GetComponent<OxygenBreather>().SuitTank.IsEmpty();
		}

		public bool IsUsingTank()
		{
			if (this.masterOxygenBreather == null)
			{
				this.masterOxygenBreather = base.master.GetComponent<OxygenBreather>();
			}
			return this.masterOxygenBreather.IsUsingOxygenTank;
		}

		public bool IsUsingUnderwaterTank()
		{
			if (this.masterOxygenBreather == null)
			{
				this.masterOxygenBreather = base.master.GetComponent<OxygenBreather>();
			}
			return this.masterOxygenBreather.SuitTank != null && this.masterOxygenBreather.SuitTank.underwaterSupport;
		}

		public bool HasSuffocated()
		{
			return this.breath.value <= 0f;
		}

		public bool IsSuffocating()
		{
			return this.breath.value <= 45.454544f;
		}

		public bool IsFullBreath()
		{
			return this.breath.value == this.breath.GetMax();
		}

		public void Kill()
		{
			base.GetComponent<Health>().Kill(Db.Get().Deaths.Suffocation);
		}

		private AmountInstance breath;

		public AttributeModifier breathing;

		public AttributeModifier holdingbreath;

		private OxygenBreather masterOxygenBreather;
	}
}
