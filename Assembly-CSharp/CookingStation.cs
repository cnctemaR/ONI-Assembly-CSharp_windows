using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class CookingStation : Fabricator, IEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreType = Db.Get().ChoreTypes.Cook;
		this.fetchChoreTypeIdHash = Db.Get().ChoreTypes.CookFetch.IdHash;
		this.choreTags = GameTags.ChoreTypes.CookingChores;
		this.requiredRolePerk = RoleManager.rolePerks.CanElectricGrill.id;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Cooking;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_cookstation_kanim") };
		this.attributeConverter = Db.Get().AttributeConverters.CookingSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (this.diseaseCountKillRate > 0)
		{
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			int num = Math.Max(1, (int)((float)this.diseaseCountKillRate * dt));
			component.ModifyDiseaseCount(-num, "CookingStation");
		}
		return false;
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(Cook.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	protected override GameObject CompleteOrder(Fabricator.UserOrder completed_order)
	{
		GameObject gameObject = base.CompleteOrder(completed_order);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.ModifyDiseaseCount(-component.DiseaseCount, "CookingStation.CompleteOrder");
		component.Temperature = 368.15f;
		base.GetComponent<Operational>().SetActive(false, false);
		return gameObject;
	}

	public override List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> descriptors = base.GetDescriptors(def);
		descriptors.Add(new Descriptor(UI.BUILDINGEFFECTS.REMOVES_DISEASE, UI.BUILDINGEFFECTS.TOOLTIPS.REMOVES_DISEASE, Descriptor.DescriptorType.Effect, false));
		return descriptors;
	}

	[SerializeField]
	private int diseaseCountKillRate = 100;
}
