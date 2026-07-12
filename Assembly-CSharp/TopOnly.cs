using System;
using STRINGS;
using UnityEngine;

public class TopOnly : SelectModuleCondition
{
	public override bool EvaluateCondition(GameObject existingModule, BuildingDef selectedPart, SelectModuleCondition.SelectionContext selectionContext)
	{
		global::Debug.Assert(existingModule != null, "Existing module is null in top only condition");
		if (selectionContext == SelectModuleCondition.SelectionContext.ReplaceModule)
		{
			global::Debug.Assert(existingModule.GetComponent<LaunchPad>() == null, "Trying to replace launch pad with rocket module");
			return existingModule.GetComponent<BuildingAttachPoint>() == null || existingModule.GetComponent<BuildingAttachPoint>().points[0].attachedBuilding == null;
		}
		return existingModule.GetComponent<LaunchPad>() != null || (existingModule.GetComponent<BuildingAttachPoint>() != null && existingModule.GetComponent<BuildingAttachPoint>().points[0].attachedBuilding == null);
	}

	public override string GetStatusTooltip(bool ready, GameObject moduleBase, BuildingDef selectedPart)
	{
		if (ready)
		{
			return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.TOP_ONLY.COMPLETE;
		}
		return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.TOP_ONLY.FAILED;
	}
}
