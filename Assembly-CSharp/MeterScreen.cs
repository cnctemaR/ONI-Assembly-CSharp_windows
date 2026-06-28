using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;

public class MeterScreen : KScreen, IRender200ms
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
		this.ImmunityTooltip.OnToolTip = new Func<string>(this.OnImmunityTooltip);
		this.RationsTooltip.OnToolTip = new Func<string>(this.OnRationsTooltip);
		this.RedAlertTooltip.OnToolTip = new Func<string>(this.OnRedAlertTooltip);
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
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open", false));
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RedAlert_ON", false));
			if (this.loopInstance == null)
			{
				this.loopInstance = LoopingSoundManager.StartSound(GlobalAssets.GetSound("RedAlert_LP", false), Vector3.zero, true);
			}
		}
		else
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RedAlert_OFF", false));
			if (this.loopInstance != null)
			{
				LoopingSoundManager.StopSound(GlobalAssets.GetSound("RedAlert_LP", false), this.loopInstance);
				this.loopInstance = null;
			}
		}
	}

	public void Render200ms(float dt)
	{
		this.Refresh();
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
		this.RefreshImmunity();
	}

	private void RefreshMinions()
	{
		int count = Components.LiveMinionIdentities.Count;
		this.currentMinions.text = count.ToString("0");
		this.MinionsTooltip.ClearMultiStringTooltip();
		this.MinionsTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_POPULATION, count.ToString("0")), this.ToolTipStyle_Header);
	}

	private void RefreshImmunity()
	{
		float worstImmunity = this.GetWorstImmunity();
		this.ImmunityText.text = Mathf.Round(worstImmunity).ToString();
	}

	private float GetWorstImmunity()
	{
		if (Components.LiveMinionIdentities.Count <= 0)
		{
			return 100f;
		}
		Components.Cmps<MinionIdentity> liveMinionIdentities = Components.LiveMinionIdentities;
		float num = Db.Get().Amounts.ImmuneLevel.Lookup(liveMinionIdentities[0]).value;
		for (int i = 1; i < liveMinionIdentities.Count; i++)
		{
			num = Mathf.Min(Db.Get().Amounts.ImmuneLevel.Lookup(liveMinionIdentities[i]).value, num);
		}
		return num;
	}

	private void RefreshRations()
	{
		if (this.RationsText != null && RationTracker.Get() != null)
		{
			long num = (long)RationTracker.Get().CountRations(null, true);
			this.RationsText.text = GameUtil.GetFormattedCalories((float)num, GameUtil.TimeSlice.None, true);
		}
	}

	private IList<MinionIdentity> GetStressedMinions()
	{
		Amount stress_amount = Db.Get().Amounts.Stress;
		List<MinionIdentity> list = new List<MinionIdentity>(Components.LiveMinionIdentities);
		return new List<MinionIdentity>(list.OrderByDescending<MinionIdentity, float>((MinionIdentity x) => stress_amount.Lookup(x).value));
	}

	private string OnStressTooltip()
	{
		float maxStress = GameUtil.GetMaxStress();
		this.StressTooltip.ClearMultiStringTooltip();
		this.StressTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_AVGSTRESS, Mathf.Round(maxStress).ToString() + "%"), this.ToolTipStyle_Header);
		Amount stress = Db.Get().Amounts.Stress;
		IList<MinionIdentity> stressedMinions = this.GetStressedMinions();
		for (int i = 0; i < stressedMinions.Count; i++)
		{
			MinionIdentity minionIdentity = stressedMinions[i];
			AmountInstance amountInstance = stress.Lookup(minionIdentity);
			this.AddToolTipAmountLine(this.StressTooltip, amountInstance, minionIdentity, i == this.stressDisplayInfo.selectedIndex);
		}
		return string.Empty;
	}

	private IList<MinionIdentity> GetImmunityLevels()
	{
		Amount amounts = Db.Get().Amounts.ImmuneLevel;
		List<MinionIdentity> list = new List<MinionIdentity>(Components.LiveMinionIdentities);
		return new List<MinionIdentity>(list.OrderBy<MinionIdentity, float>((MinionIdentity x) => amounts.Lookup(x).value));
	}

	private string OnImmunityTooltip()
	{
		float worstImmunity = this.GetWorstImmunity();
		this.ImmunityTooltip.ClearMultiStringTooltip();
		this.ImmunityTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_IMMUNITY_LEVELS, Mathf.Round(worstImmunity).ToString() + "%"), this.ToolTipStyle_Header);
		Amount immuneLevel = Db.Get().Amounts.ImmuneLevel;
		IList<MinionIdentity> immunityLevels = this.GetImmunityLevels();
		for (int i = 0; i < immunityLevels.Count; i++)
		{
			MinionIdentity minionIdentity = immunityLevels[i];
			AmountInstance amountInstance = immuneLevel.Lookup(minionIdentity);
			this.AddToolTipAmountLine(this.ImmunityTooltip, amountInstance, minionIdentity, i == this.immunityDisplayInfo.selectedIndex);
		}
		return string.Empty;
	}

	private void AddToolTipAmountLine(ToolTip tooltip, AmountInstance amount, MinionIdentity id, bool selected)
	{
		string name = id.GetComponent<KSelectable>().GetName();
		string text = name + ":  " + Mathf.Round(amount.value).ToString() + "%";
		if (selected)
		{
			tooltip.AddMultiStringTooltip("<color=#F0B310FF>" + text + "</color>", this.ToolTipStyle_Property);
		}
		else
		{
			tooltip.AddMultiStringTooltip(text, this.ToolTipStyle_Property);
		}
	}

	private string OnRationsTooltip()
	{
		this.rationsDict.Clear();
		float num = RationTracker.Get().CountRations(this.rationsDict, true);
		this.RationsText.text = GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true);
		this.RationsTooltip.ClearMultiStringTooltip();
		this.RationsTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_MEALHISTORY, GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true)), this.ToolTipStyle_Header);
		this.RationsTooltip.AddMultiStringTooltip(string.Empty, this.ToolTipStyle_Property);
		foreach (KeyValuePair<string, float> keyValuePair in this.rationsDict)
		{
			EdiblesManager.FoodInfo foodInfo = EdiblesManager.instance.GetFoodInfo(keyValuePair.Key);
			this.RationsTooltip.AddMultiStringTooltip(string.Format("{0}: {1}", foodInfo.Name, keyValuePair.Value), this.ToolTipStyle_Property);
		}
		return string.Empty;
	}

	private string OnRedAlertTooltip()
	{
		this.RedAlertTooltip.ClearMultiStringTooltip();
		this.RedAlertTooltip.AddMultiStringTooltip(UI.TOOLTIPS.RED_ALERT_TITLE, this.ToolTipStyle_Header);
		this.RedAlertTooltip.AddMultiStringTooltip(UI.TOOLTIPS.RED_ALERT_CONTENT, this.ToolTipStyle_Property);
		return string.Empty;
	}

	private void RefreshStress()
	{
		float maxStress = GameUtil.GetMaxStress();
		this.StressText.text = Mathf.Round(maxStress).ToString();
	}

	public void OnClickStress(BaseEventData base_ev_data)
	{
		IList<MinionIdentity> stressedMinions = this.GetStressedMinions();
		this.UpdateDisplayInfo(base_ev_data, ref this.stressDisplayInfo, stressedMinions);
		this.OnStressTooltip();
		this.StressTooltip.forceRefresh = true;
	}

	public void OnClickImmunity(BaseEventData base_ev_data)
	{
		IList<MinionIdentity> immunityLevels = this.GetImmunityLevels();
		this.UpdateDisplayInfo(base_ev_data, ref this.immunityDisplayInfo, immunityLevels);
		this.OnImmunityTooltip();
		this.ImmunityTooltip.forceRefresh = true;
	}

	private void UpdateDisplayInfo(BaseEventData base_ev_data, ref MeterScreen.DisplayInfo display_info, IList<MinionIdentity> minions)
	{
		PointerEventData pointerEventData = base_ev_data as PointerEventData;
		if (pointerEventData == null)
		{
			return;
		}
		PointerEventData.InputButton button = pointerEventData.button;
		if (button != PointerEventData.InputButton.Left)
		{
			if (button == PointerEventData.InputButton.Right)
			{
				display_info.selectedIndex = -1;
			}
		}
		else
		{
			if (Components.LiveMinionIdentities.Count < display_info.selectedIndex)
			{
				display_info.selectedIndex = -1;
			}
			if (Components.LiveMinionIdentities.Count > 0)
			{
				display_info.selectedIndex = (display_info.selectedIndex + 1) % Components.LiveMinionIdentities.Count;
				MinionIdentity minionIdentity = minions[display_info.selectedIndex];
				SelectTool.Instance.SelectAndFocus(minionIdentity.transform.GetPosition(), minionIdentity.GetComponent<KSelectable>(), new Vector3(5f, 0f, 0f));
			}
		}
	}

	[SerializeField]
	private LocText currentMinions;

	public ToolTip MinionsTooltip;

	public LocText StressText;

	public ToolTip StressTooltip;

	public LocText RationsText;

	public ToolTip RationsTooltip;

	public LocText ImmunityText;

	public ToolTip ImmunityTooltip;

	public TextStyleSetting ToolTipStyle_Header;

	public TextStyleSetting ToolTipStyle_Property;

	private bool startValuesSet;

	[SerializeField]
	private KToggle RedAlertButton;

	public ToolTip RedAlertTooltip;

	private EventInstance loopInstance;

	private MeterScreen.DisplayInfo stressDisplayInfo = new MeterScreen.DisplayInfo
	{
		selectedIndex = -1
	};

	private MeterScreen.DisplayInfo immunityDisplayInfo = new MeterScreen.DisplayInfo
	{
		selectedIndex = -1
	};

	private Dictionary<string, float> rationsDict = new Dictionary<string, float>();

	private struct DisplayInfo
	{
		public int selectedIndex;
	}
}
