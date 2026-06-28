using System;

public class BuildingGroupScreen : KScreen
{
	protected override void OnPrefabInit()
	{
		BuildingGroupScreen.Instance = this;
		base.OnPrefabInit();
		this.ConsumeMouseScroll = true;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		this.ConsumeMouseScroll = true;
	}

	public static BuildingGroupScreen Instance;
}
