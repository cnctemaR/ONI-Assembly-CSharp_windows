using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;

[SkipSaveFileSerialization]
public class Meteorphile : StateMachineComponent<Meteorphile.StatesInstance>
{
	protected override void OnSpawn()
	{
		this.attributeModifiers = new AttributeModifier[]
		{
			new AttributeModifier("Construction", TRAITS.METEORPHILE_MODIFIER, DUPLICANTS.TRAITS.METEORPHILE.NAME, false, false, true),
			new AttributeModifier("Digging", TRAITS.METEORPHILE_MODIFIER, DUPLICANTS.TRAITS.METEORPHILE.NAME, false, false, true),
			new AttributeModifier("Machinery", TRAITS.METEORPHILE_MODIFIER, DUPLICANTS.TRAITS.METEORPHILE.NAME, false, false, true),
			new AttributeModifier("Athletics", TRAITS.METEORPHILE_MODIFIER, DUPLICANTS.TRAITS.METEORPHILE.NAME, false, false, true),
			new AttributeModifier("Learning", TRAITS.METEORPHILE_MODIFIER, DUPLICANTS.TRAITS.METEORPHILE.NAME, false, false, true),
			new AttributeModifier("Cooking", TRAITS.METEORPHILE_MODIFIER, DUPLICANTS.TRAITS.METEORPHILE.NAME, false, false, true),
			new AttributeModifier("Art", TRAITS.METEORPHILE_MODIFIER, DUPLICANTS.TRAITS.METEORPHILE.NAME, false, false, true),
			new AttributeModifier("Strength", TRAITS.METEORPHILE_MODIFIER, DUPLICANTS.TRAITS.METEORPHILE.NAME, false, false, true),
			new AttributeModifier("Caring", TRAITS.METEORPHILE_MODIFIER, DUPLICANTS.TRAITS.METEORPHILE.NAME, false, false, true),
			new AttributeModifier("Botanist", TRAITS.METEORPHILE_MODIFIER, DUPLICANTS.TRAITS.METEORPHILE.NAME, false, false, true),
			new AttributeModifier("Ranching", TRAITS.METEORPHILE_MODIFIER, DUPLICANTS.TRAITS.METEORPHILE.NAME, false, false, true)
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

	[MyCmpReq]
	private KPrefabID kPrefabID;

	private AttributeModifier[] attributeModifiers;

	public class StatesInstance : GameStateMachine<Meteorphile.States, Meteorphile.StatesInstance, Meteorphile, object>.GameInstance
	{
		public StatesInstance(Meteorphile master)
			: base(master)
		{
		}

		public bool IsMeteors()
		{
			if (GameplayEventManager.Instance == null || base.master.kPrefabID.PrefabTag == GameTags.MinionSelectPreview)
			{
				return false;
			}
			int myWorldId = this.GetMyWorldId();
			List<GameplayEventInstance> list = new List<GameplayEventInstance>();
			GameplayEventManager.Instance.GetActiveEventsOfType<MeteorShowerEvent>(myWorldId, ref list);
			for (int i = 0; i < list.Count; i++)
			{
				MeteorShowerEvent.StatesInstance statesInstance = list[i].smi as MeteorShowerEvent.StatesInstance;
				if (statesInstance != null && statesInstance.IsInsideState(statesInstance.sm.running.bombarding))
				{
					return true;
				}
			}
			return false;
		}
	}

	public class States : GameStateMachine<Meteorphile.States, Meteorphile.StatesInstance, Meteorphile>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.root.TagTransition(GameTags.Dead, null, false);
			this.idle.Transition(this.early, (Meteorphile.StatesInstance smi) => smi.IsMeteors(), UpdateRate.SIM_200ms);
			this.early.Enter("Meteors", delegate(Meteorphile.StatesInstance smi)
			{
				smi.master.ApplyModifiers();
			}).Exit("NotMeteors", delegate(Meteorphile.StatesInstance smi)
			{
				smi.master.RemoveModifiers();
			}).ToggleStatusItem(Db.Get().DuplicantStatusItems.Meteorphile, null)
				.ToggleExpression(Db.Get().Expressions.Happy, null)
				.Transition(this.idle, (Meteorphile.StatesInstance smi) => !smi.IsMeteors(), UpdateRate.SIM_200ms);
		}

		public GameStateMachine<Meteorphile.States, Meteorphile.StatesInstance, Meteorphile, object>.State idle;

		public GameStateMachine<Meteorphile.States, Meteorphile.StatesInstance, Meteorphile, object>.State early;
	}
}
