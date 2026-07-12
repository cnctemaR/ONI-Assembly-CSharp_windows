using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class LeadSuitMonitor : GameStateMachine<LeadSuitMonitor, LeadSuitMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.wearingSuit;
		base.Target(this.owner);
		this.wearingSuit.DefaultState(this.wearingSuit.hasBattery);
		this.wearingSuit.hasBattery.Update(new Action<LeadSuitMonitor.Instance, float>(LeadSuitMonitor.CoolSuit), UpdateRate.SIM_200ms, false).TagTransition(GameTags.SuitBatteryOut, this.wearingSuit.noBattery, false);
		this.wearingSuit.noBattery.Enter(delegate(LeadSuitMonitor.Instance smi)
		{
			Attributes attributes = smi.sm.owner.Get(smi).GetAttributes();
			if (attributes != null)
			{
				foreach (AttributeModifier attributeModifier in smi.noBatteryModifiers)
				{
					attributes.Add(attributeModifier);
				}
			}
		}).Exit(delegate(LeadSuitMonitor.Instance smi)
		{
			Attributes attributes2 = smi.sm.owner.Get(smi).GetAttributes();
			if (attributes2 != null)
			{
				foreach (AttributeModifier attributeModifier2 in smi.noBatteryModifiers)
				{
					attributes2.Remove(attributeModifier2);
				}
			}
		}).TagTransition(GameTags.SuitBatteryOut, this.wearingSuit.hasBattery, true);
	}

	public static void CoolSuit(LeadSuitMonitor.Instance smi, float dt)
	{
		if (!smi.navigator)
		{
			return;
		}
		GameObject gameObject = smi.sm.owner.Get(smi);
		if (!gameObject)
		{
			return;
		}
		if (gameObject.GetSMI<ExternalTemperatureMonitor.Instance>().AverageExternalTemperature >= smi.lead_suit_tank.coolingOperationalTemperature)
		{
			smi.lead_suit_tank.batteryCharge -= 1f / smi.lead_suit_tank.batteryDuration * dt;
			if (smi.lead_suit_tank.IsEmpty())
			{
				gameObject.AddTag(GameTags.SuitBatteryOut);
			}
		}
	}

	public LeadSuitMonitor.WearingSuit wearingSuit;

	public StateMachine<LeadSuitMonitor, LeadSuitMonitor.Instance, IStateMachineTarget, object>.TargetParameter owner;

	public class WearingSuit : GameStateMachine<LeadSuitMonitor, LeadSuitMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<LeadSuitMonitor, LeadSuitMonitor.Instance, IStateMachineTarget, object>.State hasBattery;

		public GameStateMachine<LeadSuitMonitor, LeadSuitMonitor.Instance, IStateMachineTarget, object>.State noBattery;
	}

	public new class Instance : GameStateMachine<LeadSuitMonitor, LeadSuitMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, GameObject owner)
			: base(master)
		{
			base.sm.owner.Set(owner, base.smi);
			this.navigator = owner.GetComponent<Navigator>();
			this.lead_suit_tank = master.GetComponent<LeadSuitTank>();
			this.noBatteryModifiers.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, (float)(-(float)global::TUNING.EQUIPMENT.SUITS.LEADSUIT_INSULATION), global::STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.SUIT_OUT_OF_BATTERIES, false, false, true));
			this.noBatteryModifiers.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.THERMAL_CONDUCTIVITY_BARRIER, -global::TUNING.EQUIPMENT.SUITS.LEADSUIT_THERMAL_CONDUCTIVITY_BARRIER, global::STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.SUIT_OUT_OF_BATTERIES, false, false, true));
		}

		public Navigator navigator;

		public LeadSuitTank lead_suit_tank;

		public List<AttributeModifier> noBatteryModifiers = new List<AttributeModifier>();
	}
}
