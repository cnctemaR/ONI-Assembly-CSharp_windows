using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class CookingStation : ComplexFabricator, IEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreType = Db.Get().ChoreTypes.Cook;
		this.fetchChoreTypeIdHash = Db.Get().ChoreTypes.CookFetch.IdHash;
		this.choreTags = GameTags.ChoreTypes.CookingChores;
		this.workable.requiredRolePerk = RoleManager.rolePerks.CanElectricGrill.id;
		this.workable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Cooking;
		this.workable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_cookstation_kanim") };
		this.workable.AttributeConvertor = Db.Get().AttributeConverters.CookingSpeed;
		this.workable.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		ComplexFabricatorWorkable workable = this.workable;
		workable.OnWorkTickActions = (Action<Worker, float>)Delegate.Combine(workable.OnWorkTickActions, new Action<Worker, float>(delegate(Worker worker, float dt)
		{
			if (this.diseaseCountKillRate > 0)
			{
				PrimaryElement component = base.GetComponent<PrimaryElement>();
				int num = Math.Max(1, (int)((float)this.diseaseCountKillRate * dt));
				component.ModifyDiseaseCount(-num, "CookingStation");
			}
		}));
	}

	protected override List<GameObject> SpawnOrderProduct(ComplexFabricator.UserOrder completed_order)
	{
		List<GameObject> list = base.SpawnOrderProduct(completed_order);
		foreach (GameObject gameObject in list)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			component.ModifyDiseaseCount(-component.DiseaseCount, "CookingStation.CompleteOrder");
			component.Temperature = 368.15f;
		}
		base.GetComponent<Operational>().SetActive(false, false);
		return list;
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
