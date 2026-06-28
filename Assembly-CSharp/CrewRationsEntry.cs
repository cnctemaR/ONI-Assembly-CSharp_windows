using System;
using Klei.AI;
using UnityEngine;

public class CrewRationsEntry : CrewListEntry
{
	public override void Populate(MinionIdentity _identity)
	{
		base.Populate(_identity);
		this.rationMonitor = _identity.GetSMI<RationMonitor.Instance>();
		this.Refresh();
	}

	public override void Refresh()
	{
		base.Refresh();
		this.rationsEatenToday.text = GameUtil.GetFormattedCalories(this.rationMonitor.GetRationsAteToday(), GameUtil.TimeSlice.None, true);
		if (this.identity == null)
		{
			return;
		}
		Amounts amounts = this.identity.GetAmounts();
		foreach (AmountInstance amountInstance in amounts)
		{
			float min = amountInstance.GetMin();
			float max = amountInstance.GetMax();
			float num = max - min;
			float num2 = (num - (max - amountInstance.value)) / num;
			string text = Mathf.RoundToInt(num2 * 100f).ToString();
			if (amountInstance.amount == Db.Get().Amounts.Stress)
			{
				this.currentStressText.text = amountInstance.GetValueString();
				this.currentStressText.GetComponent<ToolTip>().toolTip = amountInstance.GetTooltip();
				this.stressTrendImage.SetValue(amountInstance, new Func<AmountInstance, string>(amountInstance.amount.GetTooltip));
			}
			else if (amountInstance.amount == Db.Get().Amounts.Calories)
			{
				this.currentCaloriesText.text = text + "%";
				this.currentCaloriesText.GetComponent<ToolTip>().toolTip = amountInstance.GetTooltip();
			}
			else if (amountInstance.amount == Db.Get().Amounts.HitPoints)
			{
				this.currentHealthText.text = text + "%";
				this.currentHealthText.GetComponent<ToolTip>().toolTip = amountInstance.GetTooltip();
			}
		}
	}

	public KButton incRationPerDayButton;

	public KButton decRationPerDayButton;

	public LocText rationPerDayText;

	public LocText rationsEatenToday;

	public LocText currentCaloriesText;

	public LocText currentStressText;

	public LocText currentHealthText;

	public ValueTrendImageToggle stressTrendImage;

	private RationMonitor.Instance rationMonitor;
}
