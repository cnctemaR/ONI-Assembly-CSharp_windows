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
		}
		else
		{
			instance.currentConfiguration = this;
			instance.ToggleIncubating(true);
			if (!string.IsNullOrEmpty(this.ActionStringKey))
			{
				this.ActionName = Strings.Get(this.ActionStringKey);
			}
			instance.ClearLabels();
			instance.NewLine("spacer", 24);
			instance.StartShadowBar(0f, 0f, false);
			if (this.printTitle)
			{
				this.ConfigureTitle(instance);
			}
			this.ConfigureInstructions(instance);
			instance.NewLine("PriorityText", 24);
			this.priorityLine = instance.AddText("", this.Styles_Title.Standard, true);
			instance.EndShadowBar();
			this.isConfigured = true;
		}
	}

	public override void UpdateHoverElements(List<KSelectable> selected)
	{
		if (!(ToolMenuPriorityScreen.Instance == null))
		{
			if (!this.isConfigured || this.priorityLine == null)
			{
				this.ConfigureHoverScreen();
			}
			this.priorityLine.text = string.Format(UI.TOOLS.PRIORITIZE.SPECIFIC_PRIORITY, ToolMenuPriorityScreen.Instance.GetScreenPriority().ToString());
		}
	}

	private LocText priorityLine;
}
