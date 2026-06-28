using System;

public class Painting : Artable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
		this.multitoolContext = "paint";
		this.multitoolHitEffectTag = "fx_paint_splash";
	}
}
