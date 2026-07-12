using System;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/HiveWorkableEmpty")]
public class HiveWorkableEmpty : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Emptying;
		this.attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
		this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		this.workAnims = HiveWorkableEmpty.WORK_ANIMS;
		this.workingPstComplete = new HashedString[] { HiveWorkableEmpty.PST_ANIM };
		this.workingPstFailed = new HashedString[] { HiveWorkableEmpty.PST_ANIM };
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		if (!this.wasStung)
		{
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().harvestAHiveWithoutGettingStung = true;
		}
	}

	private static readonly HashedString[] WORK_ANIMS = new HashedString[] { "working_pre", "working_loop" };

	private static readonly HashedString PST_ANIM = new HashedString("working_pst");

	public bool wasStung;
}
