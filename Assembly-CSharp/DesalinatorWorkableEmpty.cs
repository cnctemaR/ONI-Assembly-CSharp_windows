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
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_desalinator_kanim") };
		this.workAnims = DesalinatorWorkableEmpty.WORK_ANIMS;
		this.workingPstComplete = new HashedString[] { DesalinatorWorkableEmpty.PST_ANIM };
		this.workingPstFailed = new HashedString[] { DesalinatorWorkableEmpty.PST_ANIM };
		this.synchronizeAnims = false;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.timesCleaned++;
		base.OnCompleteWork(worker);
	}

	[Serialize]
	public int timesCleaned;

	private static readonly HashedString[] WORK_ANIMS = new HashedString[] { "salt_pre", "salt_loop" };

	private static readonly HashedString PST_ANIM = new HashedString("salt_pst");
}
