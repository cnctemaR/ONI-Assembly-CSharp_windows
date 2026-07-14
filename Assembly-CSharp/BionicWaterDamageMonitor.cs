using System;
using Klei.AI;

public class BionicWaterDamageMonitor : GameStateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.safe;
		this.safe.Transition(this.suffering, new StateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.Transition.ConditionCallback(BionicWaterDamageMonitor.IsSuffering), UpdateRate.SIM_200ms);
		this.suffering.Transition(this.safe, GameStateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.Not(new StateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.Transition.ConditionCallback(BionicWaterDamageMonitor.IsSuffering)), UpdateRate.SIM_200ms).ToggleEffect("BionicWaterStress").ToggleReactable(new Func<BionicWaterDamageMonitor.Instance, Reactable>(BionicWaterDamageMonitor.ZapReactable));
	}

	private static Reactable ZapReactable(BionicWaterDamageMonitor.Instance smi)
	{
		return smi.GetZapReactable();
	}

	private static bool IsSuffering(BionicWaterDamageMonitor.Instance smi)
	{
		return BionicWaterDamageMonitor.IsFloorWetWithIntolerantSubstance(smi);
	}

	private static bool IsFloorWetWithIntolerantSubstance(BionicWaterDamageMonitor.Instance smi)
	{
		if (smi.master.gameObject.HasTag(GameTags.InTransitTube))
		{
			return false;
		}
		int num = Grid.PosToCell(smi);
		return Grid.IsValidCell(num) && Grid.Element[num].IsLiquid && !smi.kpid.HasTag(GameTags.HasAirtightSuit) && smi.def.IsElementIntolerable(Grid.Element[num]) && (!smi.kpid.HasTag(GameTags.FeetProtection) || Grid.IsSubstantialLiquid(num, 0.1f)) && (!smi.kpid.HasTag(GameTags.FeetAndWaistProtection) || Grid.IsSubstantialLiquid(num, 0.35f));
	}

	public const string EFFECT_NAME = "BionicWaterStress";

	public GameStateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.State safe;

	public GameStateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.State suffering;

	public class Def : StateMachine.BaseDef
	{
		public bool IsElementIntolerable(Element element)
		{
			return element != null && element.HasTag(GameTags.AnyWater);
		}

		public static float ZapInterval = 10f;
	}

	public new class Instance : GameStateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.GameInstance
	{
		public bool IsAffectedByWaterDamage
		{
			get
			{
				return this.effects.HasEffect("BionicWaterStress");
			}
		}

		public Instance(IStateMachineTarget master, BionicWaterDamageMonitor.Def def)
			: base(master, def)
		{
			this.effects = base.GetComponent<Effects>();
		}

		public Reactable GetZapReactable()
		{
			SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(base.master.gameObject, Db.Get().Emotes.Minion.WaterDamage.Id, Db.Get().ChoreTypes.WaterDamageZap, 0f, BionicWaterDamageMonitor.Def.ZapInterval, float.PositiveInfinity, 0f);
			Emote waterDamage = Db.Get().Emotes.Minion.WaterDamage;
			selfEmoteReactable.SetEmote(waterDamage);
			selfEmoteReactable.preventChoreInterruption = true;
			return selfEmoteReactable;
		}

		public Effects effects;

		[MyCmpGet]
		public KPrefabID kpid;
	}
}
