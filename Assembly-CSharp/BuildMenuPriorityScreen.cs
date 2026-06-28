using System;
using STRINGS;

public class BuildMenuPriorityScreen : PriorityScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.buttons = base.InstantiateButtons(new Action<int>(this.OnClick), UI.PRIORITYSCREEN.BUILDMENUPRIORITYTOOLTIP, true);
		BuildMenuPriorityScreen.Instance = this;
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		base.SetScreenPriority(5, false);
	}

	private void OnClick(int priority)
	{
		base.SetScreenPriority(priority, false);
	}

	public static BuildMenuPriorityScreen Instance;
}
