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
		new Dictionary<string, float>().Add(SimHashes.DirtyWater.ToString(), 300f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_INSULATION, global::STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_ATHLETICS, global::STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.MAX_UNDERWATER_TRAVELCOST, (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_UNDERWATER_TRAVELCOST, global::STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.NAME, false, false, true));
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("Aqua_Suit", global::TUNING.EQUIPMENT.SUITS.SLOT, SimHashes.Water, (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_MASS, "suit_water_slow_kanim", global::TUNING.EQUIPMENT.SUITS.SNAPON, "body_water_slow_kanim", 6, list, null, false, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, new Tag[] { GameTags.Suit }, null);
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.RECIPE_DESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.underwaterSupport = true;
		suitTank.element = "Oxygen";
		suitTank.amount = 11f;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Clothes, false);
	}

	public const string ID = "Aqua_Suit";
}
