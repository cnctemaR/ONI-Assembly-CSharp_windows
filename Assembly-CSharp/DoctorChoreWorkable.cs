using System;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/DoctorChoreWorkable")]
public class DoctorChoreWorkable : Workable
{
	private DoctorChoreWorkable()
	{
		this.synchronizeAnims = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.attributeConverter = Db.Get().AttributeConverters.DoctorSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.BARELY_EVER_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.MedicalAid.Id;
		this.skillExperienceMultiplier = SKILLS.BARELY_EVER_EXPERIENCE;
	}
}
