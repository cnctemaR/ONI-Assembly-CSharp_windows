using System;
using System.Collections.Generic;
using System.Linq;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class MeterScreen_Stress : MeterScreen_VTD_DuplicantIterator
{
	protected override void OnSpawn()
	{
		this.minionListCustomSortOperation = new Func<List<MinionIdentity>, List<MinionIdentity>>(this.SortByStressLevel);
		base.OnSpawn();
	}

	private List<MinionIdentity> SortByStressLevel(List<MinionIdentity> minions)
	{
		Amount stress_amount = Db.Get().Amounts.Stress;
		return minions.OrderByDescending<MinionIdentity, float>((MinionIdentity x) => stress_amount.Lookup(x).value).ToList<MinionIdentity>();
	}

	protected override string OnTooltip()
	{
		float maxStressInActiveWorld = GameUtil.GetMaxStressInActiveWorld();
		this.Tooltip.ClearMultiStringTooltip();
		this.Tooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_AVGSTRESS, Mathf.Round(maxStressInActiveWorld).ToString() + "%"), this.ToolTipStyle_Header);
		Amount stress = Db.Get().Amounts.Stress;
		List<MinionIdentity> worldMinionIdentities = this.GetWorldMinionIdentities();
		bool flag = this.lastSelectedDuplicantIndex >= 0 && this.lastSelectedDuplicantIndex < worldMinionIdentities.Count;
		for (int i = 0; i < worldMinionIdentities.Count; i++)
		{
			MinionIdentity minionIdentity = worldMinionIdentities[i];
			AmountInstance amountInstance = stress.Lookup(minionIdentity);
			base.AddToolTipAmountPercentLine(amountInstance, minionIdentity, flag && worldMinionIdentities[this.lastSelectedDuplicantIndex] == minionIdentity);
		}
		return "";
	}

	protected override void InternalRefresh()
	{
		float maxStressInActiveWorld = GameUtil.GetMaxStressInActiveWorld();
		this.Label.text = Mathf.Round(maxStressInActiveWorld).ToString();
		WorldTracker worldTracker = TrackerTool.Instance.GetWorldTracker<StressTracker>(ClusterManager.Instance.activeWorldId);
		this.diagnosticGraph.GetComponentInChildren<SparkLayer>().SetColor((worldTracker.GetCurrentValue() >= STRESS.ACTING_OUT_RESET) ? Constants.NEGATIVE_COLOR : Constants.NEUTRAL_COLOR);
		this.diagnosticGraph.GetComponentInChildren<LineLayer>().RefreshLine(worldTracker.ChartableData(600f), "stressData");
	}
}
