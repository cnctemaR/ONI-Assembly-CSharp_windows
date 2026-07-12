using System;
using STRINGS;
using UnityEngine;

public class PlaceSpaceAvailable : SelectModuleCondition
{
	public override bool EvaluateCondition(GameObject existingModule, BuildingDef selectedPart, SelectModuleCondition.SelectionContext selectionContext)
	{
		BuildingAttachPoint component = existingModule.GetComponent<BuildingAttachPoint>();
		switch (selectionContext)
		{
		case SelectModuleCondition.SelectionContext.AddModuleAbove:
		{
			if (component != null && component.points[0].attachedBuilding != null && component.points[0].attachedBuilding.HasTag(GameTags.RocketModule) && !component.points[0].attachedBuilding.GetComponent<ReorderableBuilding>().CanMoveVertically(selectedPart.HeightInCells, null))
			{
				return false;
			}
			int num = Grid.OffsetCell(Grid.PosToCell(existingModule), 0, existingModule.GetComponent<Building>().Def.HeightInCells);
			foreach (CellOffset cellOffset in selectedPart.PlacementOffsets)
			{
				if (!ReorderableBuilding.CheckCellClear(Grid.OffsetCell(num, cellOffset), existingModule))
				{
					return false;
				}
			}
			return true;
		}
		case SelectModuleCondition.SelectionContext.AddModuleBelow:
		{
			if (!existingModule.GetComponent<ReorderableBuilding>().CanMoveVertically(selectedPart.HeightInCells, null))
			{
				return false;
			}
			int num2 = Grid.PosToCell(existingModule);
			foreach (CellOffset cellOffset2 in selectedPart.PlacementOffsets)
			{
				if (!ReorderableBuilding.CheckCellClear(Grid.OffsetCell(num2, cellOffset2), existingModule))
				{
					return false;
				}
			}
			return true;
		}
		case SelectModuleCondition.SelectionContext.ReplaceModule:
		{
			int num3 = selectedPart.HeightInCells - existingModule.GetComponent<Building>().Def.HeightInCells;
			if (component != null && component.points[0].attachedBuilding != null && component.points[0].attachedBuilding.HasTag(GameTags.RocketModule))
			{
				ReorderableBuilding component2 = existingModule.GetComponent<ReorderableBuilding>();
				if (!component.points[0].attachedBuilding.GetComponent<ReorderableBuilding>().CanMoveVertically(num3, component2.gameObject))
				{
					return false;
				}
			}
			ReorderableBuilding component3 = existingModule.GetComponent<ReorderableBuilding>();
			foreach (CellOffset cellOffset3 in selectedPart.PlacementOffsets)
			{
				if (!ReorderableBuilding.CheckCellClear(Grid.OffsetCell(Grid.PosToCell(component3), cellOffset3), component3.gameObject))
				{
					return false;
				}
			}
			return true;
		}
		default:
			return true;
		}
	}

	public override string GetStatusTooltip(bool ready, GameObject moduleBase, BuildingDef selectedPart)
	{
		if (ready)
		{
			return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.SPACE_AVAILABLE.COMPLETE;
		}
		return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.SPACE_AVAILABLE.FAILED;
	}
}
