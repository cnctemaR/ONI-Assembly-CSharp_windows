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

	public override void SetStage(string stage_id, bool skip_effect)
	{
		base.SetStage(stage_id, skip_effect);
		if (Db.GetArtableStages().Get(stage_id) == null)
		{
			Debug.LogError("Missing stage: " + stage_id);
		}
	}
}
