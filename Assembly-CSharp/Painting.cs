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

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.Paintings.Add(this);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Paintings.Remove(this);
	}
}
