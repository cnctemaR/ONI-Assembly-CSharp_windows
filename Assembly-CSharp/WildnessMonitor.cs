using System;
using Klei.AI;
using UnityEngine;

public class WildnessMonitor : GameStateMachine<WildnessMonitor, WildnessMonitor.Instance, IStateMachineTarget, WildnessMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.tame;
		base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
		this.wild.Enter(new StateMachine<WildnessMonitor, WildnessMonitor.Instance, IStateMachineTarget, WildnessMonitor.Def>.State.Callback(WildnessMonitor.RefreshAmounts)).Enter(new StateMachine<WildnessMonitor, WildnessMonitor.Instance, IStateMachineTarget, WildnessMonitor.Def>.State.Callback(WildnessMonitor.HideDomesticationSymbol)).Transition(this.tame, (WildnessMonitor.Instance smi) => !WildnessMonitor.IsWild(smi), UpdateRate.SIM_1000ms)
			.ToggleEffect((WildnessMonitor.Instance smi) => smi.def.wildEffect)
			.ToggleTag(GameTags.Creatures.Wild);
		this.tame.Enter(new StateMachine<WildnessMonitor, WildnessMonitor.Instance, IStateMachineTarget, WildnessMonitor.Def>.State.Callback(WildnessMonitor.RefreshAmounts)).Enter(new StateMachine<WildnessMonitor, WildnessMonitor.Instance, IStateMachineTarget, WildnessMonitor.Def>.State.Callback(WildnessMonitor.ShowDomesticationSymbol)).Transition(this.wild, new StateMachine<WildnessMonitor, WildnessMonitor.Instance, IStateMachineTarget, WildnessMonitor.Def>.Transition.ConditionCallback(WildnessMonitor.IsWild), UpdateRate.SIM_1000ms)
			.ToggleEffect((WildnessMonitor.Instance smi) => smi.def.tameEffect)
			.Enter(delegate(WildnessMonitor.Instance smi)
			{
				SaveGame.Instance.ColonyAchievementTracker.LogCritterTamed(smi.PrefabID());
			});
	}

	private static void HideDomesticationSymbol(WildnessMonitor.Instance smi)
	{
		foreach (KAnimHashedString kanimHashedString in WildnessMonitor.DOMESTICATION_SYMBOLS)
		{
			smi.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(kanimHashedString, false);
		}
	}

	private static void ShowDomesticationSymbol(WildnessMonitor.Instance smi)
	{
		foreach (KAnimHashedString kanimHashedString in WildnessMonitor.DOMESTICATION_SYMBOLS)
		{
			smi.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(kanimHashedString, true);
		}
	}

	private static bool IsWild(WildnessMonitor.Instance smi)
	{
		return smi.wildness.value > 0f;
	}

	private static void RefreshAmounts(WildnessMonitor.Instance smi)
	{
		bool flag = WildnessMonitor.IsWild(smi);
		smi.wildness.hide = !flag;
		AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(smi.gameObject);
		if (amountInstance != null)
		{
			amountInstance.hide = flag;
		}
	}

	public GameStateMachine<WildnessMonitor, WildnessMonitor.Instance, IStateMachineTarget, WildnessMonitor.Def>.State wild;

	public GameStateMachine<WildnessMonitor, WildnessMonitor.Instance, IStateMachineTarget, WildnessMonitor.Def>.State tame;

	private static readonly KAnimHashedString[] DOMESTICATION_SYMBOLS = new KAnimHashedString[] { "tag", "snapto_tag" };

	public class Def : StateMachine.BaseDef
	{
		public override void Configure(GameObject prefab)
		{
			prefab.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Wildness.Id);
		}

		public Effect wildEffect;

		public Effect tameEffect;
	}

	public new class Instance : GameStateMachine<WildnessMonitor, WildnessMonitor.Instance, IStateMachineTarget, WildnessMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, WildnessMonitor.Def def)
			: base(master, def)
		{
			this.wildness = Db.Get().Amounts.Wildness.Lookup(base.gameObject);
			this.wildness.value = this.wildness.GetMax();
		}

		public bool IsWild()
		{
			return WildnessMonitor.IsWild(this);
		}

		[ContextMenu("Tame Critter")]
		public void DebugTame()
		{
			AmountInstance amountInstance = Db.Get().Amounts.Wildness.Lookup(base.gameObject);
			if (amountInstance != null)
			{
				amountInstance.value = 0f;
				base.smi.GoTo(base.sm.tame);
			}
		}

		public AmountInstance wildness;
	}
}
