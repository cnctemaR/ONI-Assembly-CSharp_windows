using System;
using TUNING;

public class DoctorChore : Workable
{
	private DoctorChore()
	{
		this.synchronizeAnims = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.attributeConverter = Db.Get().AttributeConverters.DoctorSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.BARELY_EVER_EXPERIENCE;
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
	}
}
