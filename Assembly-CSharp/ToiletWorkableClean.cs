using System;
using KSerialization;
using TUNING;

public class ToiletWorkableClean : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Cleaning;
		this.workingStatusItem = Db.Get().MiscStatusItems.Cleaning;
		this.attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(Handyman.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		base.GetComponent<KAnimControllerBase>().Play(ToiletWorkableClean.CLEAN_ANIMS, KAnim.PlayMode.Loop);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.timesCleaned++;
		base.OnCompleteWork(worker);
	}

	public override HashedString[] GetWorkAnims(Worker worker)
	{
		return ToiletWorkableClean.CLEAN_ANIMS;
	}

	public override HashedString GetWorkPstAnim(Worker worker)
	{
		return ToiletWorkableClean.PST_ANIM;
	}

	[Serialize]
	public int timesCleaned;

	private static readonly HashedString[] CLEAN_ANIMS = new HashedString[] { "unclog_pre", "unclog_loop" };

	private static readonly HashedString PST_ANIM = new HashedString("unclog_pst");
}
