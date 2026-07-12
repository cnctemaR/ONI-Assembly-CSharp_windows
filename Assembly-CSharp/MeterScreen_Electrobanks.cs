using System;
using System.Collections.Generic;
using System.Linq;
using STRINGS;
using UnityEngine;

public class MeterScreen_Electrobanks : MeterScreen_ValueTrackerDisplayer
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.LiveMinionIdentities.OnAdd += this.OnNewMinionAdded;
		List<MinionIdentity> allMinionsFromAllWorlds = this.GetAllMinionsFromAllWorlds();
		bool flag;
		if (allMinionsFromAllWorlds != null)
		{
			flag = allMinionsFromAllWorlds.Find((MinionIdentity m) => m.model == BionicMinionConfig.MODEL) != null;
		}
		else
		{
			flag = false;
		}
		this.SetVisibility(flag);
		BionicBatteryMonitor.WattageModifier difficultyModifier = BionicBatteryMonitor.GetDifficultyModifier();
		this.bionicJoulesPerCycle = (difficultyModifier.value + 200f) * 600f;
	}

	protected override void OnCleanUp()
	{
		Components.LiveMinionIdentities.OnAdd -= this.OnNewMinionAdded;
		base.OnCleanUp();
	}

	private void OnNewMinionAdded(MinionIdentity id)
	{
		if (id.model == BionicMinionConfig.MODEL)
		{
			this.SetVisibility(true);
		}
	}

	public void SetVisibility(bool isVisible)
	{
		base.gameObject.SetActive(isVisible);
	}

	protected override string OnTooltip()
	{
		this.per_electrobankType_UnitCount_Dictionary.Clear();
		float num = 0f;
		string formattedJoules = GameUtil.GetFormattedJoules(WorldResourceAmountTracker<ElectrobankTracker>.Get().CountAmount(this.per_electrobankType_UnitCount_Dictionary, out num, ClusterManager.Instance.activeWorld.worldInventory, true), "F1", GameUtil.TimeSlice.None);
		this.Label.text = formattedJoules;
		this.Tooltip.ClearMultiStringTooltip();
		this.Tooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_ELECTROBANK_JOULES, formattedJoules, GameUtil.GetFormattedJoules(this.bionicJoulesPerCycle, "F1", GameUtil.TimeSlice.None), GameUtil.GetFormattedUnits((float)((int)num), GameUtil.TimeSlice.None, true, "")), this.ToolTipStyle_Header);
		this.Tooltip.AddMultiStringTooltip("", this.ToolTipStyle_Property);
		foreach (KeyValuePair<string, float> keyValuePair in this.per_electrobankType_UnitCount_Dictionary.OrderByDescending<KeyValuePair<string, float>, float>((KeyValuePair<string, float> x) => x.Value).ToDictionary<KeyValuePair<string, float>, string, float>((KeyValuePair<string, float> t) => t.Key, (KeyValuePair<string, float> t) => t.Value))
		{
			GameObject prefab = Assets.GetPrefab(keyValuePair.Key);
			this.Tooltip.AddMultiStringTooltip((prefab != null) ? string.Format("{0} ({1}): {2}", prefab.GetProperName(), GameUtil.GetFormattedUnits((float)((int)keyValuePair.Value), GameUtil.TimeSlice.None, true, ""), GameUtil.GetFormattedJoules(keyValuePair.Value * 120000f, "F1", GameUtil.TimeSlice.None)) : string.Format(UI.TOOLTIPS.METERSCREEN_INVALID_ELECTROBANK_TYPE, keyValuePair.Key), this.ToolTipStyle_Property);
		}
		return "";
	}

	protected override void InternalRefresh()
	{
		if (!SaveLoader.Instance.IsDLCActiveForCurrentSave("DLC3_ID"))
		{
			return;
		}
		if (this.Label != null && WorldResourceAmountTracker<ElectrobankTracker>.Get() != null)
		{
			float num2;
			long num = (long)WorldResourceAmountTracker<ElectrobankTracker>.Get().CountAmount(null, out num2, ClusterManager.Instance.activeWorld.worldInventory, true);
			if (this.cachedJoules != num)
			{
				this.Label.text = GameUtil.GetFormattedJoules((float)num, "F1", GameUtil.TimeSlice.None);
				this.cachedJoules = num;
			}
		}
		this.diagnosticGraph.GetComponentInChildren<SparkLayer>().SetColor(((float)this.cachedJoules > (float)this.GetWorldMinionIdentities().Count * 120000f) ? Constants.NEUTRAL_COLOR : Constants.NEGATIVE_COLOR);
		WorldTracker worldTracker = TrackerTool.Instance.GetWorldTracker<ElectrobankJoulesTracker>(ClusterManager.Instance.activeWorldId);
		if (worldTracker != null)
		{
			this.diagnosticGraph.GetComponentInChildren<LineLayer>().RefreshLine(worldTracker.ChartableData(600f), "joules");
		}
	}

	private long cachedJoules = -1L;

	private Dictionary<string, float> per_electrobankType_UnitCount_Dictionary = new Dictionary<string, float>();

	private float bionicJoulesPerCycle;
}
