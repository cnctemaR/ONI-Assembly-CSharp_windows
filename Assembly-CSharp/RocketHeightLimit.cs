using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class RocketHeightLimit : SelectModuleCondition
{
	public override bool EvaluateCondition(GameObject existingModule, BuildingDef selectedPart, SelectModuleCondition.SelectionContext selectionContext)
	{
		int num = selectedPart.HeightInCells;
		if (selectionContext == SelectModuleCondition.SelectionContext.ReplaceModule)
		{
			num -= existingModule.GetComponent<Building>().Def.HeightInCells;
		}
		if (existingModule == null)
		{
			return true;
		}
		RocketModuleCluster component = existingModule.GetComponent<RocketModuleCluster>();
		if (component == null)
		{
			return true;
		}
		int num2 = component.CraftInterface.MaxHeight;
		if (num2 <= 0)
		{
			num2 = ROCKETRY.ROCKET_HEIGHT.MAX_MODULE_STACK_HEIGHT;
		}
		RocketEngineCluster component2 = existingModule.GetComponent<RocketEngineCluster>();
		RocketEngineCluster component3 = selectedPart.BuildingComplete.GetComponent<RocketEngineCluster>();
		if (selectionContext == SelectModuleCondition.SelectionContext.ReplaceModule && component2 != null)
		{
			if (component3 != null)
			{
				num2 = component3.maxHeight;
			}
			else
			{
				num2 = ROCKETRY.ROCKET_HEIGHT.MAX_MODULE_STACK_HEIGHT;
			}
		}
		if (component3 != null && selectionContext == SelectModuleCondition.SelectionContext.AddModuleBelow)
		{
			num2 = component3.maxHeight;
		}
		return num2 == -1 || component.CraftInterface.RocketHeight + num <= num2;
	}

	public override string GetStatusTooltip(bool ready, GameObject moduleBase, BuildingDef selectedPart)
	{
		global::UnityEngine.Object component = moduleBase.GetComponent<RocketEngineCluster>();
		RocketEngineCluster component2 = selectedPart.BuildingComplete.GetComponent<RocketEngineCluster>();
		bool flag = component != null || component2 != null;
		if (ready)
		{
			return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.MAX_HEIGHT.COMPLETE;
		}
		if (flag)
		{
			return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.MAX_HEIGHT.FAILED;
		}
		return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.MAX_HEIGHT.FAILED_NO_ENGINE;
	}
}
