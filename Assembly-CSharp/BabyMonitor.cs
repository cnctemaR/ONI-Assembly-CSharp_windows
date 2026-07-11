using System;
using Klei;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class BabyMonitor : GameStateMachine<BabyMonitor, BabyMonitor.Instance, IStateMachineTarget, BabyMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.baby;
		this.root.Enter(new StateMachine<BabyMonitor, BabyMonitor.Instance, IStateMachineTarget, BabyMonitor.Def>.State.Callback(BabyMonitor.AddBabyEffect));
		this.baby.Transition(this.spawnadult, new StateMachine<BabyMonitor, BabyMonitor.Instance, IStateMachineTarget, BabyMonitor.Def>.Transition.ConditionCallback(BabyMonitor.IsReadyToSpawnAdult), UpdateRate.SIM_4000ms);
		this.spawnadult.ToggleBehaviour(GameTags.Creatures.Behaviours.GrowUpBehaviour, (BabyMonitor.Instance smi) => true, null);
		this.babyEffect = new Effect("IsABaby", CREATURES.MODIFIERS.BABY.NAME, CREATURES.MODIFIERS.BABY.TOOLTIP, 0f, true, false, false, null, 0f, null);
		this.babyEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -0.9f, CREATURES.MODIFIERS.BABY.NAME, true, false, true));
		this.babyEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, 5f, CREATURES.MODIFIERS.BABY.NAME, false, false, true));
	}

	private static void AddBabyEffect(BabyMonitor.Instance smi)
	{
		smi.Get<Effects>().Add(smi.sm.babyEffect, false);
	}

	private static bool IsReadyToSpawnAdult(BabyMonitor.Instance smi)
	{
		AmountInstance amountInstance = Db.Get().Amounts.Age.Lookup(smi.gameObject);
		float num = 5f;
		if (GenericGameSettings.instance.acceleratedLifecycle)
		{
			num = 0.005f;
		}
		return amountInstance.value > num;
	}

	public GameStateMachine<BabyMonitor, BabyMonitor.Instance, IStateMachineTarget, BabyMonitor.Def>.State baby;

	public GameStateMachine<BabyMonitor, BabyMonitor.Instance, IStateMachineTarget, BabyMonitor.Def>.State spawnadult;

	public Effect babyEffect;

	public class Def : StateMachine.BaseDef
	{
		public Tag adultPrefab;
	}

	public new class Instance : GameStateMachine<BabyMonitor, BabyMonitor.Instance, IStateMachineTarget, BabyMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, BabyMonitor.Def def)
			: base(master, def)
		{
		}

		public void SpawnAdult()
		{
			Vector3 position = base.smi.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(base.smi.def.adultPrefab), position);
			gameObject.SetActive(true);
			gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim("growup_pst");
			foreach (AmountInstance amountInstance in base.gameObject.GetAmounts())
			{
				AmountInstance amountInstance2 = amountInstance.amount.Lookup(gameObject);
				if (amountInstance2 != null)
				{
					float num = amountInstance.value / amountInstance.GetMax();
					amountInstance2.value = num * amountInstance2.GetMax();
				}
			}
			gameObject.Trigger(-2027483228, base.gameObject);
			KSelectable component = base.gameObject.GetComponent<KSelectable>();
			if (SelectTool.Instance != null && SelectTool.Instance.selected != null && SelectTool.Instance.selected == component)
			{
				SelectTool.Instance.Select(gameObject.GetComponent<KSelectable>(), false);
			}
			base.smi.gameObject.DeleteObject();
		}
	}
}
