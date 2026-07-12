using System;
using TUNING;

public class EmptyMilkSeparatorWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workLayer = Grid.SceneLayer.BuildingFront;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Cleaning;
		this.workingStatusItem = Db.Get().MiscStatusItems.Cleaning;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_milk_separator_kanim") };
		this.attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		base.SetWorkTime(15f);
		this.synchronizeAnims = true;
	}

	public override void OnPendingCompleteWork(Worker worker)
	{
		global::System.Action onWork_PST_Begins = this.OnWork_PST_Begins;
		if (onWork_PST_Begins != null)
		{
			onWork_PST_Begins();
		}
		base.OnPendingCompleteWork(worker);
	}

	public global::System.Action OnWork_PST_Begins;
}
