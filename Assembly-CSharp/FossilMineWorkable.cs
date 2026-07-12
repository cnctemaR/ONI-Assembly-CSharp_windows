using System;

public class FossilMineWorkable : ComplexFabricatorWorkable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.shouldShowSkillPerkStatusItem = false;
	}
}
