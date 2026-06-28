using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class MoveToLocationToolHoverTextCard : HoverTextConfiguration
{
	public override void ConfigureHoverScreen()
	{
		HoverTextScreen instance = HoverTextScreen.Instance;
		if (!string.IsNullOrEmpty(this.ActionStringKey))
		{
			this.ActionName = Strings.Get(this.ActionStringKey);
		}
		if (instance.LoadPreConfiguredToolFields(this))
		{
			this.isConfigured = true;
			return;
		}
		instance.ToggleIncubating(true);
		instance.ClearLabels();
		if (this.printTitle)
		{
			this.ConfigureTitle(instance, true);
		}
		this.isConfigured = true;
	}

	public override void UpdateHoverElements(List<KSelectable> selected)
	{
		base.UpdateHoverElements(selected);
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
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
			hoverTextDrawer.DrawText(UI.TOOLS.MOVETOLOCATION.UNREACHABLE, this.Styles_Title.Standard);
		}
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
	}

	private LocText unreachableLine;
}
