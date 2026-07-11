using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class BoringMachineConfig : IEquipmentConfig
{
	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add("Iron", 50f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.DIGGING, (float)global::TUNING.EQUIPMENT.TOOLS.BORINGMACHINE_DIG, global::STRINGS.EQUIPMENT.PREFABS.BORING_MACHINE.NAME, false, false, true));
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("BoringMachine", global::TUNING.EQUIPMENT.TOOLS.TOOLSLOT, global::TUNING.EQUIPMENT.TOOLS.TOOLFABRICATOR, (float)global::TUNING.EQUIPMENT.TOOLS.BORINGMACHINE_FABTIME, SimHashes.Iron, dictionary, (float)global::TUNING.EQUIPMENT.TOOLS.BORINGMACHINE_MASS, global::TUNING.EQUIPMENT.TOOLS.TOOL_ANIM, string.Empty, string.Empty, 4, list, null, false, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, null);
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.BORING_MACHINE.RECIPE_DESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
	}
}
