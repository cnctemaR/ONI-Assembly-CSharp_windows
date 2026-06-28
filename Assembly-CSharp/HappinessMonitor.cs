using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class HappinessMonitor : GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.Transition(this.happy, (HappinessMonitor.Instance smi) => smi.happiness.value >= smi.def.happyThreshold, UpdateRate.SIM_1000ms);
		this.happy.Transition(this.satisfied, (HappinessMonitor.Instance smi) => smi.happiness.value < smi.def.happyThreshold, UpdateRate.SIM_1000ms).ToggleEffect((HappinessMonitor.Instance smi) => this.effect);
		float num = 15f;
		this.effect = new Effect("Happy", CREATURES.MODIFIERS.HAPPY.NAME, CREATURES.MODIFIERS.HAPPY.TOOLTIP, 0f, true, false, false);
		this.effect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, 100f / num / 600f, CREATURES.MODIFIERS.HAPPY.NAME, false, false, true));
	}

	private GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State satisfied;

	private GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State happy;

	private Effect effect;

	public class Def : StateMachine.BaseDef
	{
		public override void Configure(GameObject prefab)
		{
			prefab.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Happiness.Id);
		}

		public float happyThreshold = 75f;

		public float startingHappiness = 50f;
	}

	public new class Instance : GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, HappinessMonitor.Def def)
			: base(master, def)
		{
			this.happiness = Db.Get().Amounts.Happiness.Lookup(base.gameObject);
			this.happiness.value = base.smi.def.startingHappiness;
		}

		public AmountInstance happiness;
	}
}
