using System;
using System.Collections.Generic;

public class AttackToolHoverTextCard : HoverTextConfiguration
{
	public override void ConfigureHoverScreen()
	{
		if (!string.IsNullOrEmpty(this.ActionStringKey))
		{
			this.ActionName = Strings.Get(this.ActionStringKey);
		}
		HoverTextScreen instance = HoverTextScreen.Instance;
		if (instance.LoadPreConfiguredToolFields(this))
		{
			this.isConfigured = true;
			return;
		}
		instance.ToggleIncubating(true);
		instance.currentConfiguration = this;
		instance.ClearLabels();
		if (this.printTitle)
		{
			this.ConfigureTitle(instance, true);
		}
		this.isConfigured = true;
	}

	public override void UpdateHoverElements(List<KSelectable> hover_objects)
	{
		base.UpdateHoverElements(hover_objects);
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		hoverTextDrawer.BeginShadowBar(false);
		base.DrawTitle(instance, hoverTextDrawer);
		base.DrawInstructions(HoverTextScreen.Instance, hoverTextDrawer);
		hoverTextDrawer.EndShadowBar();
		if (hover_objects != null)
		{
			foreach (KSelectable kselectable in hover_objects)
			{
				if (kselectable.GetComponent<AttackableBase>() != null)
				{
					hoverTextDrawer.BeginShadowBar(false);
					hoverTextDrawer.DrawText(kselectable.GetProperName().ToUpper(), this.Styles_Title.Standard);
					hoverTextDrawer.EndShadowBar();
					break;
				}
			}
		}
		hoverTextDrawer.EndDrawing();
	}

	private AttackToolHoverTextCard.HoverScreenFields hoverScreenElements;

	private struct HoverScreenFields
	{
		public ShadowBar ShadowBar;

		public LocText SelectableName;
	}
}
