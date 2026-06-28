using System;

public class ToolMenuPriorityScreen : PriorityScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.InstantiateButtons(new Action<PriorityScreen.PriorityClass, int>(this.OnClick), "STRINGS.UI.PRIORITYSCREEN.TOOLPRIORITYTOOLTIP", false);
		ToolMenuPriorityScreen.Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.gameObject.SetActive(false);
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

	public static ToolMenuPriorityScreen Instance;
}
