using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class IrrigationMonitorInstance : FertilizationMonitor.Instance
{
	public IrrigationMonitorInstance(IStateMachineTarget master, IrrigationMonitorInstance.Def def)
		: base(master, def)
	{
	}

	public override StatusItem GetStarvedStatusItem()
	{
		return Db.Get().CreatureStatusItems.NeedsIrrigation;
	}

	public override StatusItem GetNotAcceptedStatusItem()
	{
		return Db.Get().CreatureStatusItems.CantAcceptIrrigation;
	}

	protected override void AddAmounts(GameObject gameObject)
	{
		Amounts amounts = gameObject.GetAmounts();
		this.fertilization = amounts.Add(new AmountInstance(Db.Get().Amounts.Irrigation, gameObject));
	}

	protected override void MakeModifiers()
	{
		this.consumptionRate = new AttributeModifier(Db.Get().Amounts.Irrigation.deltaAttribute.Id, -0.16666667f, CREATURES.STATS.IRRIGATION.CONSUME_MODIFIER, false, false);
		this.absorptionRate = new AttributeModifier(Db.Get().Amounts.Irrigation.deltaAttribute.Id, 1.6666666f, CREATURES.STATS.IRRIGATION.ABSORBING_MODIFIER, false, false);
		AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(base.gameObject);
		this.badConditionModifier = new AttributeModifier(Db.Get().Amounts.YieldBonus.deltaAttribute.Id, 0f / amountInstance.GetMax(), CREATURES.STATS.YIELDBONUS.MODIFIERS.NOT_IRRIGATED, false, false);
		this.goodConditionModifier = new AttributeModifier(Db.Get().Amounts.YieldBonus.deltaAttribute.Id, 0.00041666668f / amountInstance.GetMax(), CREATURES.STATS.YIELDBONUS.MODIFIERS.IRRIGATED, false, false);
	}

	public override bool AcceptsFertilizer()
	{
		PlantablePlot component = base.sm.fertilizerStorage.Get(this).GetComponent<PlantablePlot>();
		return component != null && component.AcceptsIrrigation;
	}

	public new class Def : FertilizationMonitor.Instance.Def
	{
	}
}
