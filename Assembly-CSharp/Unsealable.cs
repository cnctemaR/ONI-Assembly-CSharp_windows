using System;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Unsealable")]
public class Unsealable : Workable
{
	private Unsealable()
	{
	}

	public override CellOffset[] GetOffsets(int cell)
	{
		if (this.facingRight)
		{
			return OffsetGroups.RightOnly;
		}
		return OffsetGroups.LeftOnly;
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
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.unsealed = true;
		base.OnCompleteWork(worker);
		Deconstructable component = base.GetComponent<Deconstructable>();
		if (component != null)
		{
			component.allowDeconstruction = true;
			Game.Instance.Trigger(1980521255, base.gameObject);
		}
	}

	[Serialize]
	public bool facingRight;

	[Serialize]
	public bool unsealed;
}
