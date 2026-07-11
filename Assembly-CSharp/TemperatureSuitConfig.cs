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
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, global::TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_INSULATION, global::STRINGS.EQUIPMENT.PREFABS.TEMPERATURE_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)global::TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_ATHLETICS, global::STRINGS.EQUIPMENT.PREFABS.TEMPERATURE_SUIT.NAME, false, false, true));
		string text = "Temperature_Suit";
		string slot = global::TUNING.EQUIPMENT.SUITS.SLOT;
		string fabricator = global::TUNING.EQUIPMENT.SUITS.FABRICATOR;
		float num = (float)global::TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_FABTIME;
		SimHashes simHashes = SimHashes.Water;
		Dictionary<string, float> dictionary2 = dictionary;
		float num2 = (float)global::TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_MASS;
		string anim = global::TUNING.EQUIPMENT.SUITS.ANIM;
		string snapon = global::TUNING.EQUIPMENT.SUITS.SNAPON;
		string text2 = "body_oxygen_kanim";
		int num3 = 6;
		List<AttributeModifier> list2 = list;
		Tag[] array = new Tag[] { GameTags.Suit };
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef(text, slot, fabricator, num, simHashes, dictionary2, num2, anim, snapon, text2, num3, list2, null, false, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, array);
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.TEMPERATURE_SUIT.RECIPE_DESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.element = "Water";
		suitTank.amount = 100f;
	}

	public const string ID = "Temperature_Suit";
}
