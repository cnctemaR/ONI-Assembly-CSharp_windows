using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class MicrobeMusher : ComplexFabricator
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreType = Db.Get().ChoreTypes.Cook;
		this.choreTags = GameTags.ChoreTypes.CookingChores;
		this.workable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Mushing;
		this.workable.AttributeConverter = Db.Get().AttributeConverters.CookingSpeed;
		this.workable.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		this.workable.SkillExperienceSkillGroup = Db.Get().SkillGroups.Cooking.Id;
		this.workable.SkillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		this.fetchChoreTypeIdHash = Db.Get().ChoreTypes.CookFetch.IdHash;
		this.workable.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new string[] { "meter_target", "meter_ration" });
		this.workable.meter.meterController.SetSymbolVisiblity(MicrobeMusher.canHash, false);
		this.workable.meter.meterController.SetSymbolVisiblity(MicrobeMusher.meterRationHash, false);
		this.workable.meter.meterController.GetComponent<KBatchedAnimTracker>().skipInitialDisable = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("WaterFetchingTutorial", 2f, delegate(object obj)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_FetchingWater);
		}, null, null);
	}

	protected override List<GameObject> SpawnOrderProduct(ComplexFabricator.UserOrder completed_order)
	{
		List<GameObject> list = base.SpawnOrderProduct(completed_order);
		foreach (GameObject gameObject in list)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			if (component != null)
			{
				if (gameObject.PrefabID() == "MushBar")
				{
					byte index = Db.Get().Diseases.GetIndex("FoodPoisoning");
					component.AddDisease(index, 1000, "Made of mud");
				}
				if (gameObject.GetComponent<PrimaryElement>().DiseaseCount > 0)
				{
					Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_DiseaseCooking);
				}
			}
		}
		return list;
	}

	[SerializeField]
	public Vector3 mushbarSpawnOffset = Vector3.right;

	private static readonly KAnimHashedString meterRationHash = new KAnimHashedString("meter_ration");

	private static readonly KAnimHashedString canHash = new KAnimHashedString("can");
}
