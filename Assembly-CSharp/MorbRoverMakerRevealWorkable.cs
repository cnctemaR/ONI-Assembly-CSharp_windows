using System;

public class MorbRoverMakerRevealWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		this.workAnims = new HashedString[] { "reveal_working_pre", "reveal_working_loop" };
		this.workingPstComplete = new HashedString[] { "reveal_working_pst" };
		this.workingPstFailed = new HashedString[] { "reveal_working_pst" };
		base.OnPrefabInit();
		this.workingStatusItem = Db.Get().BuildingStatusItems.MorbRoverMakerBuildingRevealed;
		base.SetWorkerStatusItem(Db.Get().DuplicantStatusItems.MorbRoverMakerWorkingOnRevealing);
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_gravitas_morb_tank_kanim") };
		this.lightEfficiencyBonus = true;
		this.synchronizeAnims = true;
		base.SetWorkTime(15f);
	}

	protected override void OnStartWork(WorkerBase worker)
	{
		base.OnStartWork(worker);
	}

	public const string WORKABLE_PRE_ANIM_NAME = "reveal_working_pre";

	public const string WORKABLE_LOOP_ANIM_NAME = "reveal_working_loop";

	public const string WORKABLE_PST_ANIM_NAME = "reveal_working_pst";
}
