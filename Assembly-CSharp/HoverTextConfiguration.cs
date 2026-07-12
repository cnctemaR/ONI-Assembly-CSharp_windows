using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/HoverTextConfiguration")]
public class HoverTextConfiguration : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.ConfigureHoverScreen();
	}

	protected virtual void ConfigureTitle(HoverTextScreen screen)
	{
		if (string.IsNullOrEmpty(this.ToolName))
		{
			this.ToolName = Strings.Get(this.ToolNameStringKey).String.ToUpper();
		}
	}

	protected void DrawTitle(HoverTextScreen screen, HoverTextDrawer drawer)
	{
		drawer.DrawText(this.ToolName, this.ToolTitleTextStyle);
	}

	protected void DrawInstructions(HoverTextScreen screen, HoverTextDrawer drawer)
	{
		TextStyleSetting standard = this.Styles_Instruction.Standard;
		drawer.NewLine(26);
		if (KInputManager.currentControllerIsGamepad)
		{
			drawer.DrawIcon(KInputManager.steamInputInterpreter.GetActionSprite(global::Action.MouseLeft, false), 20);
		}
		else
		{
			drawer.DrawIcon(screen.GetSprite("icon_mouse_left"), 20);
		}
		drawer.DrawText(this.ActionName, standard);
		drawer.AddIndent(8);
		if (KInputManager.currentControllerIsGamepad)
		{
			drawer.DrawIcon(KInputManager.steamInputInterpreter.GetActionSprite(global::Action.MouseRight, false), 20);
		}
		else
		{
			drawer.DrawIcon(screen.GetSprite("icon_mouse_right"), 20);
		}
		drawer.DrawText(this.backStr, standard);
	}

	public virtual void ConfigureHoverScreen()
	{
		if (!string.IsNullOrEmpty(this.ActionStringKey))
		{
			this.ActionName = Strings.Get(this.ActionStringKey);
		}
		HoverTextScreen instance = HoverTextScreen.Instance;
		this.ConfigureTitle(instance);
		this.backStr = UI.TOOLS.GENERIC.BACK.ToString().ToUpper();
	}

	public virtual void UpdateHoverElements(List<KSelectable> hover_objects)
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
		this.DrawTitle(instance, hoverTextDrawer);
		this.DrawInstructions(HoverTextScreen.Instance, hoverTextDrawer);
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
	}

	public TextStyleSetting[] HoverTextStyleSettings;

	public string ToolNameStringKey = "";

	public string ActionStringKey = "";

	[HideInInspector]
	public string ActionName = "";

	[HideInInspector]
	public string ToolName;

	protected string backStr;

	public TextStyleSetting ToolTitleTextStyle;

	public HoverTextConfiguration.TextStylePair Styles_Title;

	public HoverTextConfiguration.TextStylePair Styles_BodyText;

	public HoverTextConfiguration.TextStylePair Styles_Instruction;

	public HoverTextConfiguration.TextStylePair Styles_Warning;

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
