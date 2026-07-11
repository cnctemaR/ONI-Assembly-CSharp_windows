using System;
using TUNING;

public class EggIncubatorWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.synchronizeAnims = false;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_incubator_kanim") };
		base.SetWorkTime(15f);
		this.showProgressBar = true;
		this.requiredRolePerk = RoleManager.rolePerks.CanWrangleCreatures.id;
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole("Rancher", work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_QUICK);
		resume.AddExperienceIfRole("SeniorRancher", work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_QUICK);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		EggIncubator component = base.GetComponent<EggIncubator>();
		if (component && component.Occupant)
		{
			IncubationMonitor.Instance smi = component.Occupant.GetSMI<IncubationMonitor.Instance>();
			if (smi != null)
			{
				smi.ApplySongBuff();
			}
		}
	}
}
