using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class PlaceToolHoverTextCard : HoverTextConfiguration
{
	public override void UpdateHoverElements(List<KSelectable> hoverObjects)
	{
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
		if (!Grid.IsValidCell(num) || (int)Grid.WorldIdx[num] != ClusterManager.Instance.activeWorldId)
		{
			hoverTextDrawer.EndDrawing();
			return;
		}
		hoverTextDrawer.BeginShadowBar(false);
		this.ActionName = UI.TOOLS.PLACE.TOOLACTION;
		if (this.currentPlaceable != null && this.currentPlaceable.GetProperName() != null)
		{
			this.ToolName = string.Format(UI.TOOLS.PLACE.NAME, this.currentPlaceable.GetProperName());
		}
		base.DrawTitle(instance, hoverTextDrawer);
		base.DrawInstructions(instance, hoverTextDrawer);
		int num2 = 26;
		int num3 = 8;
		string text;
		if (this.currentPlaceable != null && !this.currentPlaceable.IsValidPlaceLocation(num, out text))
		{
			hoverTextDrawer.NewLine(num2);
			hoverTextDrawer.AddIndent(num3);
			hoverTextDrawer.DrawText(text, this.HoverTextStyleSettings[1]);
		}
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
	}

	public Placeable currentPlaceable;
}
