using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class NoFreeRocketInterior : SelectModuleCondition
{
	public override bool EvaluateCondition(GameObject existingModule, BuildingDef selectedPart, SelectModuleCondition.SelectionContext selectionContext)
	{
		int num = 0;
		using (List<WorldContainer>.Enumerator enumerator = ClusterManager.Instance.WorldContainers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsModuleInterior)
				{
					num++;
				}
			}
		}
		return num < ClusterManager.MAX_ROCKET_INTERIOR_COUNT;
	}

	public override string GetStatusTooltip(bool ready, GameObject moduleBase, BuildingDef selectedPart)
	{
		if (ready)
		{
			return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.PASSENGER_MODULE_AVAILABLE.COMPLETE;
		}
		return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.PASSENGER_MODULE_AVAILABLE.FAILED;
	}
}
