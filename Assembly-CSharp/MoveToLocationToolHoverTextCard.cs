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
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		hoverTextDrawer.BeginShadowBar(false);
		base.DrawTitle(HoverTextScreen.Instance, hoverTextDrawer);
		base.DrawInstructions(HoverTextScreen.Instance, hoverTextDrawer);
		bool flag = !MoveToLocationTool.Instance.CanMoveTo(num);
		if (flag)
		{
			hoverTextDrawer.NewLine(26);
			hoverTextDrawer.DrawText(UI.TOOLS.MOVETOLOCATION.UNREACHABLE, this.HoverTextStyleSettings[1]);
		}
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
	}
}
