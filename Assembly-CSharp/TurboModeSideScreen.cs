using System;
using UnityEngine;

public class TurboModeSideScreen : SideScreenContent
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		MultiToggle multiToggle = this.toggle;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(this.OnClick));
	}

	private void Refresh()
	{
		this.toggle.ChangeState((this.target.UserSliderSetting == 0f) ? 0 : 1);
	}

	private void OnClick()
	{
		this.target.SetUserSpecifiedPowerConsumptionValue((this.target.UserSliderSetting == 0f) ? this.target.maxPower : this.target.minPower);
		this.Refresh();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		SpaceHeater component = target.GetComponent<SpaceHeater>();
		return component != null && component.heatLiquid;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		if (target == null)
		{
			global::Debug.LogError("The target object provided was null");
			return;
		}
		this.target = target.GetComponent<SpaceHeater>();
		if (this.target == null)
		{
			global::Debug.LogError("The target provided does not have an ICheckboxControl component");
			return;
		}
		this.Refresh();
	}

	public override void ClearTarget()
	{
		base.ClearTarget();
		this.target = null;
	}

	public MultiToggle toggle;

	public LocText label;

	private SpaceHeater target;
}
