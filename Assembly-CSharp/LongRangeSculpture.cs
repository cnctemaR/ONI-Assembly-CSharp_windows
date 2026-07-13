using System;

public class LongRangeSculpture : Sculpture
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = null;
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
		this.multitoolContext = "dig";
		this.multitoolHitEffectTag = "fx_dig_splash";
	}
}
