using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/GetBalloonWorkable")]
public class GetBalloonWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.faceTargetWhenWorking = true;
		this.workerStatusItem = null;
		this.workingStatusItem = null;
		this.workAnims = GetBalloonWorkable.GET_BALLOON_ANIMS;
		this.workingPstComplete = new HashedString[] { GetBalloonWorkable.PST_ANIM };
		this.workingPstFailed = new HashedString[] { GetBalloonWorkable.PST_ANIM };
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.balloonArtist.GiveBalloon();
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("EquippableBalloon"), worker.transform.GetPosition());
		gameObject.GetComponent<Equippable>().Assign(worker.GetComponent<MinionIdentity>());
		gameObject.GetComponent<Equippable>().isEquipped = true;
		gameObject.SetActive(true);
		base.OnCompleteWork(worker);
	}

	public override Vector3 GetFacingTarget()
	{
		return this.balloonArtist.master.transform.GetPosition();
	}

	public void SetBalloonArtist(BalloonArtistChore.StatesInstance chore)
	{
		this.balloonArtist = chore;
	}

	private static readonly HashedString[] GET_BALLOON_ANIMS = new HashedString[] { "working_pre", "working_loop" };

	private static readonly HashedString PST_ANIM = new HashedString("working_pst");

	private BalloonArtistChore.StatesInstance balloonArtist;
}
