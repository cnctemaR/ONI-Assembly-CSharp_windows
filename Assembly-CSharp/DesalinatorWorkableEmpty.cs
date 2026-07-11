using System;
using KSerialization;
using TUNING;

public class DesalinatorWorkableEmpty : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Cleaning;
		this.workingStatusItem = Db.Get().MiscStatusItems.Cleaning;
		this.attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.workAnims = DesalinatorWorkableEmpty.CLEAN_ANIMS;
		this.workingPstComplete = DesalinatorWorkableEmpty.PST_ANIM;
		this.workingPstFailed = DesalinatorWorkableEmpty.PST_ANIM;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.timesCleaned++;
		base.OnCompleteWork(worker);
	}

	[Serialize]
	public int timesCleaned;

	private static readonly HashedString[] CLEAN_ANIMS = new HashedString[] { "unclog_pre", "unclog_loop" };

	private static readonly HashedString PST_ANIM = new HashedString("unclog_pst");
}
