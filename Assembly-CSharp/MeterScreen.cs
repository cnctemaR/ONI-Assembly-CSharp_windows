using System;
using System.Collections.Generic;
using System.Linq;
using Klei.AI;
using STRINGS;
using TUNING;
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
		MultiToggle redAlertButton = this.RedAlertButton;
		redAlertButton.onClick = (global::System.Action)Delegate.Combine(redAlertButton.onClick, new global::System.Action(delegate
		{
			this.OnRedAlertClick();
		}));
		Game.Instance.Subscribe(1983128072, delegate(object data)
		{
			this.Refresh();
		});
		Game.Instance.Subscribe(1585324898, delegate(object data)
		{
			this.RefreshRedAlertButtonState();
		});
		Game.Instance.Subscribe(-1393151672, delegate(object data)
		{
			this.RefreshRedAlertButtonState();
		});
	}

	private void OnRedAlertClick()
	{
		bool flag = !ClusterManager.Instance.activeWorld.AlertManager.IsRedAlertToggledOn();
		ClusterManager.Instance.activeWorld.AlertManager.ToggleRedAlert(flag);
		if (flag)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open", false));
			return;
		}
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
	}

	private void RefreshRedAlertButtonState()
	{
		this.RedAlertButton.ChangeState(ClusterManager.Instance.activeWorld.IsRedAlert() ? 1 : 0);
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
		this.RefreshWorldMinionIdentities();
		this.RefreshMinions();
		this.RefreshRations();
		this.RefreshStress();
		this.RefreshSick();
		this.RefreshRedAlertButtonState();
	}

	private void RefreshWorldMinionIdentities()
	{
		this.worldLiveMinionIdentities = Components.LiveMinionIdentities.GetWorldItems(ClusterManager.Instance.activeWorldId, false);
	}

	private List<MinionIdentity> GetWorldMinionIdentities()
	{
		if (this.worldLiveMinionIdentities == null)
		{
			this.RefreshWorldMinionIdentities();
		}
		return this.worldLiveMinionIdentities;
	}

	private void RefreshMinions()
	{
		int count = Components.LiveMinionIdentities.Count;
		int count2 = this.GetWorldMinionIdentities().Count;
		if (count2 == this.cachedMinionCount)
		{
			return;
		}
		this.cachedMinionCount = count2;
		string text;
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			ClusterGridEntity component = ClusterManager.Instance.activeWorld.GetComponent<ClusterGridEntity>();
			text = string.Format(UI.TOOLTIPS.METERSCREEN_POPULATION_CLUSTER, component.Name, count2, count);
			this.currentMinions.text = string.Format("{0}/{1}", count2, count);
		}
		else
		{
			this.currentMinions.text = string.Format("{0}", count);
			text = string.Format(UI.TOOLTIPS.METERSCREEN_POPULATION, count.ToString("0"));
		}
		this.MinionsTooltip.ClearMultiStringTooltip();
		this.MinionsTooltip.AddMultiStringTooltip(text, this.ToolTipStyle_Header);
	}

	private void RefreshSick()
	{
		int num = this.CountSickDupes();
		this.SickText.text = num.ToString();
	}

	private void RefreshRations()
	{
		if (this.RationsText != null && RationTracker.Get() != null)
		{
			long num = (long)RationTracker.Get().CountRations(null, ClusterManager.Instance.activeWorld.worldInventory, true);
			if (this.cachedCalories != num)
			{
				this.RationsText.text = GameUtil.GetFormattedCalories((float)num, GameUtil.TimeSlice.None, true);
				this.cachedCalories = num;
			}
		}
		this.rationsSpark.GetComponentInChildren<SparkLayer>().SetColor(((float)this.cachedCalories > (float)Components.LiveMinionIdentities.GetWorldItems(ClusterManager.Instance.activeWorldId, false).Count * 1000000f) ? Constants.NEUTRAL_COLOR : Constants.NEGATIVE_COLOR);
		this.rationsSpark.GetComponentInChildren<LineLayer>().RefreshLine(TrackerTool.Instance.GetWorldTracker<KCalTracker>(ClusterManager.Instance.activeWorldId).ChartableData(600f), "kcal");
	}

	private IList<MinionIdentity> GetStressedMinions()
	{
		Amount stress_amount = Db.Get().Amounts.Stress;
		return new List<MinionIdentity>(this.GetWorldMinionIdentities()).OrderByDescending<MinionIdentity, float>((MinionIdentity x) => stress_amount.Lookup(x).value).ToList<MinionIdentity>();
	}

	private string OnStressTooltip()
	{
		float maxSressInActiveWorld = GameUtil.GetMaxSressInActiveWorld();
		this.StressTooltip.ClearMultiStringTooltip();
		this.StressTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_AVGSTRESS, Mathf.Round(maxSressInActiveWorld).ToString() + "%"), this.ToolTipStyle_Header);
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
		int num = this.CountSickDupes();
		List<MinionIdentity> worldMinionIdentities = this.GetWorldMinionIdentities();
		this.SickTooltip.ClearMultiStringTooltip();
		this.SickTooltip.AddMultiStringTooltip(string.Format(UI.TOOLTIPS.METERSCREEN_SICK_DUPES, num.ToString()), this.ToolTipStyle_Header);
		for (int i = 0; i < worldMinionIdentities.Count; i++)
		{
			MinionIdentity minionIdentity = worldMinionIdentities[i];
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

	private int CountSickDupes()
	{
		int num = 0;
		using (List<MinionIdentity>.Enumerator enumerator = this.GetWorldMinionIdentities().GetEnumerator())
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
		float num = RationTracker.Get().CountRations(this.rationsDict, ClusterManager.Instance.activeWorld.worldInventory, true);
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
		float maxSressInActiveWorld = GameUtil.GetMaxSressInActiveWorld();
		this.StressText.text = Mathf.Round(maxSressInActiveWorld).ToString();
		WorldTracker worldTracker = TrackerTool.Instance.GetWorldTracker<StressTracker>(ClusterManager.Instance.activeWorldId);
		this.stressSpark.GetComponentInChildren<SparkLayer>().SetColor((worldTracker.GetCurrentValue() >= STRESS.ACTING_OUT_RESET) ? Constants.NEGATIVE_COLOR : Constants.NEUTRAL_COLOR);
		this.stressSpark.GetComponentInChildren<LineLayer>().RefreshLine(worldTracker.ChartableData(600f), "stressData");
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
		return this.GetWorldMinionIdentities();
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
		List<MinionIdentity> worldMinionIdentities = this.GetWorldMinionIdentities();
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
			if (worldMinionIdentities.Count < display_info.selectedIndex)
			{
				display_info.selectedIndex = -1;
			}
			if (worldMinionIdentities.Count > 0)
			{
				display_info.selectedIndex = (display_info.selectedIndex + 1) % worldMinionIdentities.Count;
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

	public GameObject stressSpark;

	public LocText RationsText;

	public ToolTip RationsTooltip;

	public GameObject rationsSpark;

	public LocText SickText;

	public ToolTip SickTooltip;

	public TextStyleSetting ToolTipStyle_Header;

	public TextStyleSetting ToolTipStyle_Property;

	private bool startValuesSet;

	public MultiToggle RedAlertButton;

	public ToolTip RedAlertTooltip;

	private MeterScreen.DisplayInfo stressDisplayInfo = new MeterScreen.DisplayInfo
	{
		selectedIndex = -1
	};

	private MeterScreen.DisplayInfo immunityDisplayInfo = new MeterScreen.DisplayInfo
	{
		selectedIndex = -1
	};

	private List<MinionIdentity> worldLiveMinionIdentities;

	private int cachedMinionCount = -1;

	private long cachedCalories = -1L;

	private Dictionary<string, float> rationsDict = new Dictionary<string, float>();

	private struct DisplayInfo
	{
		public int selectedIndex;
	}
}
