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
		new Dictionary<string, float>().Add(SimHashes.Ice.ToString(), 300f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, global::TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_INSULATION, global::STRINGS.EQUIPMENT.PREFABS.TEMPERATURE_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)global::TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_ATHLETICS, global::STRINGS.EQUIPMENT.PREFABS.TEMPERATURE_SUIT.NAME, false, false, true));
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("Temperature_Suit", global::TUNING.EQUIPMENT.SUITS.SLOT, SimHashes.Water, (float)global::TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_MASS, global::TUNING.EQUIPMENT.SUITS.ANIM, global::TUNING.EQUIPMENT.SUITS.SNAPON, "body_oxygen_kanim", 6, list, null, false, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, new Tag[] { GameTags.Suit }, null);
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.TEMPERATURE_SUIT.RECIPE_DESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.element = "Water";
		suitTank.amount = 100f;
		go.GetComponent<KPrefabID>().AddTag(GameTags.PedestalDisplayable, false);
	}

	public const string ID = "Temperature_Suit";
}
