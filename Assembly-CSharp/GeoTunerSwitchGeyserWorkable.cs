using System;

public class GeoTunerSwitchGeyserWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_use_remote_kanim") };
		this.faceTargetWhenWorking = true;
		this.synchronizeAnims = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.SetWorkTime(3f);
	}

	private const string animName = "anim_use_remote_kanim";
}
