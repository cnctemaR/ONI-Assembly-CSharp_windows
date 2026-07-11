using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class SuffocationMonitor : GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.root.Update("CheckOverPressure", delegate(SuffocationMonitor.Instance smi, float dt)
		{
			smi.CheckOverPressure();
		}, UpdateRate.SIM_200ms, false).TagTransition(GameTags.Dead, this.dead, false);
		this.satisfied.DefaultState(this.satisfied.normal).ToggleAttributeModifier("Breathing", (SuffocationMonitor.Instance smi) => smi.breathing, null).EventTransition(GameHashes.ExitedBreathableArea, this.nooxygen, (SuffocationMonitor.Instance smi) => !smi.IsInBreathableArea());
		this.satisfied.normal.Transition(this.satisfied.low, (SuffocationMonitor.Instance smi) => smi.oxygenBreather.IsLowOxygen(), UpdateRate.SIM_200ms);
		this.satisfied.low.Transition(this.satisfied.normal, (SuffocationMonitor.Instance smi) => !smi.oxygenBreather.IsLowOxygen(), UpdateRate.SIM_200ms).ToggleEffect("LowOxygen");
		this.nooxygen.EventTransition(GameHashes.EnteredBreathableArea, this.satisfied, (SuffocationMonitor.Instance smi) => smi.IsInBreathableArea()).TagTransition(GameTags.RecoveringBreath, this.satisfied, false).ToggleExpression(Db.Get().Expressions.Suffocate, null)
			.ToggleAttributeModifier("Holding Breath", (SuffocationMonitor.Instance smi) => smi.holdingbreath, null)
			.ToggleTag(GameTags.NoOxygen)
			.DefaultState(this.nooxygen.holdingbreath);
		this.nooxygen.holdingbreath.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.HoldingBreath, null).Transition(this.nooxygen.suffocating, (SuffocationMonitor.Instance smi) => smi.IsSuffocating(), UpdateRate.SIM_200ms);
		this.nooxygen.suffocating.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.Suffocating, null).Transition(this.death, (SuffocationMonitor.Instance smi) => smi.HasSuffocated(), UpdateRate.SIM_200ms);
		this.death.Enter("SuffocationDeath", delegate(SuffocationMonitor.Instance smi)
		{
			smi.Kill();
		});
		this.dead.DoNothing();
	}

	public SuffocationMonitor.SatisfiedState satisfied;

	public SuffocationMonitor.NoOxygenState nooxygen;

	public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State death;

	public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State dead;

	public class NoOxygenState : GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State holdingbreath;

		public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State suffocating;
	}

	public class SatisfiedState : GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State normal;

		public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State low;
	}

	public new class Instance : GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(OxygenBreather oxygen_breather)
			: base(oxygen_breather)
		{
			this.breath = Db.Get().Amounts.Breath.Lookup(base.master.gameObject);
			Klei.AI.Attribute deltaAttribute = Db.Get().Amounts.Breath.deltaAttribute;
			float num = 0.90909094f;
			this.breathing = new AttributeModifier(deltaAttribute.Id, num, DUPLICANTS.MODIFIERS.BREATHING.NAME, false, false, true);
			this.holdingbreath = new AttributeModifier(deltaAttribute.Id, -num, DUPLICANTS.MODIFIERS.HOLDINGBREATH.NAME, false, false, true);
			this.oxygenBreather = oxygen_breather;
		}

		public OxygenBreather oxygenBreather { get; private set; }

		public bool IsInBreathableArea()
		{
			return base.master.GetComponent<KPrefabID>().HasTag(GameTags.RecoveringBreath) || base.master.GetComponent<Sensors>().GetSensor<BreathableAreaSensor>().IsBreathable();
		}

		public bool HasSuffocated()
		{
			return this.breath.value <= 0f;
		}

		public bool IsSuffocating()
		{
			return this.breath.deltaAttribute.GetTotalValue() <= 0f && this.breath.value <= 45.454544f;
		}

		public void Kill()
		{
			base.gameObject.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Suffocation);
		}

		public void CheckOverPressure()
		{
			if (this.IsInHighPressure())
			{
				if (!this.wasInHighPressure)
				{
					this.wasInHighPressure = true;
					this.highPressureTime = Time.time;
				}
				else if (Time.time - this.highPressureTime > 3f)
				{
					base.master.GetComponent<Effects>().Add("PoppedEarDrums", true);
				}
			}
			else
			{
				this.wasInHighPressure = false;
			}
		}

		private bool IsInHighPressure()
		{
			int num = Grid.PosToCell(base.gameObject);
			for (int i = 0; i < SuffocationMonitor.Instance.pressureTestOffsets.Length; i++)
			{
				int num2 = Grid.OffsetCell(num, SuffocationMonitor.Instance.pressureTestOffsets[i]);
				if (Grid.IsValidCell(num2) && Grid.Element[num2].IsGas && Grid.Mass[num2] > 4f)
				{
					return true;
				}
			}
			return false;
		}

		private AmountInstance breath;

		public AttributeModifier breathing;

		public AttributeModifier holdingbreath;

		private static CellOffset[] pressureTestOffsets = new CellOffset[]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1)
		};

		private const float HIGH_PRESSURE_DELAY = 3f;

		private bool wasInHighPressure;

		private float highPressureTime;
	}
}
