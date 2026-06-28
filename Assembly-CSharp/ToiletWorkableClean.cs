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

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.Play(ToiletWorkableClean.CleanAnims, KAnim.PlayMode.Loop);
	}

	protected override void OnStopWork(Worker worker)
	{
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.Queue("unclog_pst", KAnim.PlayMode.Once, 1f, 0f);
		base.OnStopWork(worker);
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(Handyman.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.timesCleaned++;
		base.OnCompleteWork(worker);
	}

	public override HashedString[] GetWorkAnims(Worker worker)
	{
		return ToiletWorkableClean.CleanAnims;
	}

	[Serialize]
	public int timesCleaned;

	private static readonly HashedString[] CleanAnims = new HashedString[] { "unclog_pre", "unclog_loop" };
}
