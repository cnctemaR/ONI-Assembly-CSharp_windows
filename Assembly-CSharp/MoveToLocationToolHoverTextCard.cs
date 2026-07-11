using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class MoveToLocationToolHoverTextCard : HoverTextConfiguration
{
	public override void UpdateHoverElements(List<KSelectable> selected)
	{
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		HoverTextDrawer hoverTextDrawer = HoverTextScreen.Instance.BeginDrawing();
		hoverTextDrawer.BeginShadowBar(false);
		base.DrawTitle(HoverTextScreen.Instance, hoverTextDrawer);
		base.DrawInstructions(HoverTextScreen.Instance, hoverTextDrawer);
		if (!MoveToLocationTool.Instance.CanMoveTo(num))
		{
			hoverTextDrawer.NewLine(26);
			hoverTextDrawer.DrawText(UI.TOOLS.MOVETOLOCATION.UNREACHABLE, this.HoverTextStyleSettings[1]);
		}
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
	}
}
