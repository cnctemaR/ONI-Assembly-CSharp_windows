using System;
using Klei.AI;
using STRINGS;

public class HappinessMonitor : GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.Transition(this.happy, new StateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Transition.ConditionCallback(HappinessMonitor.IsHappy), UpdateRate.SIM_1000ms).Transition(this.unhappy, GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Not(new StateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Transition.ConditionCallback(HappinessMonitor.IsHappy)), UpdateRate.SIM_1000ms);
		this.happy.DefaultState(this.happy.wild).Transition(this.satisfied, GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Not(new StateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Transition.ConditionCallback(HappinessMonitor.IsHappy)), UpdateRate.SIM_1000ms);
		this.happy.wild.ToggleEffect((HappinessMonitor.Instance smi) => this.happyWildEffect).TagTransition(GameTags.Creatures.Wild, this.happy.tame, true);
		this.happy.tame.ToggleEffect((HappinessMonitor.Instance smi) => this.happyTameEffect).TagTransition(GameTags.Creatures.Wild, this.happy.wild, false);
		this.unhappy.DefaultState(this.unhappy.wild).Transition(this.satisfied, new StateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Transition.ConditionCallback(HappinessMonitor.IsHappy), UpdateRate.SIM_1000ms).ToggleTag(GameTags.Creatures.Unhappy);
		this.unhappy.wild.ToggleEffect((HappinessMonitor.Instance smi) => this.unhappyWildEffect).TagTransition(GameTags.Creatures.Wild, this.unhappy.tame, true);
		this.unhappy.tame.ToggleEffect((HappinessMonitor.Instance smi) => this.unhappyTameEffect).TagTransition(GameTags.Creatures.Wild, this.unhappy.wild, false);
		this.happyWildEffect = new Effect("Happy", CREATURES.MODIFIERS.HAPPY.NAME, CREATURES.MODIFIERS.HAPPY.TOOLTIP, 0f, true, false, false, null, 0f, null, "");
		this.happyTameEffect = new Effect("Happy", CREATURES.MODIFIERS.HAPPY.NAME, CREATURES.MODIFIERS.HAPPY.TOOLTIP, 0f, true, false, false, null, 0f, null, "");
		this.happyTameEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, 9f, CREATURES.MODIFIERS.HAPPY.NAME, true, false, true));
		this.unhappyWildEffect = new Effect("Unhappy", CREATURES.MODIFIERS.UNHAPPY.NAME, CREATURES.MODIFIERS.UNHAPPY.TOOLTIP, 0f, true, false, true, null, 0f, null, "");
		this.unhappyWildEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -15f, CREATURES.MODIFIERS.UNHAPPY.NAME, false, false, true));
		this.unhappyTameEffect = new Effect("Unhappy", CREATURES.MODIFIERS.UNHAPPY.NAME, CREATURES.MODIFIERS.UNHAPPY.TOOLTIP, 0f, true, false, true, null, 0f, null, "");
		this.unhappyTameEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -80f, CREATURES.MODIFIERS.UNHAPPY.NAME, false, false, true));
	}

	private static bool IsHappy(HappinessMonitor.Instance smi)
	{
		return smi.happiness.GetTotalValue() >= smi.def.threshold;
	}

	private GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State satisfied;

	private HappinessMonitor.HappyState happy;

	private HappinessMonitor.UnhappyState unhappy;

	private Effect happyWildEffect;

	private Effect happyTameEffect;

	private Effect unhappyWildEffect;

	private Effect unhappyTameEffect;

	public class Def : StateMachine.BaseDef
	{
		public float threshold;
	}

	public class UnhappyState : GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State
	{
		public GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State wild;

		public GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State tame;
	}

	public class HappyState : GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State
	{
		public GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State wild;

		public GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State tame;
	}

	public new class Instance : GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, HappinessMonitor.Def def)
			: base(master, def)
		{
			this.happiness = base.gameObject.GetAttributes().Add(Db.Get().CritterAttributes.Happiness);
		}

		public AttributeInstance happiness;
	}
}
