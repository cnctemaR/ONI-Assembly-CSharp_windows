using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class BuildToolHoverTextCard : HoverTextConfiguration
{
	public override void UpdateHoverElements(List<KSelectable> hoverObjects)
	{
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		hoverTextDrawer.BeginShadowBar(false);
		this.ActionName = ((this.currentDef != null && this.currentDef.DragBuild) ? UI.TOOLS.BUILD.TOOLACTION_DRAG : UI.TOOLS.BUILD.TOOLACTION);
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
			hoverTextDrawer.NewLine(num2);
			hoverTextDrawer.AddIndent(num3);
			hoverTextDrawer.DrawText(ResourceRemainingDisplayScreen.instance.GetString(), this.Styles_BodyText.Standard);
			hoverTextDrawer.EndShadowBar();
			HashedString mode = SimDebugView.Instance.GetMode();
			if (mode == OverlayModes.Logic.ID && hoverObjects != null)
			{
				SelectToolHoverTextCard component2 = SelectTool.Instance.GetComponent<SelectToolHoverTextCard>();
				using (List<KSelectable>.Enumerator enumerator = hoverObjects.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KSelectable kselectable = enumerator.Current;
						LogicPorts component3 = kselectable.GetComponent<LogicPorts>();
						LogicPorts.Port port;
						bool flag;
						if (component3 != null && component3.TryGetPortAtCell(num, out port, out flag))
						{
							bool flag2 = component3.IsPortConnected(port.id);
							hoverTextDrawer.BeginShadowBar(false);
							int num4;
							if (flag)
							{
								string text3 = (port.displayCustomName ? port.description : UI.LOGIC_PORTS.PORT_INPUT_DEFAULT_NAME.text);
								num4 = component3.GetInputValue(port.id);
								hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_INPUT_HOVER_FMT.Replace("{Port}", text3).Replace("{Name}", kselectable.GetProperName().ToUpper()), component2.Styles_Title.Standard);
							}
							else
							{
								string text4 = (port.displayCustomName ? port.description : UI.LOGIC_PORTS.PORT_OUTPUT_DEFAULT_NAME.text);
								num4 = component3.GetOutputValue(port.id);
								hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_OUTPUT_HOVER_FMT.Replace("{Port}", text4).Replace("{Name}", kselectable.GetProperName().ToUpper()), component2.Styles_Title.Standard);
							}
							hoverTextDrawer.NewLine(26);
							TextStyleSetting textStyleSetting;
							if (flag2)
							{
								textStyleSetting = ((num4 == 1) ? component2.Styles_LogicActive.Selected : component2.Styles_LogicSignalInactive);
							}
							else
							{
								textStyleSetting = component2.Styles_LogicActive.Standard;
							}
							hoverTextDrawer.DrawIcon((num4 == 1 && flag2) ? component2.iconActiveAutomationPort : component2.iconDash, textStyleSetting.textColor, 18, 2);
							hoverTextDrawer.DrawText(port.activeDescription, textStyleSetting);
							hoverTextDrawer.NewLine(26);
							TextStyleSetting textStyleSetting2;
							if (flag2)
							{
								textStyleSetting2 = ((num4 == 0) ? component2.Styles_LogicStandby.Selected : component2.Styles_LogicSignalInactive);
							}
							else
							{
								textStyleSetting2 = component2.Styles_LogicStandby.Standard;
							}
							hoverTextDrawer.DrawIcon((num4 == 0 && flag2) ? component2.iconActiveAutomationPort : component2.iconDash, textStyleSetting2.textColor, 18, 2);
							hoverTextDrawer.DrawText(port.inactiveDescription, textStyleSetting2);
							hoverTextDrawer.EndShadowBar();
						}
						LogicGate component4 = kselectable.GetComponent<LogicGate>();
						LogicGateBase.PortId portId;
						if (component4 != null && component4.TryGetPortAtCell(num, out portId))
						{
							int portValue = component4.GetPortValue(portId);
							bool portConnected = component4.GetPortConnected(portId);
							LogicGate.LogicGateDescriptions.Description portDescription = component4.GetPortDescription(portId);
							hoverTextDrawer.BeginShadowBar(false);
							if (portId == LogicGateBase.PortId.OutputOne)
							{
								hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_MULTI_OUTPUT_HOVER_FMT.Replace("{Port}", portDescription.name).Replace("{Name}", kselectable.GetProperName().ToUpper()), component2.Styles_Title.Standard);
							}
							else
							{
								hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_MULTI_INPUT_HOVER_FMT.Replace("{Port}", portDescription.name).Replace("{Name}", kselectable.GetProperName().ToUpper()), component2.Styles_Title.Standard);
							}
							hoverTextDrawer.NewLine(26);
							TextStyleSetting textStyleSetting3;
							if (portConnected)
							{
								textStyleSetting3 = ((portValue == 1) ? component2.Styles_LogicActive.Selected : component2.Styles_LogicSignalInactive);
							}
							else
							{
								textStyleSetting3 = component2.Styles_LogicActive.Standard;
							}
							hoverTextDrawer.DrawIcon((portValue == 1 && portConnected) ? component2.iconActiveAutomationPort : component2.iconDash, textStyleSetting3.textColor, 18, 2);
							hoverTextDrawer.DrawText(portDescription.active, textStyleSetting3);
							hoverTextDrawer.NewLine(26);
							TextStyleSetting textStyleSetting4;
							if (portConnected)
							{
								textStyleSetting4 = ((portValue == 0) ? component2.Styles_LogicStandby.Selected : component2.Styles_LogicSignalInactive);
							}
							else
							{
								textStyleSetting4 = component2.Styles_LogicStandby.Standard;
							}
							hoverTextDrawer.DrawIcon((portValue == 0 && portConnected) ? component2.iconActiveAutomationPort : component2.iconDash, textStyleSetting4.textColor, 18, 2);
							hoverTextDrawer.DrawText(portDescription.inactive, textStyleSetting4);
							hoverTextDrawer.EndShadowBar();
						}
					}
					goto IL_0702;
				}
			}
			if (mode == OverlayModes.Power.ID)
			{
				CircuitManager circuitManager = Game.Instance.circuitManager;
				ushort circuitID = circuitManager.GetCircuitID(num);
				if (circuitID != 65535)
				{
					hoverTextDrawer.BeginShadowBar(false);
					float num5 = circuitManager.GetWattsNeededWhenActive(circuitID);
					num5 += this.currentDef.EnergyConsumptionWhenActive;
					float maxSafeWattageForCircuit = circuitManager.GetMaxSafeWattageForCircuit(circuitID);
					Color color = ((num5 >= maxSafeWattageForCircuit) ? Color.red : Color.white);
					hoverTextDrawer.AddIndent(num3);
					hoverTextDrawer.DrawText(string.Format(UI.DETAILTABS.ENERGYGENERATOR.POTENTIAL_WATTAGE_CONSUMED, GameUtil.GetFormattedWattage(num5, GameUtil.WattageFormatterUnit.Automatic, true)), this.Styles_BodyText.Standard, color, true);
					hoverTextDrawer.EndShadowBar();
				}
			}
		}
		IL_0702:
		hoverTextDrawer.EndDrawing();
	}

	public BuildingDef currentDef;
}
