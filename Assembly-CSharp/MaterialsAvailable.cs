using System;
using STRINGS;
using UnityEngine;

public class MaterialsAvailable : SelectModuleCondition
{
	public override bool IgnoreInSanboxMode()
	{
		return true;
	}

	public override bool EvaluateCondition(GameObject existingModule, BuildingDef selectedPart, SelectModuleCondition.SelectionContext selectionContext)
	{
		return existingModule == null || ProductInfoScreen.MaterialsMet(selectedPart.CraftRecipe);
	}

	public override string GetStatusTooltip(bool ready, GameObject moduleBase, BuildingDef selectedPart)
	{
		if (ready)
		{
			return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.MATERIALS_AVAILABLE.COMPLETE;
		}
		string text = UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.MATERIALS_AVAILABLE.FAILED;
		foreach (Recipe.Ingredient ingredient in selectedPart.CraftRecipe.Ingredients)
		{
			string text2 = "\n" + string.Format("{0}{1}: {2}", "• ", ingredient.tag.ProperName(), GameUtil.GetFormattedMass(ingredient.amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
			text += text2;
		}
		return text;
	}
}
