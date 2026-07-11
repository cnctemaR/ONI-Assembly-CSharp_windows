using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class AquaSuitConfig : IEquipmentConfig
{
	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add(SimHashes.DirtyWater.ToString(), 300f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_INSULATION, global::STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_ATHLETICS, global::STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.MAX_UNDERWATER_TRAVELCOST, (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_UNDERWATER_TRAVELCOST, global::STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.NAME, false, false, true));
		string text = "Aqua_Suit";
		string slot = global::TUNING.EQUIPMENT.SUITS.SLOT;
		string fabricator = global::TUNING.EQUIPMENT.SUITS.FABRICATOR;
		float num = (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_FABTIME;
		SimHashes simHashes = SimHashes.Water;
		Dictionary<string, float> dictionary2 = dictionary;
		float num2 = (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_MASS;
		string text2 = "suit_water_slow_kanim";
		string snapon = global::TUNING.EQUIPMENT.SUITS.SNAPON;
		string text3 = "body_water_slow_kanim";
		int num3 = 6;
		List<AttributeModifier> list2 = list;
		Tag[] array = new Tag[] { GameTags.Suit };
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef(text, slot, fabricator, num, simHashes, dictionary2, num2, text2, snapon, text3, num3, list2, null, false, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, array);
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.RECIPE_DESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.underwaterSupport = true;
		suitTank.element = "Oxygen";
		suitTank.amount = 11f;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Clothes);
	}

	public const string ID = "Aqua_Suit";
}
