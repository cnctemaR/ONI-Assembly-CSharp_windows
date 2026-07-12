using System;
using STRINGS;
using UnityEngine;

public class EngineOnBottom : SelectModuleCondition
{
	public override bool EvaluateCondition(GameObject existingModule, BuildingDef selectedPart, SelectModuleCondition.SelectionContext selectionContext)
	{
		if (existingModule == null || existingModule.GetComponent<LaunchPad>() != null)
		{
			return true;
		}
		if (selectionContext == SelectModuleCondition.SelectionContext.ReplaceModule)
		{
			return existingModule.GetComponent<AttachableBuilding>().GetAttachedTo() == null;
		}
		return selectionContext == SelectModuleCondition.SelectionContext.AddModuleBelow && existingModule.GetComponent<AttachableBuilding>().GetAttachedTo() == null;
	}

	public override string GetStatusTooltip(bool ready, GameObject moduleBase, BuildingDef selectedPart)
	{
		if (ready)
		{
			return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.ENGINE_AT_BOTTOM.COMPLETE;
		}
		return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.ENGINE_AT_BOTTOM.FAILED;
	}
}
