using System;
using System.Collections.Generic;
using STRINGS;

public class PrebuildToolHoverTextCard : HoverTextConfiguration
{
	public override void UpdateHoverElements(List<KSelectable> selected)
	{
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		hoverTextDrawer.BeginShadowBar(false);
		PlanScreen.RequirementsState requirementsState = this.currentReqState;
		if (requirementsState != PlanScreen.RequirementsState.Complete && requirementsState != PlanScreen.RequirementsState.Materials)
		{
			if (requirementsState == PlanScreen.RequirementsState.Tech)
			{
				TechItem techItem = Db.Get().TechItems.Get(this.currentDef.PrefabID);
				Tech parentTech = techItem.parentTech;
				hoverTextDrawer.DrawText(string.Format(UI.PRODUCTINFO_RESEARCHREQUIRED, parentTech.Name).ToUpper(), this.HoverTextStyleSettings[0]);
			}
		}
		else
		{
			hoverTextDrawer.DrawText(UI.TOOLTIPS.NOMATERIAL.text.ToUpper(), this.HoverTextStyleSettings[0]);
			hoverTextDrawer.NewLine(26);
			hoverTextDrawer.DrawText(UI.TOOLTIPS.SELECTAMATERIAL, this.HoverTextStyleSettings[1]);
		}
		hoverTextDrawer.NewLine(26);
		hoverTextDrawer.DrawIcon(instance.GetSprite("icon_mouse_right"), 18);
		hoverTextDrawer.DrawText(this.backStr, this.Styles_Instruction.Standard);
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
	}

	public PlanScreen.RequirementsState currentReqState;

	public BuildingDef currentDef;
}
