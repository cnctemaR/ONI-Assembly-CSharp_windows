using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class BuildToolHoverTextCard : HoverTextConfiguration
{
	public override void UpdateHoverElements(List<KSelectable> hoverObjects_dont_use_this_is_null)
	{
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		hoverTextDrawer.BeginShadowBar(false);
		this.ActionName = ((!(this.currentDef != null) || !this.currentDef.DragBuild) ? UI.TOOLS.BUILD.TOOLACTION : UI.TOOLS.BUILD.TOOLACTION_DRAG);
		if (this.currentDef != null && this.currentDef.Name != null)
		{
			this.ToolName = string.Format(UI.TOOLS.BUILD.NAME, this.currentDef.Name);
		}
		base.DrawTitle(instance, hoverTextDrawer);
		base.DrawInstructions(instance, hoverTextDrawer);
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
		int num2 = 26;
		int num3 = 8;
		if (this.currentDef != null)
		{
			if (PlayerController.Instance.ActiveTool != null)
			{
				Type type = PlayerController.Instance.ActiveTool.GetType();
				if (typeof(BuildTool).IsAssignableFrom(type) || typeof(BaseUtilityBuildTool).IsAssignableFrom(type))
				{
					if (this.currentDef.BuildingComplete.GetComponent<Rotatable>() != null)
					{
						hoverTextDrawer.NewLine(num2);
						hoverTextDrawer.AddIndent(num3);
						string text = UI.TOOLTIPS.HELP_ROTATE_KEY.ToString();
						text = text.Replace("{Key}", GameUtil.GetActionString(global::Action.RotateBuilding));
						hoverTextDrawer.DrawText(text, this.Styles_Instruction.Standard);
					}
					Orientation getBuildingOrientation = BuildTool.Instance.GetBuildingOrientation;
					string text2 = "Unknown reason";
					Vector3 vector = Grid.CellToPosCCC(num, Grid.SceneLayer.Building);
					if (!this.currentDef.IsValidPlaceLocation(BuildTool.Instance.visualizer, vector, getBuildingOrientation, out text2))
					{
						hoverTextDrawer.NewLine(num2);
						hoverTextDrawer.AddIndent(num3);
						hoverTextDrawer.DrawText(text2, this.HoverTextStyleSettings[1]);
					}
					RoomTracker component = this.currentDef.BuildingComplete.GetComponent<RoomTracker>();
					if (component != null && !component.SufficientBuildLocation(num))
					{
						hoverTextDrawer.NewLine(num2);
						hoverTextDrawer.AddIndent(num3);
						hoverTextDrawer.DrawText(UI.TOOLTIPS.HELP_REQUIRES_ROOM, this.HoverTextStyleSettings[1]);
					}
				}
			}
			CircuitManager circuitManager = Game.Instance.circuitManager;
			ushort circuitID = circuitManager.GetCircuitID(num);
			if (circuitID != 65535)
			{
				float num4 = circuitManager.GetWattsNeededWhenActive(circuitID);
				num4 += this.currentDef.EnergyConsumptionWhenActive;
				float maxSafeWattageForCircuit = circuitManager.GetMaxSafeWattageForCircuit(circuitID);
				Color color = ((num4 < maxSafeWattageForCircuit) ? Color.white : Color.red);
				hoverTextDrawer.NewLine(num2);
				hoverTextDrawer.AddIndent(num3);
				hoverTextDrawer.DrawText(string.Format(UI.DETAILTABS.ENERGYGENERATOR.POTENTIAL_WATTAGE_CONSUMED, GameUtil.GetFormattedWattage(num4, GameUtil.WattageFormatterUnit.Automatic)), this.Styles_BodyText.Standard, color, true);
			}
			hoverTextDrawer.NewLine(num2);
			hoverTextDrawer.AddIndent(num3);
			hoverTextDrawer.DrawText(ResourceRemainingDisplayScreen.instance.GetString(), this.Styles_BodyText.Standard);
		}
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
	}

	public BuildingDef currentDef;
}
