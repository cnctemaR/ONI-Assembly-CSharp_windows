using System;
using System.Collections.Generic;
using System.Linq;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;

public class MeterScreen : KScreen, IRender1000ms
{
	public static MeterScreen Instance { get; private set; }

	public static void DestroyInstance()
	{
		MeterScreen.Instance = null;
	}

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
		this.SickTooltip.OnToolTip = new Func<string>(this.OnSickTooltip);
		this.RationsTooltip.OnToolTip = new Func<string>(this.OnRationsTooltip);
		this.RedAlertTooltip.OnToolTip = new Func<string>(this.OnRedAlertTooltip);
		this.RedAlertButton.onClick += delegate
		{
			this.OnRedAlertClick();
		};
	}

	private void OnRedAlertClick()
	{
		bool flag = !VignetteManager.Instance.Get().IsRedAlertToggledOn();
		VignetteManager.Instance.Get().ToggleRedAlert(flag);
		if (flag)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open", false));
			return;
		}
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
	}

	public void Render1000ms(float dt)
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
		this.RefreshSick();
	}

	private void RefreshMinions()
	{
		int count = Components.LiveMinionIdentities.Count;
		if (count == this.cachedMinionCount)
		{
			return;
		}
		this.cachedMinionCount = count;
		this.currentMinions.text = count.ToString("0");
		this.MinionsTooltip.ClearMultiStringTooltip();
		this.MinionsTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_POPULATION, count.ToString("0")), this.ToolTipStyle_Header);
	}

	private void RefreshSick()
	{
		int num = MeterScreen.CountSickDupes();
		this.SickText.text = num.ToString();
	}

	private void RefreshRations()
	{
		if (this.RationsText != null && RationTracker.Get() != null)
		{
			long num = (long)RationTracker.Get().CountRations(null, true);
			if (this.cachedCalories != num)
			{
				this.RationsText.text = GameUtil.GetFormattedCalories((float)num, GameUtil.TimeSlice.None, true);
				this.cachedCalories = num;
			}
		}
	}

	private IList<MinionIdentity> GetStressedMinions()
	{
		Amount stress_amount = Db.Get().Amounts.Stress;
		return new List<MinionIdentity>(new List<MinionIdentity>(Components.LiveMinionIdentities.Items).OrderByDescending<MinionIdentity, float>((MinionIdentity x) => stress_amount.Lookup(x).value));
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
			this.AddToolTipAmountPercentLine(this.StressTooltip, amountInstance, minionIdentity, i == this.stressDisplayInfo.selectedIndex);
		}
		return "";
	}

	private string OnSickTooltip()
	{
		int num = MeterScreen.CountSickDupes();
		this.SickTooltip.ClearMultiStringTooltip();
		this.SickTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_SICK_DUPES, num.ToString()), this.ToolTipStyle_Header);
		for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
		{
			MinionIdentity minionIdentity = Components.LiveMinionIdentities[i];
			string text = minionIdentity.GetComponent<KSelectable>().GetName();
			Sicknesses sicknesses = minionIdentity.GetComponent<MinionModifiers>().sicknesses;
			if (sicknesses.IsInfected())
			{
				text += " (";
				int num2 = 0;
				foreach (SicknessInstance sicknessInstance in sicknesses)
				{
					text = text + ((num2 > 0) ? ", " : "") + sicknessInstance.modifier.Name;
					num2++;
				}
				text += ")";
			}
			bool flag = i == this.immunityDisplayInfo.selectedIndex;
			this.AddToolTipLine(this.SickTooltip, text, flag);
		}
		return "";
	}

	private static int CountSickDupes()
	{
		int num = 0;
		using (List<MinionIdentity>.Enumerator enumerator = Components.LiveMinionIdentities.Items.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.GetComponent<MinionModifiers>().sicknesses.IsInfected())
				{
					num++;
				}
			}
		}
		return num;
	}

	private void AddToolTipLine(ToolTip tooltip, string str, bool selected)
	{
		if (selected)
		{
			tooltip.AddMultiStringTooltip("<color=#F0B310FF>" + str + "</color>", this.ToolTipStyle_Property);
			return;
		}
		tooltip.AddMultiStringTooltip(str, this.ToolTipStyle_Property);
	}

	private void AddToolTipAmountPercentLine(ToolTip tooltip, AmountInstance amount, MinionIdentity id, bool selected)
	{
		string text = id.GetComponent<KSelectable>().GetName() + ":  " + Mathf.Round(amount.value).ToString() + "%";
		this.AddToolTipLine(tooltip, text, selected);
	}

	private string OnRationsTooltip()
	{
		this.rationsDict.Clear();
		float num = RationTracker.Get().CountRations(this.rationsDict, true);
		this.RationsText.text = GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true);
		this.RationsTooltip.ClearMultiStringTooltip();
		this.RationsTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_MEALHISTORY, GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true)), this.ToolTipStyle_Header);
		this.RationsTooltip.AddMultiStringTooltip("", this.ToolTipStyle_Property);
		foreach (KeyValuePair<string, float> keyValuePair in this.rationsDict.OrderByDescending<KeyValuePair<string, float>, float>(delegate(KeyValuePair<string, float> x)
		{
			EdiblesManager.FoodInfo foodInfo2 = EdiblesManager.GetFoodInfo(x.Key);
			return x.Value * ((foodInfo2 != null) ? foodInfo2.CaloriesPerUnit : (-1f));
		}).ToDictionary<KeyValuePair<string, float>, string, float>((KeyValuePair<string, float> t) => t.Key, (KeyValuePair<string, float> t) => t.Value))
		{
			EdiblesManager.FoodInfo foodInfo = EdiblesManager.GetFoodInfo(keyValuePair.Key);
			this.RationsTooltip.AddMultiStringTooltip((foodInfo != null) ? string.Format("{0}: {1}", foodInfo.Name, GameUtil.GetFormattedCalories(keyValuePair.Value * foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true)) : string.Format(UI.TOOLTIPS.METERSCREEN_INVALID_FOOD_TYPE, keyValuePair.Key), this.ToolTipStyle_Property);
		}
		return "";
	}

	private string OnRedAlertTooltip()
	{
		this.RedAlertTooltip.ClearMultiStringTooltip();
		this.RedAlertTooltip.AddMultiStringTooltip(UI.TOOLTIPS.RED_ALERT_TITLE, this.ToolTipStyle_Header);
		this.RedAlertTooltip.AddMultiStringTooltip(UI.TOOLTIPS.RED_ALERT_CONTENT, this.ToolTipStyle_Property);
		return "";
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

	private IList<MinionIdentity> GetSickMinions()
	{
		return Components.LiveMinionIdentities.Items;
	}

	public void OnClickImmunity(BaseEventData base_ev_data)
	{
		IList<MinionIdentity> sickMinions = this.GetSickMinions();
		this.UpdateDisplayInfo(base_ev_data, ref this.immunityDisplayInfo, sickMinions);
		this.OnSickTooltip();
		this.SickTooltip.forceRefresh = true;
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
			if (button != PointerEventData.InputButton.Right)
			{
				return;
			}
			display_info.selectedIndex = -1;
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
				return;
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

	public LocText SickText;

	public ToolTip SickTooltip;

	public TextStyleSetting ToolTipStyle_Header;

	public TextStyleSetting ToolTipStyle_Property;

	private bool startValuesSet;

	[SerializeField]
	private KToggle RedAlertButton;

	public ToolTip RedAlertTooltip;

	private MeterScreen.DisplayInfo stressDisplayInfo = new MeterScreen.DisplayInfo
	{
		selectedIndex = -1
	};

	private MeterScreen.DisplayInfo immunityDisplayInfo = new MeterScreen.DisplayInfo
	{
		selectedIndex = -1
	};

	private int cachedMinionCount = -1;

	private long cachedCalories = -1L;

	private Dictionary<string, float> rationsDict = new Dictionary<string, float>();

	private struct DisplayInfo
	{
		public int selectedIndex;
	}
}
