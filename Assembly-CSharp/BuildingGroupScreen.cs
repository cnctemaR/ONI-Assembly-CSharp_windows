using System;

public class BuildingGroupScreen : KScreen
{
	protected override void OnPrefabInit()
	{
		BuildingGroupScreen.Instance = this;
		base.OnPrefabInit();
		base.ConsumeMouseScroll = true;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		base.ConsumeMouseScroll = true;
	}

	public static BuildingGroupScreen Instance;
}
