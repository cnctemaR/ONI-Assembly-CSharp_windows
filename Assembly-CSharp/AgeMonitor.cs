using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class AgeMonitor : GameStateMachine<AgeMonitor, AgeMonitor.Instance, IStateMachineTarget, AgeMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.alive;
		this.alive.ToggleAttributeModifier("Aging", (AgeMonitor.Instance smi) => this.aging, null).Transition(this.time_to_die, new StateMachine<AgeMonitor, AgeMonitor.Instance, IStateMachineTarget, AgeMonitor.Def>.Transition.ConditionCallback(AgeMonitor.TimeToDie), UpdateRate.SIM_1000ms).Update(new Action<AgeMonitor.Instance, float>(AgeMonitor.UpdateOldStatusItem), UpdateRate.SIM_1000ms, false);
		this.time_to_die.Enter(new StateMachine<AgeMonitor, AgeMonitor.Instance, IStateMachineTarget, AgeMonitor.Def>.State.Callback(AgeMonitor.Die));
		this.aging = new AttributeModifier(Db.Get().Amounts.Age.deltaAttribute.Id, 0.0016666667f, CREATURES.MODIFIERS.AGE.NAME, false, false, true);
	}

	private static void Die(AgeMonitor.Instance smi)
	{
		smi.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Generic);
	}

	private static bool TimeToDie(AgeMonitor.Instance smi)
	{
		return smi.age.value >= smi.age.GetMax();
	}

	private static void UpdateOldStatusItem(AgeMonitor.Instance smi, float dt)
	{
		KSelectable component = smi.GetComponent<KSelectable>();
		bool flag = smi.age.value > smi.age.GetMax() - 5f;
		smi.oldStatusGuid = component.ToggleStatusItem(Db.Get().CreatureStatusItems.Old, smi.oldStatusGuid, flag, smi);
	}

	private const float OLD_WARNING = 5f;

	public GameStateMachine<AgeMonitor, AgeMonitor.Instance, IStateMachineTarget, AgeMonitor.Def>.State alive;

	public GameStateMachine<AgeMonitor, AgeMonitor.Instance, IStateMachineTarget, AgeMonitor.Def>.State time_to_die;

	private AttributeModifier aging;

	public class Def : StateMachine.BaseDef
	{
		public override void Configure(GameObject prefab)
		{
			prefab.AddOrGet<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Age.Id);
		}

		public float maxAgePercentOnSpawn = 0.75f;
	}

	public new class Instance : GameStateMachine<AgeMonitor, AgeMonitor.Instance, IStateMachineTarget, AgeMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, AgeMonitor.Def def)
			: base(master, def)
		{
			this.age = Db.Get().Amounts.Age.Lookup(base.gameObject);
			base.Subscribe(1119167081, delegate(object data)
			{
				this.RandomizeAge();
			});
		}

		public void RandomizeAge()
		{
			this.age.value = global::UnityEngine.Random.value * this.age.GetMax() * base.def.maxAgePercentOnSpawn;
			AmountInstance amountInstance = Db.Get().Amounts.Fertility.Lookup(base.gameObject);
			if (amountInstance != null)
			{
				amountInstance.value = this.age.value / this.age.GetMax() * amountInstance.GetMax() * 1.75f;
				amountInstance.value = Mathf.Min(amountInstance.value, amountInstance.GetMax() * 0.9f);
			}
		}

		public float CyclesUntilDeath
		{
			get
			{
				return this.age.GetMax() - this.age.value;
			}
		}

		public AmountInstance age;

		public Guid oldStatusGuid;
	}
}
