using System;

public class BuildMenuPriorityScreen : PriorityScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.InstantiateButtons(new Action<PriorityScreen.PriorityClass, int>(this.OnClick), "STRINGS.UI.PRIORITYSCREEN.BUILDMENUPRIORITYTOOLTIP", true);
		BuildMenuPriorityScreen.Instance = this;
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		base.SetScreenPriority(PriorityScreen.PriorityClass.basic, 5, false);
	}

	private void OnClick(PriorityScreen.PriorityClass priorityClass, int priority)
	{
		base.SetScreenPriority(priorityClass, priority, false);
	}

	public static BuildMenuPriorityScreen Instance;
}
