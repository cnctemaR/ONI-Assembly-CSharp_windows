using System;
using TUNING;

public class CompleteIncubationWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.synchronizeAnims = true;
		this.faceTargetWhenWorking = true;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_incubator_kanim") };
		this.workAnims = new HashedString[] { "hatching" };
		this.workPstAnim = "hatching";
		base.SetWorkTime(0f);
		this.showProgressBar = false;
		this.requiredRolePerk = RoleManager.rolePerks.CanWrangleCreatures.id;
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole("Rancher", work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_QUICK);
		resume.AddExperienceIfRole("SeniorRancher", work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_QUICK);
	}
}
