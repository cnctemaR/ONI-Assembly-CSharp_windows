using System;
using TUNING;

public class EmptyLitterboxWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workingStatusItem = Db.Get().BuildingStatusItems.LitterBoxBeingEmptied;
		base.SetWorkerStatusItem(Db.Get().DuplicantStatusItems.EmptyingLitterBox);
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_compost_kanim") };
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.BARELY_EVER_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
		this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		this.lightEfficiencyBonus = true;
		this.synchronizeAnims = true;
		this.shouldShowSkillPerkStatusItem = false;
		base.SetWorkTime(15f);
	}
}
