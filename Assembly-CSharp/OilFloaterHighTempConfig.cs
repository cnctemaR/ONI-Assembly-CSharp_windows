using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class OilFloaterHighTempConfig : IEntityConfig
{
	public static GameObject CreateOilFloater(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = BaseOilFloaterConfig.BaseOilFloater(id, name, desc, anim_file, "OilfloaterHighTempBaseTrait", 373.15f, 473.15f, 323.15f, 573.15f, is_baby, "hot_");
		EntityTemplates.ExtendEntityToWildCreature(gameObject, OilFloaterTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("OilfloaterHighTempBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, OilFloaterTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -OilFloaterTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		return BaseOilFloaterConfig.SetupDiet(gameObject, SimHashes.CarbonDioxide.CreateTag(), SimHashes.Petroleum.CreateTag(), OilFloaterHighTempConfig.CALORIES_PER_KG_OF_ORE, global::TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f, OilFloaterHighTempConfig.MIN_POOP_SIZE_IN_KG);
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = OilFloaterHighTempConfig.CreateOilFloater("OilfloaterHighTemp", global::STRINGS.CREATURES.SPECIES.OILFLOATER.VARIANT_HIGHTEMP.NAME, global::STRINGS.CREATURES.SPECIES.OILFLOATER.VARIANT_HIGHTEMP.DESC, "oilfloater_kanim", false);
		string text = "OilfloaterHighTempEgg";
		string text2 = global::STRINGS.CREATURES.SPECIES.OILFLOATER.VARIANT_HIGHTEMP.EGG_NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.OILFLOATER.VARIANT_HIGHTEMP.DESC;
		string text4 = "egg_oilfloater_kanim";
		float egg_MASS = OilFloaterTuning.EGG_MASS;
		string text5 = "OilfloaterHighTempBaby";
		float num = 60.000004f;
		float num2 = 20f;
		List<FertilityMonitor.BreedingChance> egg_CHANCES_HIGHTEMP = OilFloaterTuning.EGG_CHANCES_HIGHTEMP;
		int egg_SORT_ORDER = OilFloaterHighTempConfig.EGG_SORT_ORDER;
		EntityTemplates.ExtendEntityToFertileCreature(gameObject, text, text2, text3, text4, egg_MASS, text5, num, num2, egg_CHANCES_HIGHTEMP, this.GetDlcIds(), egg_SORT_ORDER, true, false, true, 1f, false);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "OilfloaterHighTemp";

	public const string BASE_TRAIT_ID = "OilfloaterHighTempBaseTrait";

	public const string EGG_ID = "OilfloaterHighTempEgg";

	public const SimHashes CONSUME_ELEMENT = SimHashes.CarbonDioxide;

	public const SimHashes EMIT_ELEMENT = SimHashes.Petroleum;

	private static float KG_ORE_EATEN_PER_CYCLE = 20f;

	private static float CALORIES_PER_KG_OF_ORE = OilFloaterTuning.STANDARD_CALORIES_PER_CYCLE / OilFloaterHighTempConfig.KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 0.5f;

	public static int EGG_SORT_ORDER = OilFloaterConfig.EGG_SORT_ORDER + 1;
}
