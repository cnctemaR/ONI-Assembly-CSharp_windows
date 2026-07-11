using System;
using System.Collections.Generic;
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
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.tooltip.OnComplexToolTip = new Func<List<Tuple<string, ScriptableObject>>>(SaveGame.Instance.GetColonyToolTip);
	}

	private void Update()
	{
		if (GameClock.Instance != null && this.displayedDayCount != GameUtil.GetCurrentCycle())
		{
			this.text.text = this.Days();
			this.displayedDayCount = GameUtil.GetCurrentCycle();
		}
	}

	private string Days()
	{
		return GameUtil.GetCurrentCycle().ToString();
	}

	public static global::DateTime Instance;

	public LocText day;

	private int displayedDayCount = -1;

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
