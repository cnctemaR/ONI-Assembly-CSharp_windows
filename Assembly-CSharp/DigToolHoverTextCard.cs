using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class DigToolHoverTextCard : HoverTextConfiguration
{
	public override void ConfigureHoverScreen()
	{
		using (new KProfiler.Region("ConfigureDigToolHoverScreen", null))
		{
			if (!string.IsNullOrEmpty(this.ActionStringKey))
			{
				this.ActionName = Strings.Get(this.ActionStringKey);
			}
			HoverTextScreen instance = HoverTextScreen.Instance;
			if (instance.LoadPreConfiguredToolFields(this))
			{
				this.isConfigured = true;
			}
			else
			{
				instance.ToggleIncubating(true);
				instance.currentConfiguration = this;
				instance.ClearLabels();
				instance.NewLine("Spacer", 24);
				instance.StartShadowBar(0f, 0f, false);
				this.hoverScreenElements.UnknownAreaLine = instance.NewLine("UnknownArea_DigTool", 24);
				instance.AddIcon(instance.GetSprite("iconWarning"), 18f);
				instance.AddIndent(4f, 18f);
				instance.AddText(UI.TOOLS.GENERIC.UNKNOWN, null, true);
				instance.EndShadowBar();
				base.SetLineActive(this.hoverScreenElements.UnknownAreaLine, false);
				instance.StartShadowBar(0f, 0f, false);
				this.ConfigureTitle(instance);
				this.ConfigureInstructions(instance);
				instance.NewLine("Line_ElementName", 24);
				this.hoverScreenElements.ElementName = instance.AddText(string.Empty, this.Styles_Title.Standard, true);
				instance.NewLine("Line_Category", 24);
				instance.AddIcon(instance.GetSprite("iconHex01"), this.iconColor_basic, 18f);
				instance.AddIndent(4f, 18f);
				this.hoverScreenElements.ElementCategory = instance.AddText(string.Empty, this.Styles_Title.Standard, false);
				instance.NewLine("Mass", 24);
				instance.AddIcon(instance.GetSprite("inspectorUI_mass_icon_orange"), this.iconColor_basic, 18f);
				instance.AddIndent(4f, 18f);
				this.hoverScreenElements.ElementMass = new LocText[4];
				this.hoverScreenElements.ElementMass[0] = instance.AddText(string.Empty, this.Styles_Values.Property.Standard, true);
				this.hoverScreenElements.ElementMass[1] = instance.AddText(string.Empty, this.Styles_Values.Property_Decimal.Standard, true);
				this.hoverScreenElements.ElementMass[2] = instance.AddText(string.Empty, this.Styles_Values.Property_Unit.Standard, false);
				this.hoverScreenElements.ElementMass[3] = instance.AddText(string.Empty, this.Styles_Values.Property_Unit.Standard, true);
				instance.NewLine("HardnessLine", 24);
				instance.AddIcon(instance.GetSprite("inspectorUI_hardness_icon"), this.iconColor_basic, 18f);
				instance.AddIndent(1f, 18f);
				this.hoverScreenElements.ElementHardnessDescription = instance.AddText(string.Empty, this.Styles_Values.Property_Unit.Standard, true);
				instance.EndShadowBar();
				this.isConfigured = true;
			}
		}
	}

	public override void SetNotConfigured()
	{
		base.SetNotConfigured();
	}

	public override void UpdateHoverElements(KSelectable[] selected)
	{
		if (!this.isConfigured || this.hoverScreenElements.ElementCategory == null)
		{
			this.ConfigureHoverScreen();
		}
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		bool flag = false;
		if (Grid.Visible[num] > 0 && Grid.Solid[num] && Diggable.IsDiggable(num))
		{
			flag = true;
		}
		else if (Grid.Visible[num] == 0 && DebugPaintElementScreen.Instance.gameObject.activeSelf && Diggable.IsDiggable(num) && Grid.Solid[num] && Diggable.IsDiggable(num))
		{
			flag = true;
		}
		base.SetLineActive(this.TitleLine, Grid.Visible[num] > 0);
		base.SetLineActive(this.InstructionLine, Grid.Visible[num] > 0);
		base.SetLineActive(this.hoverScreenElements.UnknownAreaLine, Grid.Visible[num] == 0 && !flag && !DebugPaintElementScreen.Instance.gameObject.activeSelf);
		base.SetLineActive(this.hoverScreenElements.ElementMass[0].transform.parent.gameObject, flag);
		base.SetLineActive(this.hoverScreenElements.ElementName.transform.parent.gameObject, flag);
		base.SetLineActive(this.hoverScreenElements.ElementCategory.transform.parent.gameObject, flag);
		base.SetLineActive(this.hoverScreenElements.ElementHardnessDescription.transform.parent.gameObject, flag);
		this.hoverScreenElements.ElementHardnessDescription.text = ((!Grid.Element[num].IsSolid) ? string.Empty : (" " + GameUtil.GetHardnessString(Grid.Element[num], true)));
		if (flag)
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

	private DigToolHoverTextCard.HoverScreenFields hoverScreenElements;

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
