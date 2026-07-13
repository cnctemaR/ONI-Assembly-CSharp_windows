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
		this.babyEffect = new Effect("IsABaby", CREATURES.MODIFIERS.BABY.NAME, CREATURES.MODIFIERS.BABY.TOOLTIP, 0f, true, false, false, null, -1f, 0f, null, "");
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
		float num = smi.def.adultThreshold;
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

		public string onGrowDropID;

		public float onGrowDropUnits = 1f;

		public bool forceAdultNavType;

		public float adultThreshold = 5f;

		public Action<GameObject> configureAdultOnMaturation;
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
			if (!base.smi.gameObject.HasTag(GameTags.Creatures.PreventGrowAnimation))
			{
				gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim("growup_pst");
			}
			if (base.smi.def.onGrowDropID != null)
			{
				GameObject gameObject2 = Util.KInstantiate(Assets.GetPrefab(base.smi.def.onGrowDropID), position);
				gameObject2.GetComponent<PrimaryElement>().Mass *= base.smi.def.onGrowDropUnits;
				gameObject2.SetActive(true);
			}
			foreach (AmountInstance amountInstance in base.gameObject.GetAmounts())
			{
				AmountInstance amountInstance2 = amountInstance.amount.Lookup(gameObject);
				if (amountInstance2 != null)
				{
					float num = amountInstance.value / amountInstance.GetMax();
					amountInstance2.value = num * amountInstance2.GetMax();
				}
			}
			EffectInstance effectInstance = base.gameObject.GetComponent<Effects>().Get("AteFromFeeder");
			if (effectInstance != null)
			{
				gameObject.GetComponent<Effects>().Add(effectInstance.effect, effectInstance.shouldSave).timeRemaining = effectInstance.timeRemaining;
			}
			Navigator component = gameObject.GetComponent<Navigator>();
			if (!base.smi.def.forceAdultNavType)
			{
				Navigator component2 = base.smi.GetComponent<Navigator>();
				component.SetCurrentNavType(component2.CurrentNavType);
			}
			int num2 = Grid.PosToCell(position);
			if (!component.NavGrid.NavTable.IsValid(num2, component.CurrentNavType))
			{
				foreach (CellOffset cellOffset in Pickupable.displacementOffsets)
				{
					int num3 = Grid.OffsetCell(num2, cellOffset);
					if (component.NavGrid.NavTable.IsValid(num3, component.CurrentNavType))
					{
						gameObject.transform.SetPosition(Grid.CellToPos(num3));
						break;
					}
				}
			}
			gameObject.Trigger(-2027483228, base.gameObject);
			KSelectable component3 = base.gameObject.GetComponent<KSelectable>();
			if (SelectTool.Instance != null && SelectTool.Instance.selected != null && SelectTool.Instance.selected == component3)
			{
				SelectTool.Instance.Select(gameObject.GetComponent<KSelectable>(), false);
			}
			base.smi.gameObject.Trigger(663420073, gameObject);
			base.smi.gameObject.DeleteObject();
			if (base.def.configureAdultOnMaturation != null)
			{
				base.def.configureAdultOnMaturation(gameObject);
			}
		}
	}
}
