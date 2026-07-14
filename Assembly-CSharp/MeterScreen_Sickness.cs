using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class MeterScreen_Sickness : MeterScreen_VTD_DuplicantIterator
{
	protected override void InternalRefresh()
	{
		List<MinionIdentity> worldMinionIdentities = this.GetWorldMinionIdentities();
		int num = this.CountSickDupes(worldMinionIdentities);
		this.Label.text = num.ToString();
	}

	protected override string OnTooltip()
	{
		List<MinionIdentity> worldMinionIdentities = this.GetWorldMinionIdentities();
		int num = this.CountSickDupes(worldMinionIdentities);
		this.Tooltip.ClearMultiStringTooltip();
		this.Tooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_SICK_DUPES, num.ToString()), this.ToolTipStyle_Header);
		for (int i = 0; i < worldMinionIdentities.Count; i++)
		{
			MinionIdentity minionIdentity = worldMinionIdentities[i];
			if (!minionIdentity.IsNullOrDestroyed())
			{
				string text = minionIdentity.GetComponent<KSelectable>().GetName();
				Sicknesses sicknesses = minionIdentity.GetComponent<MinionModifiers>().sicknesses;
				if (sicknesses.IsInfected())
				{
					text += " (";
					int num2 = 0;
					foreach (SicknessInstance sicknessInstance in sicknesses.ModifierList)
					{
						text = text + ((num2 > 0) ? ", " : "") + sicknessInstance.modifier.Name;
						num2++;
					}
					text += ")";
				}
				bool flag = i == this.lastSelectedDuplicantIndex;
				base.AddToolTipLine(text, flag);
			}
		}
		return "";
	}

	private int CountSickDupes(List<MinionIdentity> minions)
	{
		int num = 0;
		foreach (MinionIdentity minionIdentity in minions)
		{
			if (!minionIdentity.IsNullOrDestroyed() && minionIdentity.GetComponent<MinionModifiers>().sicknesses.IsInfected())
			{
				num++;
			}
		}
		return num;
	}
}
