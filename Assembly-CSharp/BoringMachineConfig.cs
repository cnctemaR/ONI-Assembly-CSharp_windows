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
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTEMOD_IDS.DIGGING, (float)global::TUNING.EQUIPMENT.TOOLS.BORINGMACHINE_DIG, global::STRINGS.EQUIPMENT.PREFABS.BORINGMACHINE.NAME, false));
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("BoringMachine", global::TUNING.EQUIPMENT.TOOLS.TOOLSLOT, global::TUNING.EQUIPMENT.TOOLS.TOOLFABRICATOR, global::TUNING.EQUIPMENT.TOOLS.BORINGMACHINE_FABTIME, "Iron", dictionary, (float)global::TUNING.EQUIPMENT.TOOLS.BORINGMACHINE_MASS, global::TUNING.EQUIPMENT.TOOLS.TOOL_ANIM, string.Empty, string.Empty, PathFinderFlags.None, list);
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.BORINGMACHINE.RECIPEDESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
	}
}
