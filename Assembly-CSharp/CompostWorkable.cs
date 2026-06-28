using System;
using TUNING;

public class CompostWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
	}

	protected override void OnStartWork(Worker worker)
	{
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.Play("working_pre", KAnim.PlayMode.Once, 1f, 0f);
		component.Queue("working_loop", KAnim.PlayMode.Loop, 1f, 0f);
	}

	protected override void OnStopWork(Worker worker)
	{
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.Queue("working_pst", KAnim.PlayMode.Once, 1f, 0f);
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(Handyman.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}
}
