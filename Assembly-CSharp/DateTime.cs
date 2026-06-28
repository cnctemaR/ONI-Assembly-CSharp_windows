using System;
using STRINGS;
using UnityEngine;

public class DateTime : KScreen
{
	protected override void OnActivate()
	{
		base.OnActivate();
		global::DateTime.Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.tooltip.OnToolTip = new Func<string>(this.OnToolTip);
	}

	private void Update()
	{
		if (GameClock.Instance != null)
		{
			this.text.text = this.Days();
		}
	}

	private string Days()
	{
		return GameUtil.GetCurrentDay().ToString();
	}

	private string OnToolTip()
	{
		if (GameClock.Instance != null)
		{
			this.tooltip.ClearMultiStringTooltip();
			this.tooltip.AddMultiStringTooltip(string.Format(UI.ASTEROIDCLOCK.CYCLES_OLD, this.Days()), this.tooltipstyle_Days);
			this.tooltip.AddMultiStringTooltip(string.Format(UI.ASTEROIDCLOCK.TIME_PLAYED, (GameClock.Instance.GetTimePlayedInSeconds() / 3600f).ToString("0.00")), this.tooltipstyle_Playtime);
		}
		return "";
	}

	public static global::DateTime Instance;

	public LocText day;

	[SerializeField]
	private LocText text;

	[SerializeField]
	private ToolTip tooltip;

	[SerializeField]
	private TextStyleSetting tooltipstyle_Days;

	[SerializeField]
	private TextStyleSetting tooltipstyle_Playtime;
}
