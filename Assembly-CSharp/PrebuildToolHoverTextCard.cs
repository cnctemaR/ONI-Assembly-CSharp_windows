using System;
using STRINGS;
using UnityEngine;

public class PrebuildToolHoverTextCard : HoverTextConfiguration
{
	public override void ConfigureHoverScreen()
	{
		HoverTextScreen instance = HoverTextScreen.Instance;
		if (instance.LoadPreConfiguredToolFields(this))
		{
			this.isConfigured = true;
		}
		else
		{
			instance.currentConfiguration = this;
			instance.ToggleIncubating(true);
			int num = 16;
			int num2 = 46;
			instance.ClearLabels();
			instance.StartShadowBar((float)num2, 0f, false);
			instance.NewLine("Line_NoSelection", num);
			instance.AddUnBoundedIcon(instance.GetSprite("inspectorUI_cannot_build"), new Color(0.95686275f, 0.2901961f, 0.2784314f), 27f, 27f, 36f, 18f);
			PlanScreen.RequirementsState requirementsState = this.currentReqState;
			if (requirementsState != PlanScreen.RequirementsState.Complete && requirementsState != PlanScreen.RequirementsState.Materials)
			{
				if (requirementsState == PlanScreen.RequirementsState.Tech)
				{
					TechItem techItem = Db.Get().TechItems.Get(this.currentDef.PrefabID);
					Tech parentTech = techItem.parentTech;
					instance.AddText(string.Format(UI.PRODUCTINFO_RESEARCHREQUIRED, parentTech.Name), this.HoverTextStyleSettings[0], true);
				}
			}
			else
			{
				instance.AddText(UI.TOOLTIPS.NOMATERIAL, this.HoverTextStyleSettings[0], true);
				instance.NewLine("warningLine", num + 6);
				instance.AddIndent(36f, 18f);
				instance.AddText(UI.TOOLTIPS.SELECTAMATERIAL, this.HoverTextStyleSettings[1], false);
			}
			instance.EndShadowBar();
			instance.StartShadowBar((float)num2, 360f, false);
			instance.NewLine("NewLine", 24);
			instance.AddIndent(34f, 18f);
			instance.AddIcon(instance.GetSprite("icon_mouse_right"), 17f, 17f, Color.white);
			instance.AddText(UI.TOOLS.GENERIC.BACK, this.Styles_Instruction.Standard, false);
			instance.EndShadowBar();
			this.isConfigured = true;
		}
	}

	public PlanScreen.RequirementsState currentReqState;

	public BuildingDef currentDef;
}
