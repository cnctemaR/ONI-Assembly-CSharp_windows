using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class MeterScreen : KScreen
{
	public static MeterScreen Instance { get; private set; }

	public bool StartValuesSet
	{
		get
		{
			return this.startValuesSet;
		}
	}

	protected override void OnPrefabInit()
	{
		MeterScreen.Instance = this;
	}

	protected override void OnSpawn()
	{
		this.StressTooltip.OnToolTip = new Func<string>(this.OnStressTooltip);
		this.ToxicityTooltip.OnToolTip = new Func<string>(this.OnToxicityTooltip);
		this.RationsTooltip.OnToolTip = new Func<string>(this.OnRationsTooltip);
		this.RedAlertButton.onClick += delegate
		{
			this.OnRedAlertClick();
		};
	}

	private void OnRedAlertClick()
	{
		bool flag = !RedAlertManager.Instance.Get().IsOn();
		RedAlertManager.Instance.Get().Toggle(flag);
		if (flag)
		{
			this.RedAlertButton.ActivateFlourish(true, ImageToggleState.State.Active);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open", false));
			if (this.loopInstance == null)
			{
				this.loopInstance = LoopingSoundManager.StartSound(GlobalAssets.GetSound("RedAlert_LP", false), Vector3.zero);
			}
		}
		else
		{
			this.RedAlertButton.ActivateFlourish(false, ImageToggleState.State.Inactive);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
			if (this.loopInstance != null)
			{
				LoopingSoundManager.StopSound(GlobalAssets.GetSound("RedAlert_LP", false), this.loopInstance);
				this.loopInstance = null;
			}
		}
	}

	private void SimUpdate(float dt)
	{
		this.tickCount++;
		if (this.tickCount % 4 == 0)
		{
			this.Refresh();
		}
	}

	public void InitializeValues()
	{
		if (this.startValuesSet)
		{
			return;
		}
		this.startValuesSet = true;
		this.Refresh();
	}

	private void Refresh()
	{
		this.RefreshMinions();
		this.RefreshRations();
		this.RefreshStress();
		this.RefreshToxicity();
	}

	private void RefreshMinions()
	{
		int count = Components.LiveMinionIdentities.Count;
		this.currentMinions.text = count.ToString("0");
		this.MinionsTooltip.ClearMultiStringTooltip();
		this.MinionsTooltip.AddMultiStringTooltip("Population: " + count.ToString("0"), this.ToolTipStyle_Header);
	}

	private void RefreshToxicity()
	{
		float averageToxicity = GameUtil.GetAverageToxicity();
		this.ToxicityText.text = Mathf.Round(averageToxicity).ToString() + "%";
	}

	private void RefreshRations()
	{
		if (this.RationsText != null && RationTracker.Get() != null)
		{
			int num = (int)(RationTracker.Get().CountRations(null, true) * 100000f);
			this.RationsText.text = GameUtil.GetFormattedCalories((float)num, GameUtil.TimeSlice.None, true);
		}
	}

	private string OnStressTooltip()
	{
		float maxStress = GameUtil.GetMaxStress();
		this.StressTooltip.ClearMultiStringTooltip();
		this.StressTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_AVGSTRESS, Mathf.Round(maxStress).ToString() + "%"), this.ToolTipStyle_Header);
		Amount stress_amount = Db.Get().Amounts.Stress;
		foreach (MinionIdentity minionIdentity in new List<MinionIdentity>(Components.LiveMinionIdentities).OrderBy<MinionIdentity, float>((MinionIdentity x) => -stress_amount.Lookup(x).value))
		{
			AmountInstance amountInstance = stress_amount.Lookup(minionIdentity);
			this.StressTooltip.AddMultiStringTooltip(minionIdentity.GetComponent<KSelectable>().GetName() + ":  " + Mathf.Round(amountInstance.value).ToString() + "%", this.ToolTipStyle_Property);
		}
		return string.Empty;
	}

	private string OnToxicityTooltip()
	{
		float averageToxicity = GameUtil.GetAverageToxicity();
		this.ToxicityTooltip.ClearMultiStringTooltip();
		this.ToxicityTooltip.AddMultiStringTooltip("Average Toxicity " + Mathf.Round(averageToxicity).ToString() + "%", this.ToolTipStyle_Header);
		Amount toxicity_amount = Db.Get().Amounts.Toxicity;
		foreach (MinionIdentity minionIdentity in new List<MinionIdentity>(Components.LiveMinionIdentities).OrderBy<MinionIdentity, float>((MinionIdentity x) => -toxicity_amount.Lookup(x).value))
		{
			AmountInstance amountInstance = toxicity_amount.Lookup(minionIdentity);
			this.ToxicityTooltip.AddMultiStringTooltip(minionIdentity.GetComponent<KSelectable>().GetName() + ":  " + Mathf.Round(amountInstance.value).ToString() + "%", this.ToolTipStyle_Property);
		}
		return string.Empty;
	}

	private string OnRationsTooltip()
	{
		this.rationsDict.Clear();
		int num = (int)RationTracker.Get().CountRations(this.rationsDict, true);
		int num2 = (int)((float)num * 100000f);
		this.RationsText.text = GameUtil.GetFormattedCalories((float)num2, GameUtil.TimeSlice.None, true);
		this.RationsTooltip.ClearMultiStringTooltip();
		this.RationsTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_MEALHISTORY, GameUtil.GetFormattedCalories((float)num2, GameUtil.TimeSlice.None, true)), this.ToolTipStyle_Header);
		this.RationsTooltip.AddMultiStringTooltip(string.Empty, this.ToolTipStyle_Property);
		foreach (KeyValuePair<string, float> keyValuePair in this.rationsDict)
		{
			EdiblesManager.FoodInfo foodInfo = EdiblesManager.instance.GetFoodInfo(keyValuePair.Key);
			this.RationsTooltip.AddMultiStringTooltip(string.Format("{0}: {1}", foodInfo.Name, keyValuePair.Value), this.ToolTipStyle_Property);
		}
		return string.Empty;
	}

	private void RefreshStress()
	{
		float maxStress = GameUtil.GetMaxStress();
		this.StressText.text = Mathf.Round(maxStress).ToString();
	}

	[SerializeField]
	private LocText currentMinions;

	private int tickCount;

	public ToolTip MinionsTooltip;

	public LocText StressText;

	public ToolTip StressTooltip;

	public LocText RationsText;

	public ToolTip RationsTooltip;

	public LocText ToxicityText;

	public ToolTip ToxicityTooltip;

	public TextStyleSetting ToolTipStyle_Header;

	public TextStyleSetting ToolTipStyle_Property;

	private bool startValuesSet;

	[SerializeField]
	private KToggle RedAlertButton;

	private EventInstance loopInstance;

	private Dictionary<string, float> rationsDict = new Dictionary<string, float>();
}
