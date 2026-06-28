using System;
using KSerialization;

public class Unsealable : Workable
{
	private Unsealable()
	{
	}

	public override CellOffset[] GetOffsets()
	{
		CellOffset[] array;
		if (this.facingRight)
		{
			array = OffsetGroups.RightOnly;
		}
		else
		{
			array = OffsetGroups.LeftOnly;
		}
		return array;
	}

	protected override void OnPrefabInit()
	{
		this.faceTargetWhenWorking = true;
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_door_poi_kanim") };
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.SetWorkTime(3f);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		KBatchedAnimController component = base.gameObject.GetComponent<KBatchedAnimController>();
		component.Play("working_pre", KAnim.PlayMode.Once, 1f, 0f);
		component.Queue("working_loop", KAnim.PlayMode.Loop, 1f, 0f);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.unsealed = true;
		base.OnCompleteWork(worker);
		KBatchedAnimController component = base.gameObject.GetComponent<KBatchedAnimController>();
		component.Play("working_pst", KAnim.PlayMode.Once, 1f, 0f);
	}

	[Serialize]
	public bool facingRight;

	[Serialize]
	public bool unsealed = false;
}
