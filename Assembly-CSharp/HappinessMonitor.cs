using System;
using Klei;
using Klei.AI;
using STRINGS;

public class HappinessMonitor : GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.root.Update(delegate(HappinessMonitor.Instance smi, float dt)
		{
			smi.Update(dt);
		}, UpdateRate.SIM_200ms, false);
		this.satisfied.Transition(this.happy, (HappinessMonitor.Instance smi) => smi.happiness.GetTotalValue() >= smi.def.threshold, UpdateRate.SIM_1000ms).Transition(this.unhappy, (HappinessMonitor.Instance smi) => smi.happiness.GetTotalValue() < smi.def.threshold, UpdateRate.SIM_1000ms);
		this.happy.Transition(this.satisfied, (HappinessMonitor.Instance smi) => smi.happiness.GetTotalValue() < smi.def.threshold, UpdateRate.SIM_1000ms).ToggleEffect((HappinessMonitor.Instance smi) => this.happyEffect);
		this.unhappy.DefaultState(this.unhappy.wild).Transition(this.satisfied, (HappinessMonitor.Instance smi) => smi.happiness.GetTotalValue() >= smi.def.threshold, UpdateRate.SIM_1000ms);
		this.unhappy.wild.ToggleEffect((HappinessMonitor.Instance smi) => this.unhappyWildEffect).TagTransition(GameTags.Creatures.Wild, this.unhappy.tame, true);
		this.unhappy.tame.ToggleEffect((HappinessMonitor.Instance smi) => this.unhappyTameEffect).TagTransition(GameTags.Creatures.Wild, this.unhappy.wild, false);
		this.happyEffect = new Effect("Happy", CREATURES.MODIFIERS.HAPPY.NAME, CREATURES.MODIFIERS.HAPPY.TOOLTIP, 0f, true, false, false);
		this.unhappyWildEffect = new Effect("Unhappy", CREATURES.MODIFIERS.UNHAPPY.NAME, CREATURES.MODIFIERS.UNHAPPY.TOOLTIP, 0f, true, false, true);
		if (!GenericGameSettings.instance.acceleratedLifecycle)
		{
			this.unhappyWildEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, -1f, CREATURES.MODIFIERS.UNHAPPY.NAME, true, false, true));
		}
		this.unhappyWildEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -15f, CREATURES.MODIFIERS.UNHAPPY.NAME, false, false, true));
		this.unhappyTameEffect = new Effect("Unhappy", CREATURES.MODIFIERS.UNHAPPY.NAME, CREATURES.MODIFIERS.UNHAPPY.TOOLTIP, 0f, true, false, true);
		if (!GenericGameSettings.instance.acceleratedLifecycle)
		{
			this.unhappyTameEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, -1f, CREATURES.MODIFIERS.UNHAPPY.NAME, true, false, true));
		}
		this.unhappyTameEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -80f, CREATURES.MODIFIERS.UNHAPPY.NAME, false, false, true));
	}

	public const float UNHAPPY_FERTILITY_DEBUFF = -1f;

	private GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State satisfied;

	private GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State happy;

	private HappinessMonitor.UnhappyState unhappy;

	private Effect happyEffect;

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

	public new class Instance : GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, HappinessMonitor.Def def)
			: base(master, def)
		{
			this.happiness = base.gameObject.GetAttributes().Add(Db.Get().CritterAttributes.Happiness);
			this.happinessAmount = base.gameObject.GetAmounts().Add(new AmountInstance(Db.Get().Amounts.Happiness, base.gameObject));
		}

		public void Update(float dt)
		{
			this.happinessAmount.value = this.happiness.GetTotalValue();
		}

		public AttributeInstance happiness;

		public AmountInstance happinessAmount;
	}
}
