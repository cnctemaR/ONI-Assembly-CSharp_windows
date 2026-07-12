using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class DateTime : KScreen
{
	public static void DestroyInstance()
	{
		global::DateTime.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		global::DateTime.Instance = this;
		this.milestoneEffect.gameObject.SetActive(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.tooltip.OnComplexToolTip = new ToolTip.ComplexTooltipDelegate(this.BuildTooltip);
		Game.Instance.Subscribe(2070437606, new Action<object>(this.OnMilestoneDayReached));
		Game.Instance.Subscribe(-720092972, new Action<object>(this.OnMilestoneDayApproaching));
	}

	private List<global::Tuple<string, TextStyleSetting>> BuildTooltip()
	{
		List<global::Tuple<string, TextStyleSetting>> colonyToolTip = SaveGame.Instance.GetColonyToolTip();
		if (TimeOfDay.IsMilestoneApproaching)
		{
			colonyToolTip.Add(new global::Tuple<string, TextStyleSetting>(" ", null));
			colonyToolTip.Add(new global::Tuple<string, TextStyleSetting>(UI.ASTEROIDCLOCK.MILESTONE_TITLE.text, ToolTipScreen.Instance.defaultTooltipHeaderStyle));
			colonyToolTip.Add(new global::Tuple<string, TextStyleSetting>(UI.ASTEROIDCLOCK.MILESTONE_DESCRIPTION.text.Replace("{0}", (GameClock.Instance.GetCycle() + 2).ToString()), ToolTipScreen.Instance.defaultTooltipBodyStyle));
		}
		return colonyToolTip;
	}

	private void Update()
	{
		if (GameClock.Instance != null && this.displayedDayCount != GameUtil.GetCurrentCycle())
		{
			this.text.text = this.Days();
			this.displayedDayCount = GameUtil.GetCurrentCycle();
		}
	}

	private void OnMilestoneDayApproaching(object data)
	{
		int num = (int)data;
		this.milestoneEffect.gameObject.SetActive(true);
		this.milestoneEffect.Play("100fx_pre", KAnim.PlayMode.Loop, 1f, 0f);
	}

	private void OnMilestoneDayReached(object data)
	{
		int num = (int)data;
		this.milestoneEffect.gameObject.SetActive(true);
		this.milestoneEffect.Play("100fx", KAnim.PlayMode.Once, 1f, 0f);
	}

	private string Days()
	{
		return GameUtil.GetCurrentCycle().ToString();
	}

	public static global::DateTime Instance;

	private const string MILESTONE_ANTICIPATION_ANIMATION_NAME = "100fx_pre";

	private const string MILESTONE_ANIMATION_NAME = "100fx";

	public LocText day;

	private int displayedDayCount = -1;

	[SerializeField]
	private KBatchedAnimController milestoneEffect;

	[SerializeField]
	private LocText text;

	[SerializeField]
	private ToolTip tooltip;

	[SerializeField]
	private TextStyleSetting tooltipstyle_Days;

	[SerializeField]
	private TextStyleSetting tooltipstyle_Playtime;

	[SerializeField]
	public KToggle scheduleToggle;
}
