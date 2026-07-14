using System;
using Klei.AI;
using STRINGS;
using TUNING;

[SkipSaveFileSerialization]
public class MinnowSwimmer : StateMachineComponent<MinnowSwimmer.StatesInstance>
{
	protected override void OnSpawn()
	{
		string[] all_ATTRIBUTES = DUPLICANTSTATS.ALL_ATTRIBUTES;
		this.attributeModifiers = new AttributeModifier[all_ATTRIBUTES.Length];
		for (int i = 0; i < all_ATTRIBUTES.Length; i++)
		{
			this.attributeModifiers[i] = new AttributeModifier(all_ATTRIBUTES[i], 4f, DUPLICANTS.CONGENITALTRAITS.MINNOW.NAME, false, false, true);
		}
		base.smi.StartSM();
	}

	public void ApplyModifiers()
	{
		Attributes attributes = base.gameObject.GetAttributes();
		for (int i = 0; i < this.attributeModifiers.Length; i++)
		{
			attributes.Add(this.attributeModifiers[i]);
		}
	}

	public void RemoveModifiers()
	{
		Attributes attributes = base.gameObject.GetAttributes();
		for (int i = 0; i < this.attributeModifiers.Length; i++)
		{
			attributes.Remove(this.attributeModifiers[i]);
		}
	}

	[MyCmpReq]
	private KPrefabID kPrefabID;

	[MyCmpGet]
	private Navigator navigator;

	private AttributeModifier[] attributeModifiers;

	public class StatesInstance : GameStateMachine<MinnowSwimmer.States, MinnowSwimmer.StatesInstance, MinnowSwimmer, object>.GameInstance
	{
		public StatesInstance(MinnowSwimmer master)
			: base(master)
		{
		}

		public bool IsSwimming()
		{
			return base.master.navigator != null && base.master.navigator.IsSwimming();
		}
	}

	public class States : GameStateMachine<MinnowSwimmer.States, MinnowSwimmer.StatesInstance, MinnowSwimmer>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.root.TagTransition(GameTags.Dead, null, false);
			this.idle.Transition(this.swimming, (MinnowSwimmer.StatesInstance smi) => smi.IsSwimming(), UpdateRate.SIM_200ms);
			this.swimming.Enter("Swimming", delegate(MinnowSwimmer.StatesInstance smi)
			{
				smi.master.ApplyModifiers();
			}).Exit("NotSwimming", delegate(MinnowSwimmer.StatesInstance smi)
			{
				smi.master.RemoveModifiers();
			}).Transition(this.idle, (MinnowSwimmer.StatesInstance smi) => !smi.IsSwimming(), UpdateRate.SIM_200ms);
		}

		public GameStateMachine<MinnowSwimmer.States, MinnowSwimmer.StatesInstance, MinnowSwimmer, object>.State idle;

		public GameStateMachine<MinnowSwimmer.States, MinnowSwimmer.StatesInstance, MinnowSwimmer, object>.State swimming;
	}
}
