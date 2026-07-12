using System;

public class FoodDehydratorWorkableEmpty : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Emptying;
		this.workAnims = FoodDehydratorWorkableEmpty.WORK_ANIMS;
		this.workingPstComplete = FoodDehydratorWorkableEmpty.WORK_ANIMS_PST;
		this.workingPstFailed = FoodDehydratorWorkableEmpty.WORK_ANIMS_FAIL_PST;
	}

	private static readonly HashedString[] WORK_ANIMS = new HashedString[] { "empty_pre", "empty_loop" };

	private static readonly HashedString[] WORK_ANIMS_PST = new HashedString[] { "empty_pst" };

	private static readonly HashedString[] WORK_ANIMS_FAIL_PST = new HashedString[] { "" };
}
