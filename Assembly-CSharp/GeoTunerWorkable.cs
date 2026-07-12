using System;
using TUNING;

public class GeoTunerWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.SetWorkTime(30f);
		this.requiredSkillPerk = Db.Get().SkillPerks.AllowGeyserTuning.Id;
		base.SetWorkerStatusItem(Db.Get().DuplicantStatusItems.Studying);
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_geotuner_kanim") };
		this.attributeConverter = Db.Get().AttributeConverters.GeotuningSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		this.lightEfficiencyBonus = true;
	}
}
