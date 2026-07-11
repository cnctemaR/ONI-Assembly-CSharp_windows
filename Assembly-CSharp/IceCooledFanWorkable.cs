using System;
using TUNING;

public class IceCooledFanWorkable : Workable
{
	private IceCooledFanWorkable()
	{
		this.showProgressBar = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
		this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		this.workerStatusItem = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnStartWork(Worker worker)
	{
		this.operational.SetActive(true, false);
	}

	protected override void OnStopWork(Worker worker)
	{
		this.operational.SetActive(false, false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.operational.SetActive(false, false);
	}

	[MyCmpGet]
	private Operational operational;
}
