using System;
using STRINGS;

public class ToolMenuPriorityScreen : PriorityScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.buttons = base.InstantiateButtons(new Action<int>(this.OnClick), UI.PRIORITYSCREEN.TOOLPRIORITYTOOLTIP, false);
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
		base.SetScreenPriority(5);
	}

	private void OnClick(int priority)
	{
		base.SetScreenPriority(priority);
	}

	public static ToolMenuPriorityScreen Instance;
}
