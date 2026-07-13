using System;
using Klei.AI;
using STRINGS;

public class HappinessMonitor : GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.Transition(this.happy, new StateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Transition.ConditionCallback(HappinessMonitor.IsHappy), UpdateRate.SIM_1000ms).Transition(this.neutral, new StateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Transition.ConditionCallback(HappinessMonitor.IsNeutral), UpdateRate.SIM_1000ms).Transition(this.glum, new StateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Transition.ConditionCallback(HappinessMonitor.IsGlum), UpdateRate.SIM_1000ms)
			.Transition(this.miserable, new StateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Transition.ConditionCallback(HappinessMonitor.IsMisirable), UpdateRate.SIM_1000ms);
		this.happy.DefaultState(this.happy.wild).Transition(this.satisfied, GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Not(new StateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Transition.ConditionCallback(HappinessMonitor.IsHappy)), UpdateRate.SIM_1000ms).ToggleTag(GameTags.Creatures.Happy)
			.ToggleCritterEmotion(Db.Get().CritterEmotions.Happy, null);
		this.happy.wild.ToggleEffect((HappinessMonitor.Instance smi) => this.happyWildEffect).TagTransition(GameTags.Creatures.Wild, this.happy.tame, true);
		this.happy.tame.ToggleEffect((HappinessMonitor.Instance smi) => this.happyTameEffect).TagTransition(GameTags.Creatures.Wild, this.happy.wild, false);
		this.neutral.DefaultState(this.neutral.wild).Transition(this.satisfied, GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Not(new StateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Transition.ConditionCallback(HappinessMonitor.IsNeutral)), UpdateRate.SIM_1000ms);
		this.neutral.wild.ToggleEffect((HappinessMonitor.Instance smi) => this.neutralWildEffect).TagTransition(GameTags.Creatures.Wild, this.neutral.tame, true);
		this.neutral.tame.ToggleEffect((HappinessMonitor.Instance smi) => this.neutralTameEffect).TagTransition(GameTags.Creatures.Wild, this.neutral.wild, false);
		this.glum.DefaultState(this.glum.wild).Transition(this.satisfied, GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Not(new StateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Transition.ConditionCallback(HappinessMonitor.IsGlum)), UpdateRate.SIM_1000ms).ToggleTag(GameTags.Creatures.Unhappy);
		this.glum.wild.ToggleEffect((HappinessMonitor.Instance smi) => this.glumWildEffect).TagTransition(GameTags.Creatures.Wild, this.glum.tame, true);
		this.glum.tame.ToggleEffect((HappinessMonitor.Instance smi) => this.glumTameEffect).TagTransition(GameTags.Creatures.Wild, this.glum.wild, false);
		this.miserable.DefaultState(this.miserable.wild).Transition(this.satisfied, GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Not(new StateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Transition.ConditionCallback(HappinessMonitor.IsMisirable)), UpdateRate.SIM_1000ms).ToggleTag(GameTags.Creatures.Unhappy);
		this.miserable.wild.ToggleEffect((HappinessMonitor.Instance smi) => this.miserableWildEffect).TagTransition(GameTags.Creatures.Wild, this.miserable.tame, true);
		this.miserable.tame.ToggleEffect((HappinessMonitor.Instance smi) => this.miserableTameEffect).TagTransition(GameTags.Creatures.Wild, this.miserable.wild, false);
		this.happyWildEffect = new Effect("Happy", CREATURES.MODIFIERS.HAPPY_WILD.NAME, CREATURES.MODIFIERS.HAPPY_WILD.TOOLTIP, 0f, true, false, false, null, -1f, 0f, null, "");
		this.happyTameEffect = new Effect("Happy", CREATURES.MODIFIERS.HAPPY_TAME.NAME, CREATURES.MODIFIERS.HAPPY_TAME.TOOLTIP, 0f, true, false, false, null, -1f, 0f, null, "");
		this.neutralWildEffect = new Effect("Neutral", CREATURES.MODIFIERS.NEUTRAL.NAME, CREATURES.MODIFIERS.NEUTRAL.TOOLTIP, 0f, true, false, false, null, -1f, 0f, null, "");
		this.neutralTameEffect = new Effect("Neutral", CREATURES.MODIFIERS.NEUTRAL.NAME, CREATURES.MODIFIERS.NEUTRAL.TOOLTIP, 0f, true, false, false, null, -1f, 0f, null, "");
		this.glumWildEffect = new Effect("Glum", CREATURES.MODIFIERS.GLUM.NAME, CREATURES.MODIFIERS.GLUM.TOOLTIP, 0f, true, false, true, null, -1f, 0f, null, "");
		this.glumTameEffect = new Effect("Glum", CREATURES.MODIFIERS.GLUM.NAME, CREATURES.MODIFIERS.GLUM.TOOLTIP, 0f, true, false, true, null, -1f, 0f, null, "");
		this.miserableWildEffect = new Effect("Miserable", CREATURES.MODIFIERS.MISERABLE.NAME, CREATURES.MODIFIERS.MISERABLE.TOOLTIP, 0f, true, false, true, null, -1f, 0f, null, "");
		this.miserableTameEffect = new Effect("Miserable", CREATURES.MODIFIERS.MISERABLE.NAME, CREATURES.MODIFIERS.MISERABLE.TOOLTIP, 0f, true, false, true, null, -1f, 0f, null, "");
		this.happyTameEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, 9f, CREATURES.MODIFIERS.HAPPY_TAME.NAME, true, false, true));
		this.glumWildEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -15f, CREATURES.MODIFIERS.GLUM.NAME, false, false, true));
		this.glumTameEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -80f, CREATURES.MODIFIERS.GLUM.NAME, false, false, true));
		this.miserableTameEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -80f, CREATURES.MODIFIERS.MISERABLE.NAME, false, false, true));
		this.miserableTameEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, -1f, CREATURES.MODIFIERS.MISERABLE.NAME, true, false, true));
		this.miserableTameEffect.Add(new AttributeModifier(Db.Get().Amounts.ScaleGrowth.deltaAttribute.Id, -1f, CREATURES.MODIFIERS.MISERABLE.NAME, true, false, true));
		this.miserableWildEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -15f, CREATURES.MODIFIERS.MISERABLE.NAME, false, false, true));
		this.miserableWildEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, -1f, CREATURES.MODIFIERS.MISERABLE.NAME, true, false, true));
		this.miserableWildEffect.Add(new AttributeModifier(Db.Get().Amounts.ScaleGrowth.deltaAttribute.Id, -1f, CREATURES.MODIFIERS.MISERABLE.NAME, true, false, true));
	}

	private static bool IsHappy(HappinessMonitor.Instance smi)
	{
		return smi.happiness.GetTotalValue() >= smi.def.happyThreshold;
	}

	private static bool IsNeutral(HappinessMonitor.Instance smi)
	{
		float totalValue = smi.happiness.GetTotalValue();
		return totalValue > smi.def.glumThreshold && totalValue < smi.def.happyThreshold;
	}

	private static bool IsGlum(HappinessMonitor.Instance smi)
	{
		float totalValue = smi.happiness.GetTotalValue();
		return totalValue > smi.def.miserableThreshold && totalValue <= smi.def.glumThreshold;
	}

	private static bool IsMisirable(HappinessMonitor.Instance smi)
	{
		return smi.happiness.GetTotalValue() <= smi.def.miserableThreshold;
	}

	private GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State satisfied;

	private HappinessMonitor.HappyState happy;

	private HappinessMonitor.NeutralState neutral;

	private HappinessMonitor.UnhappyState glum;

	private HappinessMonitor.MiserableState miserable;

	private Effect happyWildEffect;

	private Effect happyTameEffect;

	private Effect neutralTameEffect;

	private Effect neutralWildEffect;

	private Effect glumWildEffect;

	private Effect glumTameEffect;

	private Effect miserableWildEffect;

	private Effect miserableTameEffect;

	public class Def : StateMachine.BaseDef
	{
		public float happyThreshold = 4f;

		public float glumThreshold = -1f;

		public float miserableThreshold = -10f;
	}

	public class MiserableState : GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State
	{
		public GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State wild;

		public GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State tame;
	}

	public class NeutralState : GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State
	{
		public GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State wild;

		public GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State tame;
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
