using System;
using Klei.AI;
using STRINGS;
using TUNING;

[SkipSaveFileSerialization]
public class NightOwl : StateMachineComponent<NightOwl.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.Subscribe(1623392196, new Action<object>(this.OnDeath));
		base.Subscribe(-1117766961, new Action<object>(this.OnRevived));
	}

	protected override void OnSpawn()
	{
		this.attributeModifiers = new AttributeModifier[]
		{
			new AttributeModifier("Construction", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Digging", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Machinery", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Athletics", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Learning", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Cooking", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Art", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Strength", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Caring", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Botanist", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true),
			new AttributeModifier("Ranching", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME, false, false, true)
		};
		base.smi.StartSM();
	}

	public void ApplyModifiers()
	{
		Attributes attributes = base.gameObject.GetAttributes();
		for (int i = 0; i < this.attributeModifiers.Length; i++)
		{
			AttributeModifier attributeModifier = this.attributeModifiers[i];
			attributes.Add(attributeModifier);
		}
	}

	public void RemoveModifiers()
	{
		Attributes attributes = base.gameObject.GetAttributes();
		for (int i = 0; i < this.attributeModifiers.Length; i++)
		{
			AttributeModifier attributeModifier = this.attributeModifiers[i];
			attributes.Remove(attributeModifier);
		}
	}

	private void OnDeath(object data)
	{
		base.enabled = false;
	}

	private void OnRevived(object data)
	{
		base.enabled = true;
	}

	public void ModifyTrait(Trait t)
	{
	}

	[MyCmpReq]
	private KPrefabID kPrefabID;

	private AttributeModifier[] attributeModifiers;

	public class StatesInstance : GameStateMachine<NightOwl.States, NightOwl.StatesInstance, NightOwl, object>.GameInstance
	{
		public StatesInstance(NightOwl master)
			: base(master)
		{
		}

		public bool IsNight()
		{
			return !(GameClock.Instance == null) && !(base.master.kPrefabID.PrefabTag == GameTags.MinionSelectPreview) && GameClock.Instance.IsNighttime();
		}
	}

	public class States : GameStateMachine<NightOwl.States, NightOwl.StatesInstance, NightOwl>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.idle.Transition(this.early, (NightOwl.StatesInstance smi) => smi.IsNight(), UpdateRate.SIM_200ms);
			this.early.Enter("Night", delegate(NightOwl.StatesInstance smi)
			{
				smi.master.ApplyModifiers();
			}).Exit("NotNight", delegate(NightOwl.StatesInstance smi)
			{
				smi.master.RemoveModifiers();
			}).ToggleStatusItem(Db.Get().DuplicantStatusItems.NightTime, null)
				.ToggleExpression(Db.Get().Expressions.Happy, null)
				.Transition(this.idle, (NightOwl.StatesInstance smi) => !smi.IsNight(), UpdateRate.SIM_200ms);
		}

		public GameStateMachine<NightOwl.States, NightOwl.StatesInstance, NightOwl, object>.State idle;

		public GameStateMachine<NightOwl.States, NightOwl.StatesInstance, NightOwl, object>.State early;
	}
}
