using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class TemperatureSuitConfig : IEquipmentConfig
{
	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add(SimHashes.Ice.ToString(), 300f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, (float)global::TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_INSULATION, global::STRINGS.EQUIPMENT.PREFABS.TEMPERATURE_SUIT.NAME, false, false));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)global::TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_ATHLETICS, global::STRINGS.EQUIPMENT.PREFABS.TEMPERATURE_SUIT.NAME, false, false));
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("TemperatureSuit", global::TUNING.EQUIPMENT.SUITS.SLOT, global::TUNING.EQUIPMENT.SUITS.FABRICATOR, (float)global::TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_FABTIME, SimHashes.Water, dictionary, (float)global::TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_MASS, global::TUNING.EQUIPMENT.SUITS.ANIM, global::TUNING.EQUIPMENT.SUITS.SNAPON, "body_oxygen_kanim", PathFinderFlags.SuitRequired, list, null, false, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f);
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.TEMPERATURE_SUIT.RECIPE_DESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.element = "Water";
		suitTank.amount = 100f;
	}

	public const string ID = "TemperatureSuit";
}
