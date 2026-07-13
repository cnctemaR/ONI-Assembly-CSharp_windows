using System;
using TUNING;

public class FoodSmokerWorkableEmpty : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Emptying;
		this.workAnims = FoodSmokerWorkableEmpty.WORK_ANIMS;
		this.workingPstComplete = FoodSmokerWorkableEmpty.WORK_ANIMS_PST;
		this.workingPstFailed = FoodSmokerWorkableEmpty.WORK_ANIMS_FAIL_PST;
		this.requiredSkillPerk = Db.Get().SkillPerks.CanGasRange.Id;
		this.attributeConverter = Db.Get().AttributeConverters.CookingSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.FULL_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Cooking.Id;
		this.skillExperienceMultiplier = SKILLS.FULL_EXPERIENCE;
	}

	private static readonly HashedString[] WORK_ANIMS = new HashedString[] { "empty_pre", "empty_loop" };

	private static readonly HashedString[] WORK_ANIMS_PST = new HashedString[] { "empty_pst" };

	private static readonly HashedString[] WORK_ANIMS_FAIL_PST = new HashedString[] { "" };
}
