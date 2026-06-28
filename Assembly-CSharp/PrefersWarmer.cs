using System;
using Klei.AI;
using STRINGS;

[SkipSaveFileSerialization]
public class PrefersWarmer : StateMachineComponent<PrefersWarmer.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	public class StatesInstance : GameStateMachine<PrefersWarmer.States, PrefersWarmer.StatesInstance, PrefersWarmer, object>.GameInstance
	{
		public StatesInstance(PrefersWarmer master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<PrefersWarmer.States, PrefersWarmer.StatesInstance, PrefersWarmer>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.root;
			this.root.ToggleAttributeModifier(DUPLICANTS.TRAITS.NEEDS.PREFERSWARMER.NAME, (PrefersWarmer.StatesInstance smi) => this.modifier, null);
		}

		private AttributeModifier modifier = new AttributeModifier("ThermalConductivityBarrier", -0.005f, DUPLICANTS.TRAITS.NEEDS.PREFERSWARMER.NAME, false, false, true);
	}
}
