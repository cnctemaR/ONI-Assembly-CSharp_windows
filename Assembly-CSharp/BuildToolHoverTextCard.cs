using System;
using STRINGS;
using UnityEngine;

public class BuildToolHoverTextCard : HoverTextConfiguration
{
	public override void ConfigureHoverScreen()
	{
		this.ActionName = ((!(this.currentDef != null) || !this.currentDef.DragBuild) ? UI.TOOLS.BUILD.TOOLACTION : UI.TOOLS.BUILD.TOOLACTION_DRAG);
		if (this.currentDef != null && this.currentDef.Name != null)
		{
			this.ToolName = string.Format(UI.TOOLS.BUILD.NAME, this.currentDef.Name);
		}
		HoverTextScreen instance = HoverTextScreen.Instance;
		int num = 0;
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
		instance.NewLine("BuildWarning", num);
		instance.AddIndent(8f, 18f);
		this.BuildWarningText = instance.AddText(string.Empty, this.HoverTextStyleSettings[1], false);
		this.RotateLine = instance.NewLine("RotateLine", num);
		instance.AddIndent(8f, 18f);
		instance.AddText(UI.TOOLTIPS.HELP_ROTATE_KEY, this.Styles_Instruction.Standard, false);
		base.SetLineActive(this.BuildWarningText.transform.parent.gameObject, false);
		base.SetLineActive(this.RotateLine, false);
		instance.NewLine("Power", num);
		instance.AddIndent(8f, 18f);
		this.PowerText = instance.AddText(string.Empty, this.Styles_BodyText.Standard, false);
		instance.NewLine("MaterialsRemaining", num);
		instance.AddIndent(8f, 18f);
		this.MaterialsRemainingText = instance.AddText(string.Empty, this.Styles_BodyText.Standard, false);
		instance.EndShadowBar();
		this.isConfigured = true;
	}

	public override void UpdateHoverElements(KSelectable[] hoverObjects_dont_use_this_is_null)
	{
		base.UpdateHoverElements(hoverObjects_dont_use_this_is_null);
		if (!this.isConfigured || this.RotateLine == null)
		{
			this.ConfigureHoverScreen();
		}
		if (this.currentDef != null)
		{
			this.MaterialsRemainingText.text = ResourceRemainingDisplayScreen.instance.GetString();
			if (BuildTool.Instance.isActiveAndEnabled)
			{
				base.SetLineActive(this.RotateLine, this.currentDef.BuildingComplete.GetComponent<Rotatable>() != null);
				Orientation getBuildingOrientation = BuildTool.Instance.GetBuildingOrientation;
				int getLastCell = BuildTool.Instance.GetLastCell;
				string text = "Unknown reason";
				if (!this.currentDef.IsValidBuildLocation(getLastCell, getBuildingOrientation, out text))
				{
					base.SetLineActive(this.BuildWarningText.transform.parent.gameObject, true);
					this.BuildWarningText.text = text;
				}
				else
				{
					base.SetLineActive(this.BuildWarningText.transform.parent.gameObject, false);
				}
			}
			else if (UtilityBuildTool.Instance.isActiveAndEnabled || WireBuildTool.Instance.isActiveAndEnabled)
			{
				if (this.BuildWarningText.transform.parent.gameObject.activeSelf)
				{
					base.SetLineActive(this.BuildWarningText.transform.parent.gameObject, false);
				}
				if (this.RotateLine.transform.gameObject.activeSelf)
				{
					base.SetLineActive(this.RotateLine.gameObject, false);
				}
			}
			bool flag = false;
			int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			CircuitManager circuitManager = Game.Instance.circuitManager;
			ushort circuitID = circuitManager.GetCircuitID(num);
			if (circuitID != 65535)
			{
				flag = true;
				float num2 = circuitManager.GetWattsNeededWhenActive(circuitID);
				num2 += this.currentDef.EnergyConsumptionWhenActive;
				float maxSafeWattageForCircuit = circuitManager.GetMaxSafeWattageForCircuit(circuitID);
				this.PowerText.color = ((num2 < maxSafeWattageForCircuit) ? Color.white : Color.red);
				this.PowerText.text = string.Format(UI.DETAILTABS.ENERGYGENERATOR.POTENTIAL_WATTAGE_CONSUMED, GameUtil.GetFormattedWattage(num2, "F1"));
			}
			base.SetLineActive(this.PowerText.transform.parent.gameObject, flag);
		}
	}

	public BuildingDef currentDef;

	private LocText BuildWarningText;

	private LocText MaterialsRemainingText;

	private LocText PowerText;

	private GameObject RotateLine;
}
