using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class CrewRationsEntry : CrewListEntry
{
	public override void Populate(MinionIdentity _identity)
	{
		base.Populate(_identity);
		this.rationMonitor = _identity.GetSMI<RationMonitor.Instance>();
		this.incRationPerDayButton.onClick += delegate
		{
			this.IncreaseRationsPerDay();
		};
		this.decRationPerDayButton.onClick += delegate
		{
			this.DecreaseRationsPerDay();
		};
		this.Refresh();
	}

	private void IncreaseRationsPerDay()
	{
		int dailyRations = this.rationMonitor.GetDailyRations();
		if (dailyRations < 15)
		{
			this.rationMonitor.SetDailyRations(dailyRations + 1);
		}
		else
		{
			this.rationMonitor.SetRationed(false);
		}
	}

	private void DecreaseRationsPerDay()
	{
		if (!this.rationMonitor.IsRationed())
		{
			this.rationMonitor.SetRationed(true);
		}
		else
		{
			int dailyRations = this.rationMonitor.GetDailyRations();
			this.rationMonitor.SetDailyRations(dailyRations - 1);
		}
	}

	public override void Refresh()
	{
		base.Refresh();
		this.rationPerDayText.text = ((!this.rationMonitor.IsRationed()) ? UI.VITALSSCREEN_UNTIL_FULL.text : GameUtil.GetFormattedCalories((float)this.rationMonitor.GetDailyRations() * 100000f, GameUtil.TimeSlice.None, true));
		this.rationsEatenToday.text = GameUtil.GetFormattedCalories(this.rationMonitor.GetRationsAteToday() * 100000f, GameUtil.TimeSlice.None, true);
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
			string name = amountInstance.amount.Name;
			if (name != null)
			{
				if (CrewRationsEntry.<>f__switch$map2 == null)
				{
					CrewRationsEntry.<>f__switch$map2 = new Dictionary<string, int>(2)
					{
						{ "Stress", 0 },
						{ "Calories", 1 }
					};
				}
				int num3;
				if (CrewRationsEntry.<>f__switch$map2.TryGetValue(name, out num3))
				{
					if (num3 != 0)
					{
						if (num3 == 1)
						{
							this.currentCaloriesText.text = text + "%";
							this.currentCaloriesText.GetComponent<ToolTip>().toolTip = amountInstance.GetTooltip();
						}
					}
					else
					{
						this.currentStressText.text = amountInstance.GetValueString();
						this.currentStressText.GetComponent<ToolTip>().toolTip = amountInstance.GetTooltip();
						this.stressTrendImage.SetValue(amountInstance, new Func<AmountInstance, string>(amountInstance.amount.GetTooltip));
					}
				}
			}
		}
	}

	public KButton incRationPerDayButton;

	public KButton decRationPerDayButton;

	public LocText rationPerDayText;

	public LocText rationsEatenToday;

	public LocText currentCaloriesText;

	public LocText currentStressText;

	public ValueTrendImageToggle stressTrendImage;

	private RationMonitor.Instance rationMonitor;
}
