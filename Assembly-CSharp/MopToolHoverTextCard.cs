using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class MopToolHoverTextCard : HoverTextConfiguration
{
	public override void ConfigureHoverScreen()
	{
		using (new KProfiler.Region("ConfigureMopToolHoverScreen", null))
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
				instance.ClearLabels();
				instance.NewLine("Spacer", 24);
				instance.StartShadowBar(0f, 0f, false);
				this.hoverScreenElements.UnknownAreaLine = instance.NewLine("UnknownArea", 24);
				instance.AddIcon(instance.GetSprite("iconWarning"), 18f);
				instance.AddIndent(4f, 18f);
				instance.AddText(UI.TOOLS.GENERIC.UNKNOWN, null, true);
				instance.EndShadowBar();
				base.SetLineActive(this.hoverScreenElements.UnknownAreaLine, false);
				instance.StartShadowBar(0f, 0f, false);
				this.ConfigureTitle(instance);
				instance.NewLine("Line_ElementName", 24);
				this.hoverScreenElements.ElementName = instance.AddText("", this.Styles_Title.Standard, true);
				instance.NewLine("Line_Category", 24);
				instance.AddIcon(instance.GetSprite("dash"), this.iconColor_basic, 18f);
				instance.AddIndent(4f, 18f);
				this.hoverScreenElements.ElementCategory = instance.AddText("", this.Styles_Title.Standard, false);
				instance.NewLine("Mass", 24);
				instance.AddIcon(instance.GetSprite("dash"), this.iconColor_basic, 18f);
				instance.AddIndent(4f, 18f);
				this.hoverScreenElements.ElementMass = new LocText[4];
				this.hoverScreenElements.ElementMass[0] = instance.AddText("", this.Styles_Values.Property.Standard, true);
				this.hoverScreenElements.ElementMass[1] = instance.AddText("", this.Styles_Values.Property_Decimal.Standard, true);
				this.hoverScreenElements.ElementMass[2] = instance.AddText("", this.Styles_Values.Property_Unit.Standard, false);
				this.hoverScreenElements.ElementMass[3] = instance.AddText("", this.Styles_Values.Property_Unit.Standard, true);
				instance.NewLine("NewLine", 24);
				instance.AddIcon(instance.GetSprite("icon_mouse_left"), 16f);
				LocText locText = instance.AddText(UI.TOOLS.MOP.TOOLACTION, this.Styles_Instruction.Standard, true);
				locText.gameObject.name = "ActionText";
				instance.AddIndent(8f, 18f);
				instance.AddIcon(instance.GetSprite("icon_mouse_right"), 16f);
				instance.AddText(UI.TOOLS.GENERIC.BACK, this.Styles_Instruction.Standard, true);
				instance.EndShadowBar();
				this.isConfigured = true;
			}
		}
	}

	public override void UpdateHoverElements(List<KSelectable> selected)
	{
		if (!this.isConfigured || this.hoverScreenElements.ElementCategory == null)
		{
			this.ConfigureHoverScreen();
		}
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		if (Grid.IsValidCell(num))
		{
			bool flag = Grid.Visible[num] > 0 || DebugHandler.FreeCameraMode;
			bool flag2 = false;
			if (flag && Grid.Element[num].IsLiquid)
			{
				flag2 = true;
			}
			base.SetLineActive(this.hoverScreenElements.UnknownAreaLine, Grid.Visible[num] == 0 && !flag2 && !DebugPaintElementScreen.Instance.gameObject.activeSelf);
			base.SetLineActive(this.hoverScreenElements.ElementMass[0].transform.parent.gameObject, flag2);
			base.SetLineActive(this.hoverScreenElements.ElementName.transform.parent.gameObject, flag2);
			base.SetLineActive(this.hoverScreenElements.ElementCategory.transform.parent.gameObject, flag2);
			if (flag2)
			{
				this.hoverScreenElements.ElementName.GetComponent<SetTextStyleSetting>().SetStyle(this.Styles_Title.Standard);
				for (int i = 0; i < this.hoverScreenElements.ElementMass.Length; i++)
				{
					this.hoverScreenElements.ElementMass[i].GetComponent<SetTextStyleSetting>().SetStyle(this.Styles_BodyText.Standard);
				}
				this.hoverScreenElements.ElementCategory.GetComponent<SetTextStyleSetting>().SetStyle(this.Styles_BodyText.Standard);
				this.hoverScreenElements.ElementCategory.text = ElementLoader.elements[(int)Grid.Cell[num].elementIdx].GetMaterialCategoryTag().ProperName();
				this.hoverScreenElements.ElementName.text = ElementLoader.elements[(int)Grid.Cell[num].elementIdx].name.ToUpper();
				base.SetLineActive(this.hoverScreenElements.ElementCategory.transform.parent.gameObject, !ElementLoader.elements[(int)Grid.Cell[num].elementIdx].IsVacuum);
				string[] array = WorldInspector.MassStrings(num);
				if (this.hoverScreenElements.ElementMass[0].text != array[0])
				{
					this.hoverScreenElements.ElementMass[0].text = array[0];
				}
				if (this.hoverScreenElements.ElementMass[1].text != array[1])
				{
					this.hoverScreenElements.ElementMass[1].text = array[1];
				}
				if (this.hoverScreenElements.ElementMass[2].text != array[2])
				{
					this.hoverScreenElements.ElementMass[2].text = array[2];
				}
				if (this.hoverScreenElements.ElementMass[3].text != array[3])
				{
					this.hoverScreenElements.ElementMass[3].text = array[3];
				}
			}
		}
	}

	private MopToolHoverTextCard.HoverScreenFields hoverScreenElements;

	private struct HoverScreenFields
	{
		public GameObject UnknownAreaLine;

		public Image ElementStateIcon;

		public LocText ElementCategory;

		public LocText ElementName;

		public LocText[] ElementMass;

		public LocText ElementHardness;

		public LocText ElementHardnessDescription;
	}
}
