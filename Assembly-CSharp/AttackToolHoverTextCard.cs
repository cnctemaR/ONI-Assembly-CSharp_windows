using System;
using System.Collections.Generic;
using UnityEngine;

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
		instance.NewLine("Spacer", 24);
		instance.StartShadowBar(0f, 0f, false);
		if (this.printTitle)
		{
			this.ConfigureTitle(instance);
		}
		this.ConfigureInstructions(instance);
		instance.EndShadowBar();
		instance.NewLine("Spacer", 24);
		this.hoverScreenElements.ShadowBar = instance.StartShadowBar(0f, 0f, false);
		instance.NewLine("SelectableName", 24);
		this.hoverScreenElements.SelectableName = instance.AddText(string.Empty, this.Styles_Title.Standard, true);
		instance.EndShadowBar();
		this.isConfigured = true;
	}

	public override void SetNotConfigured()
	{
		base.SetNotConfigured();
	}

	public override void UpdateHoverElements(List<KSelectable> hover_objects)
	{
		if (!this.isConfigured)
		{
			this.ConfigureHoverScreen();
		}
		else
		{
			bool flag = false;
			this.hoverScreenElements.SelectableName.text = string.Empty;
			if (hover_objects != null)
			{
				foreach (KSelectable kselectable in hover_objects)
				{
					if (kselectable.GetComponent<Harvestable>() != null)
					{
						this.hoverScreenElements.SelectableName.text = kselectable.GetProperName().ToUpper();
						flag = true;
						break;
					}
				}
			}
			this.hoverScreenElements.ShadowBar.gameObject.rectTransform().localScale = ((!flag) ? Vector3.zero : Vector3.one);
			this.hoverScreenElements.SelectableName.gameObject.rectTransform().localScale = ((!flag) ? Vector3.zero : Vector3.one);
		}
	}

	private AttackToolHoverTextCard.HoverScreenFields hoverScreenElements;

	private struct HoverScreenFields
	{
		public ShadowBar ShadowBar;

		public LocText SelectableName;
	}
}
