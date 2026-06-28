using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class HoverTextConfiguration : KMonoBehaviour
{
	protected void SetLineActive(GameObject LineObject, bool active)
	{
		if (LineObject.activeSelf != active)
		{
			LineObject.SetActive(active);
		}
	}

	protected virtual void ConfigureTitle(HoverTextScreen screen)
	{
		if (string.IsNullOrEmpty(this.ToolName))
		{
			this.ToolName = Strings.Get(this.ToolNameStringKey).String;
		}
		this.TitleLine = screen.NewLine("Title Line", 24);
		this.TitleText = screen.AddText(this.ToolName, this.ToolTitleTextStyle, true);
	}

	protected virtual void ConfigureInstructions(HoverTextScreen screen)
	{
		TextStyleSetting standard = this.Styles_Instruction.Standard;
		screen.NewLine("Instructions", 24);
		screen.AddIcon(screen.GetSprite("icon_mouse_left"), 16f);
		screen.AddText(this.ActionName, standard, true);
		screen.AddIndent(8f, 18f);
		screen.AddIcon(screen.GetSprite("icon_mouse_right"), 16f);
		screen.AddText(UI.TOOLS.GENERIC.BACK, standard, true);
	}

	public virtual void ConfigureHoverScreen()
	{
		if (!string.IsNullOrEmpty(this.ActionStringKey))
		{
			this.ActionName = Strings.Get(this.ActionStringKey);
		}
		HoverTextScreen instance = HoverTextScreen.Instance;
		instance.ClearLabels();
		instance.NewLine("Spacer", 24);
		instance.StartShadowBar(0f, 0f, false);
		if (this.printTitle)
		{
			this.ConfigureTitle(instance);
		}
		this.ConfigureInstructions(instance);
		instance.EndShadowBar();
	}

	public virtual void UpdateHoverElements(KSelectable[] hoverObjects)
	{
	}

	public bool printTitle = true;

	public TextStyleSetting[] HoverTextStyleSettings;

	public string ToolNameStringKey = string.Empty;

	public string ActionStringKey = string.Empty;

	[HideInInspector]
	public string ActionName = string.Empty;

	[HideInInspector]
	public string ToolName;

	protected GameObject TitleLine;

	protected LocText TitleText;

	protected Text ActionText;

	protected Color iconColor_basic = Color.white;

	public TextStyleSetting ToolTitleTextStyle;

	public HoverTextConfiguration.TextStylePair Styles_Title;

	public HoverTextConfiguration.TextStylePair Styles_BodyText;

	public HoverTextConfiguration.TextStylePair Styles_Instruction;

	public HoverTextConfiguration.ValuePropertyTextStyles Styles_Values;

	[Serializable]
	public struct TextStylePair
	{
		public TextStyleSetting Standard;

		public TextStyleSetting Selected;
	}

	[Serializable]
	public struct ValuePropertyTextStyles
	{
		public HoverTextConfiguration.TextStylePair Property;

		public HoverTextConfiguration.TextStylePair Property_Decimal;

		public HoverTextConfiguration.TextStylePair Property_Unit;
	}
}
