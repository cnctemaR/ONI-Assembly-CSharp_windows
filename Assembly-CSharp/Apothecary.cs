using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class Apothecary : ComplexFabricator, IGameObjectEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreType = Db.Get().ChoreTypes.Compound;
		this.fetchChoreTypeIdHash = Db.Get().ChoreTypes.DoctorFetch.IdHash;
		this.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.workable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
		this.workable.AttributeConverter = Db.Get().AttributeConverters.CompoundingSpeed;
		this.workable.SkillExperienceSkillGroup = Db.Get().SkillGroups.MedicalAid.Id;
		this.workable.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		this.workable.requiredSkillPerk = Db.Get().SkillPerks.CanCompound.Id;
		this.workable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_apothecary_kanim") };
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> descriptors = base.GetDescriptors(go);
		descriptors.AddRange(new List<Descriptor>
		{
			new Descriptor(UI.BUILDINGEFFECTS.PRODUCESMEDICINE, UI.BUILDINGEFFECTS.TOOLTIPS.PRODUCESMEDICINE, Descriptor.DescriptorType.Effect, false)
		});
		return descriptors;
	}
}
