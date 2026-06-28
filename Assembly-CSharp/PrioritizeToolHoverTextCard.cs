using System;
using System.Collections.Generic;
using STRINGS;

public class PrioritizeToolHoverTextCard : HoverTextConfiguration
{
	public override void ConfigureHoverScreen()
	{
		HoverTextScreen instance = HoverTextScreen.Instance;
		if (instance.LoadPreConfiguredToolFields(this))
		{
			this.isConfigured = true;
			return;
		}
		instance.currentConfiguration = this;
		instance.ToggleIncubating(true);
		if (!string.IsNullOrEmpty(this.ActionStringKey))
		{
			this.ActionName = Strings.Get(this.ActionStringKey);
		}
		instance.ClearLabels();
		if (this.printTitle)
		{
			this.ConfigureTitle(instance, true);
		}
		this.isConfigured = true;
	}

	public override void UpdateHoverElements(List<KSelectable> selected)
	{
		if (ToolMenuPriorityScreen.Instance == null)
		{
			return;
		}
		base.UpdateHoverElements(selected);
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		hoverTextDrawer.BeginShadowBar(false);
		base.DrawTitle(HoverTextScreen.Instance, hoverTextDrawer);
		base.DrawInstructions(HoverTextScreen.Instance, hoverTextDrawer);
		hoverTextDrawer.NewLine(26);
		hoverTextDrawer.DrawText(string.Format(UI.TOOLS.PRIORITIZE.SPECIFIC_PRIORITY, ToolMenuPriorityScreen.Instance.GetScreenPriority().priority_value.ToString()), this.Styles_Title.Standard);
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
	}
}
