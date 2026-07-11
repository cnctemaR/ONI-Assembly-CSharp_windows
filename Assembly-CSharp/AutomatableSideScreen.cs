using System;
using STRINGS;
using UnityEngine;

public class AutomatableSideScreen : SideScreenContent
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.allowManualToggle.transform.parent.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.AUTOMATABLE_SIDE_SCREEN.ALLOWMANUALBUTTONTOOLTIP);
		this.allowManualToggle.onValueChanged += this.OnAllowManualChanged;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<Automatable>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		if (target == null)
		{
			global::Debug.LogError("The target object provided was null");
			return;
		}
		this.targetAutomatable = target.GetComponent<Automatable>();
		if (this.targetAutomatable == null)
		{
			global::Debug.LogError("The target provided does not have an Automatable component");
			return;
		}
		this.allowManualToggle.isOn = !this.targetAutomatable.GetAutomationOnly();
		this.allowManualToggleCheckMark.enabled = this.allowManualToggle.isOn;
	}

	private void OnAllowManualChanged(bool value)
	{
		this.targetAutomatable.SetAutomationOnly(!value);
		this.allowManualToggleCheckMark.enabled = value;
	}

	public KToggle allowManualToggle;

	public KImage allowManualToggleCheckMark;

	public GameObject content;

	private GameObject target;

	public LocText DescriptionText;

	private Automatable targetAutomatable;
}
