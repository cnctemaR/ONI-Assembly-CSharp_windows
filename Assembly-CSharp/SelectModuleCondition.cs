using System;
using UnityEngine;

public abstract class SelectModuleCondition
{
	public abstract bool EvaluateCondition(GameObject existingModule, BuildingDef selectedPart, SelectModuleCondition.SelectionContext selectionContext);

	public abstract string GetStatusTooltip(bool ready, GameObject moduleBase, BuildingDef selectedPart);

	public virtual bool IgnoreInSanboxMode()
	{
		return false;
	}

	public enum SelectionContext
	{
		AddModuleAbove,
		AddModuleBelow,
		ReplaceModule
	}
}
